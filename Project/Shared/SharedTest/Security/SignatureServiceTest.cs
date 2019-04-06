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
    [DoNotParallelize]
    public class SignatureServiceTest
    {
        static SignatureService SigServe = new SignatureService();

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            var key = GenerateKeys(1024);
            SignatureService.PublicKey = (RsaKeyParameters)(key.Public);
            SignatureService.PrivateKey = (RsaKeyParameters)(key.Private);

            //Please keep the following comments, they're nice to have when generating new private/public key string data. -Dsphar 4/6/2019
            //var publicKeyModulas = Convert.ToBase64String(SignatureService.PublicKey.Modulus.ToByteArray());
            //var publicKeyExponent = Convert.ToBase64String(SignatureService.PublicKey.Exponent.ToByteArray());
            //var privateKeyModulas = Convert.ToBase64String(SignatureService.PrivateKey.Modulus.ToByteArray());
            //var privateKeyExponent = Convert.ToBase64String(SignatureService.PrivateKey.Exponent.ToByteArray());
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

        [TestMethod]
        public void SetKeysTest()
        {

            //Clear keys from other tests
            SignatureService.PublicKey = null;
            SignatureService.PrivateKey = null;

            Assert.IsNull(SignatureService.PublicKey);
            Assert.IsNull(SignatureService.PrivateKey);

            //Load Keys
            SignatureService.SetPrivateKey(
                "AK3PGB1aToiVVGu0buZLm+P5PSAgaSIwaCoFJkuB+CC8OCiMRjPH8i567OM+DH5CU/XZh6yy6uffJ/uCyDygZYVL7viFiLzjHtBEAh/9Ma7cECrMtPQTUo+kN5JYQNnFfBy6PXjMI+jbAc2ZAwID6dCcK6OpejrP08sqW3ZOzBYZ",
                "Gbp6dhvgXLEIPGJK+U2vb519KiSKE4zJWpEFFG/SkFv0TzJGkRMzuyQorVHJzSXZ4l53Sj347mZ293DqXakbpdN8Xua/4uuHj8edjeSgOdGvJ+O4I/8NMrFep4Ve1a2D8ZOmjA8NjWE4wVKc/6oHI3jLOy+PiPd8hWfVyooUbEU="
                );

            SignatureService.SetPublicKey(
                "AK3PGB1aToiVVGu0buZLm+P5PSAgaSIwaCoFJkuB+CC8OCiMRjPH8i567OM+DH5CU/XZh6yy6uffJ/uCyDygZYVL7viFiLzjHtBEAh/9Ma7cECrMtPQTUo+kN5JYQNnFfBy6PXjMI+jbAc2ZAwID6dCcK6OpejrP08sqW3ZOzBYZ",
                "AQAB"
                );

            //Prepare a MarketDay
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

            //Sign marketDay
            string signature = SigServe.GetSignature(marketDay);

            //Verify signature
            var isVerified = SigServe.VerifySignature(marketDay, signature);
            Assert.IsTrue(isVerified);
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