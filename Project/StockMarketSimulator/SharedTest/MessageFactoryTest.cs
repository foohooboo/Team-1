﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.comms.messages;

namespace SharedTest
{
    /// <summary>
    /// Summary description for MessageFactoryTest
    /// </summary>
    [TestClass]
    public class MessageFactoryTest
    {
        public MessageFactoryTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void AckSerializeTest()
        {
            var a = new AckMessage()
            {
                AckHello = "SerialzeTest"
            };

            var test = MessageFactory.GetMessage(a.Encode()) as AckMessage;

            Assert.AreEqual(typeof(AckMessage), test.GetType());
            Assert.AreEqual(a.AckHello, test.AckHello);
        }

        [TestMethod]
        public void AckMessageCountTest()
        {
            var message = MessageFactory.GetMessage(typeof(AckMessage), 1, 5) as AckMessage;

            Assert.AreEqual(message.MessageID, $"1-5-1");

            message = MessageFactory.GetMessage(typeof(AckMessage), 1, 5) as AckMessage;

            Assert.AreEqual(message.MessageID, $"1-5-2");
        }
    }
}
