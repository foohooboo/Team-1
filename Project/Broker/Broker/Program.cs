using System;
using Broker.Conversations.States;
using log4net;
using Shared.Conversations;
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
                var c = new ConvI_StockStreamRequest(new InitialSate_ConvI_StockStreamRequest());
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
