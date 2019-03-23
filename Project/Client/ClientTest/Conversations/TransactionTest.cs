using System;
using Client.Conversations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.MailService;
using Shared.Conversations;
using Shared.MarketStructures;

namespace ClientTest.Conversations
{
    [TestClass]
    public class TransactionTest
    {

        [TestInitialize]
        public void TestInitialize()
        {
            PostOffice.AddBox("0.0.0.0:0");
            ConversationManager.Start(null);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ConversationManager.Stop();
            PostOffice.RemoveBox("0.0.0.0:0");
        }


        [TestMethod]
        public void RequestSucceed()
        {
            int portfolioId = 42;
            var testStock = new Stock("TST", "Test Stock");
            var vStock = new ValuatedStock(("1984-02-22,11.0289,11.0822,10.7222,10.7222,197402").Split(','), testStock);

            var conv = new InitiateTransactionConversation(portfolioId,vStock,1);
            ConversationManager.AddConversation(conv);



        }
    }
}
