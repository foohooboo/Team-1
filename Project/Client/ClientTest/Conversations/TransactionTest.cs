using System;
using Client.Conversations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
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

            //setup response message and mock
            var responseMessage = new PortfolioUpdateMessage(){ConversationID = conv.Id};
            var responseEnv = new Envelope(responseMessage);
            var mock = new Mock<InitTransactionStartingState>(conv);
            mock.Setup(st => st.Send()).Callback(()=> {
                //Pretend a message was sent, and response came back
                ConversationManager.ProcessIncomingMessage(responseEnv); 
            });
            mock.Setup(st => st.HandleMessage(responseEnv)).CallBase();

            //execute test
            conv.SetInitialState(mock.Object as InitTransactionStartingState);

            Assert.IsTrue(conv.CurrentState is InitTransactionStartingState);

            ConversationManager.AddConversation(conv);

            Assert.IsFalse(conv.CurrentState is InitTransactionStartingState);
            Assert.IsTrue(conv.CurrentState is ConversationDoneState);
        }
    }
}
