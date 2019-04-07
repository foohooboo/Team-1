using System;
using System.Net;
using System.Threading.Tasks;
using Broker.Conversations.CreatePortfolio;
using Broker.Conversations.GetPortfolio;
using Broker.Conversations.GetPortfolioResponse;
using Broker.Conversations.LeaderBoardUpdate;
using Broker.Conversations.StockUpdate;
using log4net;
using Shared;
using Shared.Client;
using Shared.Comms.ComService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SendErrorMessage;
using Shared.Conversations.SharedStates;
using Shared.Conversations.StockStreamRequest.Initiator;
using Shared.MarketStructures;
using Shared.PortfolioResources;
using Shared.Security;

namespace Broker
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        private static void Main(string[] args)
        {
            Log.Debug($"{nameof(Main)} (enter)");

            SignatureService.LoadPublicKey("Team1/StockServer");

            PortfolioManager.TryToCreate("dummy", "password", out Portfolio dummyPortfolio);
            ConversationManager.Start(ConversationBuilder);

            ComService.AddClient(Config.DEFAULT_UDP_CLIENT, Config.GetInt(Config.BROKER_PORT));

            PrintMenu();
            var input = Console.ReadLine();

            while (!input.Equals("exit"))
            {
                var conv = new ConvI_StockStreamRequest(
                    Config.GetInt(Config.BROKER_PROCESS_NUM)
                    );
                conv.SetInitialState(new InitialState_ConvI_StockStreamRequest(conv));

                Log.Info($"Starting new conversation: {conv.Id}");

                ConversationManager.AddConversation(conv);

                PrintMenu();
                input = Console.ReadLine();
            }

            ConversationManager.Stop();

            Log.Debug($"{nameof(Main)} (exit)");
        }

        private static void PrintMenu()
        {
            Console.WriteLine("Input Options:");
            Console.WriteLine("   -\"exit\" to end");
            Console.WriteLine("   -anything else to start a StockStreamRequest conversation.");
        }

        public static Conversation ConversationBuilder(Envelope e)
        {
            Conversation conv = null;

            switch (e.Contents)
            {
                case CreatePortfolioRequestMessage m:
                    conv = new CreatePortfoliolResponseConversation(m.ConversationID);
                    conv.SetInitialState(new CreatePortfolioReceiveState(e, conv));
                    break;

                case GetPortfolioRequest m:

                    var user = m.Account.Username;
                    var pass = m.Account.Password;

                    Log.Info($"Received GetPortfolioRequest from {e.To.ToString()} for user:{user} with password:{pass}");

                    if(PortfolioManager.TryToGet(user,pass, out Portfolio portfolio))
                    {
                        Log.Info($"Valid credentials, returning portfolio.");
                        conv = new GetPortfoliolResponseConversation(m.ConversationID);
                        conv.SetInitialState(new GetPortfolioReceiveState(e, conv));
                    }
                    else
                    {
                        Log.Info($"Invalid credentials, return error.");
                        conv = new SendErrorMessageConversation(m.ConversationID);
                        conv.SetInitialState(new SendErrorMessageState("Invalid login credentials.",e, conv,null,Config.GetInt(Config.BROKER_PROCESS_NUM)));
                    }
                                       
                    break;

                case StockPriceUpdate m:

                    var sigServ = new SignatureService();

                    if (!sigServ.VerifySignature<MarketDay>(m.StocksList, m.StockListSignature))
                    {
                        Log.Error("Stock Price Update signature validation failed. Ignoring message.");
                    }
                    else
                    {
                        LeaderboardManager.Market = m.StocksList;

                        var ackCon = new StockUpdateConversation(m.ConversationID);
                        ackCon.SetInitialState(new ProcessStockUpdateState(e, ackCon));
                        ConversationManager.AddConversation(ackCon);

                        //Send updated leaderboard to clients.
                        Task.Run(() =>
                        {
                            foreach(var clientIp in ClientManager.Clients)
                            {
                                var stockUpdateConv = new LeaderBoardUpdateRequestConversation(Config.GetInt(Config.BROKER_PROCESS_NUM));
                                stockUpdateConv.SetInitialState(new LeaderboardSendUpdateState(clientIp, stockUpdateConv, null));
                                ConversationManager.AddConversation(stockUpdateConv);
                            }
                        });
                    }
                    break;
            }

            return conv;
        }
    }
}