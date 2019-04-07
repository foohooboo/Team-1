using Shared.MarketStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedTest.MarketStructureTests
{
    [TestClass]
    public class MarketSegmentTest
    {
        public MarketSegmentTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var marketSegment = new MarketSegment();

            Assert.AreEqual(marketSegment.Count, 0);
        }
    }
}
