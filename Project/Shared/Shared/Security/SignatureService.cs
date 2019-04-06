using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Security
{
    public class SignatureService
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly HashAlgorithm HASH_ALGORITHM = new SHA1CryptoServiceProvider();

        public string GetSignature<TSignedObject>(TSignedObject obj, RSAParameters privateKey)
        {
            Log.Debug($"{nameof(GetSignature)} (enter)");

            string sig = "";

            try
            {
                byte[] serlializedObj = Serialize(obj);

                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
                RSAalg.ImportParameters(privateKey);

                var sigBytes = RSAalg.SignData(serlializedObj, HASH_ALGORITHM);
                sig = Convert.ToBase64String(sigBytes);
            }
            catch (Exception e)
            {
                Log.Error($"Problem signing {obj.GetType().Name} object...");
                Log.Error(e);
            }

            Log.Debug($"{nameof(GetSignature)} (exit)");
            return sig;
        }

        public bool VerifySignature<TSignedObject>(TSignedObject obj, string signature, RSAParameters publicKey)
        {
            Log.Debug($"{nameof(VerifySignature)} (enter)");

            var isValid = false;

            try
            {
                byte[] SignedData = Convert.FromBase64String(signature);
                byte[] DataToVerify = Serialize(obj);

                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
                RSAalg.ImportParameters(publicKey);

                isValid = RSAalg.VerifyData(DataToVerify, new SHA1CryptoServiceProvider(), SignedData);
            }
            catch (Exception e)
            {
                Log.Error($"Problem verifying signature of {obj.GetType().Name} object...");
                Log.Error(e);
            }

            Log.Debug($"{nameof(VerifySignature)} (exit)");
            return isValid;
        }


        //TODO: We may want to move the following serialize and deserialize methods into a serialization class? -Dsphar 4/6/2019

        /// <summary>
        /// Convert an object to bytes. Note: the parent object and all of it's descendant objects/fields need to be tagged
        /// [Serializable()]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] Serialize(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] param)
        {
            using (MemoryStream ms = new MemoryStream(param))
            {
                IFormatter br = new BinaryFormatter();
                return (T)br.Deserialize(ms);
            }
        }
    }
}
