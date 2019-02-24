using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.MailService;

namespace SharedTest.MailService
{
    [TestClass]
    public class PostOfficeTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var postOffice = new PostOffice();

            Assert.IsFalse(postOffice.HasPostBox());
        }

        [TestMethod]
        public void AddPostBoxTest()
        {
            var postOffice = new PostOffice();
            var a1 = new IPEndPoint(IPAddress.Any, 10);

            postOffice.AddBox(a1);
            Assert.IsTrue(postOffice.HasPostBox());
        }

        [TestMethod]
        public void AccessPostBoxTest()
        {
            var postOffice = new PostOffice();
            var a1 = new IPEndPoint(IPAddress.Any, 10);
            var a2 = new IPEndPoint(IPAddress.Any, 20);
            var a3 = new IPEndPoint(IPAddress.Any, 30);

            postOffice.AddBox(a1);
            postOffice.AddBox(a2);
            postOffice.AddBox(a3);

            var pb = postOffice.GetBox(a2);
            Assert.AreSame(a2, pb.Address);
        }

        [TestMethod]
        public void RemovePostBoxTest()
        {
            var postOffice = new PostOffice();
            var a1 = new IPEndPoint(IPAddress.Any, 10);
            var a2 = new IPEndPoint(IPAddress.Any, 20);

            postOffice.AddBox(a1);
            postOffice.AddBox(a2);

            postOffice.RemoveBox(a2);
            Assert.IsNull(postOffice.GetBox(a2));
        }
    }
}