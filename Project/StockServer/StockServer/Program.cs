using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using StockServer.Conversations.StockStreamRequest;
using StockServer.Data;
using System;
using System.Net;

namespace StockServer
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        static void Main(string[] args)
        {
            Log.Debug($"{nameof(Main)} (enter)");

            StockData.Init();
            ConversationManager.Start(ConversationBuilder);
            var listenEndpoint = new IPEndPoint(IPAddress.Any, Config.GetInt(Config.STOCK_SERVER_PORT));
            PostOffice.AddBox(listenEndpoint.ToString());
            
            Log.Info("Waiting for StockStreamRequestMessages.");
            Log.Info("Pres any key to close program.");
            Console.ReadKey();


            Log.Debug($"{nameof(Main)} (exit)");
        }
        
        public static Conversation ConversationBuilder(Envelope e)
        {
            Conversation conv = null;

            switch (e.Contents)
            {
                case StockStreamRequestMessage m:
                    conv = new ConvR_StockStreamRequest(e);
                    conv.SetInitialState(new RespondStockStreamRequest_InitialState(e,conv));
                    break;
            }
            
            return conv;
        }
    }
}
