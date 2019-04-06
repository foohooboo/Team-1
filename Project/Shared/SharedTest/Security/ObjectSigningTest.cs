using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared;
using Shared.MarketStructures;
using Shared.Security;

namespace SharedTest.Security
{
    [TestClass]
    public class SignatureServiceTest
    {
        [TestMethod]
        public void SerializationTest()
        {
            //Prepare
            var stock1 = new ValuatedStock()
            {
                Symbol = "STK1",
                Name = "Stock 1",
                Open = 1,
                High = 2,
                Low = 3,
                Close = 4, 
                Volume = 5
            };
            var stock2 = new ValuatedStock()
            {
                Symbol = "STK2",
                Name = "Stock 2",
                Open = 6,
                High = 7,
                Low = 8,
                Close = 9,
                Volume = 10
            };
            ValuatedStock[] stocks = { stock1, stock2 };
            var marketDay = new MarketDay("testDay",stocks);

            SignatureService sigServe = new SignatureService();

            //Execute
            var serialized = sigServe.Serialize(marketDay);
            var deserialziedMarketDay = sigServe.Deserialize<MarketDay>(serialized);

            //Assert
            Assert.IsTrue(deserialziedMarketDay.Equals(marketDay));
            
        }

    }
}