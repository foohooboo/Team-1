using System;
using log4net;
using Shared;
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
            var comm = new CommSystemWrapper(); //TODO: Update this example once Post Office allows us to open a listener.

            PrintMenu();
            var input = Console.ReadLine();

            while (!input.Equals("exit"))
            {
                var c = new ConvI_StockStreamRequest(new InitialState_ConvI_StockStreamRequest(Config.GetInt(Config.BROKER_PROCESS_NUM)));
                Log.Info($"Starting new conversation: {c.ConversationId}");
                ConversationManager.AddConversation(c);

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
