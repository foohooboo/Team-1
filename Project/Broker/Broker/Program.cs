using System;
using log4net;
using Shared;
using Shared.Comms.Messages;
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

            var comm = new CommSystemWrapper();

            Message ack = new AckMessage();
            string encodedAck = ack.Encode();
            Log.Info("Encoded Ack Message...");
            Log.Info(encodedAck);
            Message decoded = MessageFactory.GetMessage(encodedAck);
            //Log.Info(string.Format("Decoded Message:  type={0}",decoded.MType));

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

            Log.Debug($"{nameof(Main)} (exit)");
        }

        static void PrintMenu()
        {
            Console.WriteLine("\nInput Options:");
            Console.WriteLine("   -\"exit\" to end");
            Console.WriteLine("   -anything else to start a StockStreamRequest conversation.");
        }
    }
}
