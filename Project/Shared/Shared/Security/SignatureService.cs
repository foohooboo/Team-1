using log4net;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Shared.Security
{
    public class SignatureService
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public RsaKeyParameters PrivateKey = null;
        public RsaKeyParameters PublicKey = null;

        public string GetSignature<TSignedObject>(TSignedObject obj)
        {
            Log.Debug($"{nameof(GetSignature)} (enter)");

            string sig = "";

            try
            {
                byte[] serlializedObj = Serialize(obj);
                sig = GetSignature(serlializedObj);
            }
            catch (Exception e)
            {
                Log.Error($"Problem signing {obj.GetType().Name} object...");
                Log.Error(e);
            }

            Log.Debug($"{nameof(GetSignature)} (exit)");
            return sig;
        }

        public bool VerifySignature<TSignedObject>(TSignedObject objectToVerify, string signature)
        {
            Log.Debug($"{nameof(VerifySignature)} (enter)");

            var isValid = false;

            try
            {
                byte[] bytesToVerify = Serialize(objectToVerify);
                isValid = VerifySignature(bytesToVerify, signature);
            }
            catch (Exception e)
            {
                Log.Error($"Problem verifying signature of {objectToVerify.GetType().Name} object...");
                Log.Error(e);
            }

            Log.Debug($"{nameof(VerifySignature)} (exit)");
            return isValid;
        }

        private string GetSignature(byte[] objectAsBytes)
        {
            if (PrivateKey == null)
                throw new NullReferenceException("SingatureService.ProvateKey not loaded.");

            var signer = SignerUtilities.GetSigner("SHA384WithRSAEncryption");
            signer.Init(true, PrivateKey);
            signer.BlockUpdate(objectAsBytes, 0, objectAsBytes.Length);

            return Convert.ToBase64String(signer.GenerateSignature());
        }

        private bool VerifySignature(byte[] objectToVerifyAsBytes, string signature)
        {
            if (PublicKey == null)
                throw new NullReferenceException("SingatureService.PublicKey not loaded.");

            var signer = SignerUtilities.GetSigner("SHA384WithRSAEncryption");
            signer.Init(false, PublicKey);
            signer.BlockUpdate(objectToVerifyAsBytes, 0, objectToVerifyAsBytes.Length);

            return signer.VerifySignature(Convert.FromBase64String(signature));
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
