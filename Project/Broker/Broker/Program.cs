using System;
using log4net;
using Shared;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.Conversations.StockStreamRequest.Initiator;

namespace Broker
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        private static void Main(string[] args)
        {
            Log.Debug($"{nameof(Main)} (enter)");

            ConversationManager.Start(null);
            PostOffice.AddBox("0.0.0.0:0");
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

        static void PrintMenu()
        {
            Console.WriteLine("Input Options:");
            Console.WriteLine("   -\"exit\" to end");
            Console.WriteLine("   -anything else to start a StockStreamRequest conversation.");
        }
    }
}
