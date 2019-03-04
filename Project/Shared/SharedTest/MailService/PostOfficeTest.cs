using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Comms.MailService;

namespace SharedTest.MailService
{
    [TestClass]
    public class PostOfficeTest
    {
        [TestMethod]
        public void AddPostBoxTest()
        {
            var a1 = @"127.0.0.1:231";

            Assert.IsFalse(PostOffice.HasPostBox());
            PostOffice.AddBox(a1);
            Assert.IsTrue(PostOffice.HasPostBox());
            PostOffice.RemoveBox(a1);
        }

        [TestMethod]
        public void AccessPostBoxTest()
        {
            var a1 = @"127.0.0.1:231";
            var a2 = @"127.0.0.1:241";
            var a3 = @"127.0.0.1:261";

            PostOffice.AddBox(a1);
            PostOffice.AddBox(a2);
            PostOffice.AddBox(a3);

            var pb = PostOffice.GetBox(a2);
            var addressParts = a2.Split(':');

            Assert.AreEqual(addressParts[0], pb.LocalEndPoint.Address.ToString());
            Assert.AreEqual(addressParts[1], pb.LocalEndPoint.Port.ToString());

            PostOffice.RemoveBox(a1);
            PostOffice.RemoveBox(a2);
            PostOffice.RemoveBox(a3);
        }

        [TestMethod]
        public void RemovePostBoxTest()
        {
            var a1 = @"127.0.0.1:231";
            var a2 = @"127.0.0.1:211";

            PostOffice.AddBox(a1);
            PostOffice.AddBox(a2);

            PostOffice.RemoveBox(a2);
            Assert.IsNull(PostOffice.GetBox(a2));

            PostOffice.RemoveBox(a1);
        }
    }
}