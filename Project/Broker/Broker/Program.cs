using System;
using System.Net;
using Broker.Conversations.CreatePortfolio;
using Broker.Conversations.GetPortfolio;
using Broker.Conversations.GetPortfolioResponse;
using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SendErrorMessage;
using Shared.Conversations.SharedStates;
using Shared.Conversations.StockStreamRequest.Initiator;
using Shared.PortfolioResources;

namespace Broker
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        private static void Main(string[] args)
        {
            Log.Debug($"{nameof(Main)} (enter)");

            PortfolioManager.TryToCreate("dummy", "password", out Portfolio dummyPortfolio);
            ConversationManager.Start(ConversationBuilder);
            PostOffice.AddBox("0.0.0.0:0");

            var listenEndpoint = new IPEndPoint(IPAddress.Any, Config.GetInt(Config.BROKER_PORT));
            PostOffice.AddBox(listenEndpoint.ToString());

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
            }

            return conv;
        }


    }
}