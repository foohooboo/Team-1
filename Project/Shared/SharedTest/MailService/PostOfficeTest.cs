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
            var a1 = @"127.0.0.1:231";

            postOffice.AddBox(a1);
            Assert.IsTrue(postOffice.HasPostBox());
        }

        [TestMethod]
        public void AccessPostBoxTest()
        {
            var postOffice = new PostOffice();
            var a1 = @"127.0.0.1:231";
            var a2 = @"127.0.0.1:241";
            var a3 = @"127.0.0.1:261";

            postOffice.AddBox(a1);
            postOffice.AddBox(a2);
            postOffice.AddBox(a3);

            var pb = postOffice.GetBox(a2);
            var addressParts = a2.Split(':');

            Assert.AreEqual(addressParts[0], pb.LocalEndPoint.Address.ToString());
            Assert.AreEqual(addressParts[1], pb.LocalEndPoint.Port.ToString());
        }

        [TestMethod]
        public void RemovePostBoxTest()
        {
            var postOffice = new PostOffice();
            var a1 = @"127.0.0.1:231";
            var a2 = @"127.0.0.1:211";

            postOffice.AddBox(a1);
            postOffice.AddBox(a2);

            postOffice.RemoveBox(a2);
            Assert.IsNull(postOffice.GetBox(a2));
        }
    }
}