using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Shared.MarketStructures;
using Shared.Security;
using System;

namespace SharedTest.Security
{
    [TestClass]
    public class SignatureServiceTest
    {
        static SignatureService SigServe = new SignatureService();

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            var key = GenerateKeys(1024);
            SigServe.PublicKey = (RsaKeyParameters)(key.Public);
            SigServe.PrivateKey = (RsaKeyParameters)(key.Private);

            //var publicKeyModulas = Convert.ToBase64String(publicKey.Modulus.ToByteArray());
            //var publicKeyExponent = Convert.ToBase64String(publicKey.Exponent.ToByteArray());
            //var privateKeyModulas = Convert.ToBase64String(privateKey.Modulus.ToByteArray());
            //var privateKeyExponent = Convert.ToBase64String(privateKey.Exponent.ToByteArray());
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {

        }

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
            var marketDay = new MarketDay("testDay", stocks);

            //Execute
            var serialized = SigServe.Serialize(marketDay);
            var deserialziedMarketDay = SigServe.Deserialize<MarketDay>(serialized);

            //Assert
            Assert.IsTrue(deserialziedMarketDay.Equals(marketDay));
        }

        [TestMethod]
        public void ValidMarketDaySignatureTest()
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

            var marketDay = new MarketDay("testDay", stocks);
            string signature = SigServe.GetSignature(marketDay);

            var isVerified = SigServe.VerifySignature(marketDay, signature);
            Assert.IsTrue(isVerified);
        }

        [TestMethod]
        public void MarketDaySignatureInvalidSigTest()
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

            var marketDay = new MarketDay("testDay", stocks);
            string signature = SigServe.GetSignature(marketDay);

            
            for (int i = 0; i < 15; i++)
            {
                signature = signature.Replace(signature[i], 'j');
            }

            var isVerified = SigServe.VerifySignature(marketDay, signature);
            Assert.IsFalse(isVerified);
        }

        [TestMethod]
        public void MarketDaySignatureInvalidDataTest()
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

            var marketDay = new MarketDay("testDay", stocks);
            string signature = SigServe.GetSignature(marketDay);

            var modifiedMarketDay = new MarketDay("testDay_modified", stocks);

            var isVerified = SigServe.VerifySignature(modifiedMarketDay, signature);
            Assert.IsFalse(isVerified);
        }

        private static AsymmetricCipherKeyPair GenerateKeys(int keySize)
        {
            var gen = new RsaKeyPairGenerator();
            var secureRandom = new SecureRandom();
            var keyGenParam = new KeyGenerationParameters(secureRandom, keySize);
            gen.Init(keyGenParam);
            return gen.GenerateKeyPair();
        }
    }
}