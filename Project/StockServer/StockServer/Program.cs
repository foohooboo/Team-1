﻿using log4net;
using Shared;
using Shared.Client;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Security;
using SharedResources.Conversations.StockStreamRequest;
using StockServer.Conversations.StockStreamRequest;
using StockServer.Conversations.StockStreamResponse;
using StockServer.Conversations.StockUpdate;
using StockServer.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StockServer
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static bool IsSimRunning = true;
        static object RunSimLock = new object();

        static void Main(string[] args)
        {
            Log.Debug($"{nameof(Main)} (enter)");

            SignatureService.LoadPrivateKey("Team1/StockServer");

            StockData.Init();
            ConversationManager.Start(ConversationBuilder);

            ComService.AddUdpClient(Config.DEFAULT_UDP_CLIENT, Config.GetInt(Config.STOCK_SERVER_PORT));
            ComService.AddTcpListener(Config.DEFAULT_TCP_CLIENT, Config.GetInt(Config.STOCK_SERVER_TCP_PORT));

            Log.Info("Starting simulation.");
            Task.Run(() => RunSimulation());

            Log.Info("Waiting for StockStreamRequestMessages.");
            Log.Info("Pres any key to close program.");
            Console.ReadKey();

            lock (RunSimLock)
            {
                IsSimRunning = false;
            }

            Log.Debug($"{nameof(Main)} (exit)");
        }

        public static Conversation ConversationBuilder(Envelope e)
        {
            Conversation conv = null;

            switch (e.Contents)
            {
                case StockHistoryRequestMessage m:
                    conv = new StockHistoryResponseConversation(e);
                    conv.SetInitialState(new StockHistoryResponseState(e as TcpEnvelope, conv));
                    break;
                case StockStreamRequestMessage m:
                    conv = new StockStreamResponseConversation(m.ConversationID);
                    conv.SetInitialState(new StockStreamResponseState(e, conv));
                    break;
            }

            return conv;
        }

        public static void RunSimulation()
        {
            var tickDelay = Config.GetInt(Config.STOCK_TICK_DELAY);

            var runSimLocal = true;
            lock (RunSimLock)
            {
                runSimLocal = IsSimRunning;
            }

            while (runSimLocal)
            {
                Thread.Sleep(tickDelay);

                var nextMarketDay = StockData.AdvanceDay();
                foreach(var clientIp in ClientManager.Clients.Keys)
                {
                    var updateConv = new StockUpdateSendConversation(Config.GetInt(Config.STOCK_SERVER_PROCESS_NUM));
                    updateConv.SetInitialState(new StockUpdateSendState(nextMarketDay, clientIp, updateConv, null));
                    ConversationManager.AddConversation(updateConv);
                }

                lock (RunSimLock)
                {
                    runSimLocal = IsSimRunning;
                }
            }
        }
    }
}
