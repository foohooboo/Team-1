using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.MailService;
using Shared.Comms.Messages;

namespace SharedTest.MailService
{
    [TestClass]
    public class PostBoxTest
    {
        private IPEndPoint address;
        private PostBox postBox;

        [TestInitialize]
        public void TestInitialize()
        {
            address = new IPEndPoint(IPAddress.Any, 0);
            postBox = new PostBox(address);
        }

        [TestMethod]
        public void ConstructorTest()
        {
            Assert.AreSame(address, postBox.Address);
            Assert.IsFalse(postBox.HasMail());
        }

        [TestMethod]
        public void InsertReceivedMessageTest()
        {
            var m = MessageFactory.GetMessage<AckMessage>(1, 5);
            var e = new Envelope(m);
            postBox.Insert(e);

            Assert.IsTrue(postBox.HasMail());
            Assert.AreSame(e, postBox.GetMail());
        }

        [TestMethod]
        public void GetMailFailTest()
        {
            Assert.IsNull(postBox.GetMail());
        }
    }
}