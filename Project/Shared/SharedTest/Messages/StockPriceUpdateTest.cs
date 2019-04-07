using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Crypto.Parameters;
using Shared.Comms.Messages;
using Shared.MarketStructures;
using Shared.Security;
using System;

namespace SharedTest.Messages
{
    [TestClass]
    public class StockPriceUpdateTest
    {
        SignatureService sigServ = new SignatureService();

        public StockPriceUpdateTest()
        {

        }

        [TestMethod]
        public void DefaultConstructorTest()
        {
            var stockPriceUpdate = new StockPriceUpdate();

            var tradedCompanies = sigServ.Deserialize<MarketDay>(Convert.FromBase64String(stockPriceUpdate.SerializedStockList));

            Assert.IsNull(stockPriceUpdate.StocksList.TradedCompanies);
            Assert.IsNull(stockPriceUpdate.StocksList.Date);
        }

        [TestMethod]
        public void LoadedConstructorTest()
        {
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 };
            var date = "1990-02-20";

            var marketDay = new MarketDay(date, stocks);
            var stockPriceUpdate = new StockPriceUpdate(marketDay);

            Assert.AreEqual(3, stockPriceUpdate.StocksList.TradedCompanies.Count);
            Assert.AreEqual(stockPriceUpdate.StocksList.Date, date);
        }

        [TestMethod]
        public void InitializerTest()
        {
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 };
            var date = "1990-02-20";

            var marketDay = new MarketDay(date, stocks);
            var stockPriceUpdate = new StockPriceUpdate
            {
                SerializedStockList = Convert.ToBase64String(sigServ.Serialize(marketDay))
            };

            Assert.AreEqual(3, stockPriceUpdate.StocksList.TradedCompanies.Count);
            Assert.AreEqual(stockPriceUpdate.StocksList.Date, date);
        }

        [TestMethod]
        public void InheritsMessageTest()
        {
            var stockPriceUpdate = new StockPriceUpdate();

            Assert.AreEqual(stockPriceUpdate.SourceID, 0);
            Assert.IsNull(stockPriceUpdate.ConversationID);
            Assert.IsNull(stockPriceUpdate.MessageID);
        }

        [TestMethod]
        public void SerializerTest()
        {
            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 };
            var date = "1990-02-20";

            var marketDay = new MarketDay(date, stocks);
            var stockPriceUpdate = new StockPriceUpdate(marketDay);

            var serializedMessage = stockPriceUpdate.Encode();
            var deserializedMessage = MessageFactory.GetMessage(serializedMessage, false) as StockPriceUpdate;

            Assert.AreEqual(stockPriceUpdate.StocksList.TradedCompanies.Count, deserializedMessage.StocksList.TradedCompanies.Count);
            Assert.AreEqual(stockPriceUpdate.StocksList.Date, deserializedMessage.StocksList.Date);
        }

        [TestMethod]
        public void ValidSignatureTest()
        {
            var key = SignatureService.GenerateKeys(1024);
            SignatureService.PublicKey = (RsaKeyParameters)(key.Public);
            SignatureService.PrivateKey = (RsaKeyParameters)(key.Private);
            var sigServe = new SignatureService();

            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 };
            var date = "1990-02-29";

            var marketDay = new MarketDay(date, stocks);
            var marketDaySig = sigServe.GetSignature(marketDay);
            var stockPriceUpdate = new StockPriceUpdate(marketDay)
            {
                StockListSignature = marketDaySig
            };

            var isOriginalValid = sigServe.VerifySignature(marketDay, marketDaySig);
            Assert.IsTrue(isOriginalValid);

            var serializedMessage = stockPriceUpdate.Encode();
            var deserializedMessage = MessageFactory.GetMessage(serializedMessage, false) as StockPriceUpdate;

            MarketDay deserializedDay = sigServ.Deserialize<MarketDay>(Convert.FromBase64String(deserializedMessage.SerializedStockList));

            var isDeserializedValid = sigServe.VerifySignature(deserializedMessage.StocksList, deserializedMessage.StockListSignature);
            Assert.IsTrue(isDeserializedValid);

            Assert.AreEqual(stockPriceUpdate.StocksList.TradedCompanies.Count, deserializedMessage.StocksList.TradedCompanies.Count);
            Assert.AreEqual(stockPriceUpdate.StocksList.Date, deserializedMessage.StocksList.Date);
            Assert.IsTrue(sigServe.VerifySignature(deserializedMessage.StocksList, deserializedMessage.StockListSignature));
        }

        [TestMethod]
        public void InvalidSignatureTest()
        {
            var key = SignatureService.GenerateKeys(1024);
            SignatureService.PublicKey = (RsaKeyParameters)(key.Public);
            SignatureService.PrivateKey = (RsaKeyParameters)(key.Private);
            var sigServe = new SignatureService();

            var stock1 = new ValuatedStock();
            var stock2 = new ValuatedStock();
            ValuatedStock[] stocks = { stock1, stock2 };
            var date = "1990-02-29";

            var marketDay = new MarketDay(date, stocks);
            var marketDaySig = sigServe.GetSignature(marketDay);
            var stockPriceUpdate = new StockPriceUpdate(marketDay)
            {
                StockListSignature = marketDaySig
            };

            var isOriginalValid = sigServe.VerifySignature(marketDay, marketDaySig);
            Assert.IsTrue(isOriginalValid);

            var serializedMessage = stockPriceUpdate.Encode();
            var deserializedMessage = MessageFactory.GetMessage(serializedMessage, false) as StockPriceUpdate;

            //"currupt" signature
            deserializedMessage.StockListSignature = deserializedMessage.StockListSignature.Replace(deserializedMessage.StockListSignature.Substring(0, 5), "fffff");

            MarketDay deserializedDay = sigServ.Deserialize<MarketDay>(Convert.FromBase64String(deserializedMessage.SerializedStockList));

            var isDeserializedValid = sigServe.VerifySignature(deserializedMessage.StocksList, deserializedMessage.StockListSignature);
            Assert.IsFalse(isDeserializedValid);
        }
    }
}
