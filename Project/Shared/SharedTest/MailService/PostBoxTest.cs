using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.ComService;

namespace SharedTest.MailService
{
    [TestClass]
    public class PostBoxTest
    {
        private class TestPostBox : Client
        {
            public TestPostBox(string address) : base(address)
            {

            }

            public override void Close()
            {
                throw new System.NotImplementedException();
            }

            public override void Send(Envelope envelope)
            {
                
            }
        }

        private string address;
        private Client postBox;

        [TestInitialize]
        public void TestInitialize()
        {
            address = @"127.0.0.1:1234";
            postBox = new TestPostBox(address);
        }

        [TestMethod]
        public void ConstructorTest()
        {
            var addressParts = address.Split(':');

            Assert.AreEqual(addressParts[0], postBox.LocalEndPoint.Address.ToString());
            Assert.AreEqual(addressParts[1], postBox.LocalEndPoint.Port.ToString());
        }
    }
}