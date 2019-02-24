using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.MailService;
using Shared.Comms.Messages;

namespace SharedTest.MailService
{
    [TestClass]
    public class EnvelopeTest
    {
        [TestMethod]
        public void DefaultConstructorTest()
        {
            var e = new Envelope();

            Assert.IsNull(e.To);
            Assert.IsNull(e.From);
            Assert.IsNull(e.Contents);
        }

        [TestMethod]
        public void AddressTest()
        {
            var toEP = new IPEndPoint(IPAddress.Any, 0);
            var fromEP = new IPEndPoint(IPAddress.Any, 0);

            var e = new Envelope()
            {
                To = toEP,
                From = fromEP
            };

            Assert.AreSame(toEP, e.To);
            Assert.AreSame(fromEP, e.From);
        }

        [TestMethod]
        public void ContentsTest()
        {
            var m = MessageFactory.GetMessage<AckMessage>(1, 5);
            var e = new Envelope(m);

            Assert.AreSame(m, e.Contents);
        }

        [TestMethod]
        public void InsertTest()
        {
            var m = MessageFactory.GetMessage<AckMessage>(1, 5);
            var e = new Envelope();

            Assert.IsFalse(e.HasMessage());

            e.Insert(m);

            Assert.IsTrue(e.HasMessage());
        }

        [TestMethod]
        public void InsertFailTest()
        {
            var m = MessageFactory.GetMessage<AckMessage>(1, 5);
            var m2 = MessageFactory.GetMessage<AckMessage>(3, 7);
            var e = new Envelope(m);

            try
            {
                e.Insert(m2);
                Assert.Fail("Expected exception not thrown.");
            }
            catch (System.Exception error)
            {
                Assert.AreEqual("Envelope already has contents", error.Message);
            };
        }

        [TestMethod]
        public void RemovalTest()
        {
            var m = MessageFactory.GetMessage<AckMessage>(1, 5);
            var e = new Envelope(m);

            Assert.IsTrue(e.HasMessage());

            var m2 = e.Remove();

            Assert.IsFalse(e.HasMessage());

            Assert.AreSame(m, m2);
            Assert.IsNull(e.Contents);
        }

        [TestMethod]
        public void EmptyRemovalTest()
        {
            var e = new Envelope();
            var m = e.Remove();

            Assert.IsNull(m);
        }
    }
}