using System;
using System.Security.Cryptography;
using System.Text;

namespace DistSysAcw.Cryptography
{
    // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider.usemachinekeystore?view=net
    // key exchange at start of Transport Layer Security (TLS) handshake.
    public class RsaCryptography
    {
        private static RsaCryptography _rsaInstance;                // Singleton RSA instance.-
        private static CspParameters _cspParams;// To tell csp instances to use the machine key, instead of the user profile key store.
        private static readonly object Lock = new object();
        private readonly RSAParameters _rsaParams;                   // Stores the public and private keys.
        private RsaCryptography()
        {
            _cspParams = new CspParameters                   // Use the machine key store instead of the user profile key store.
            {
                KeyContainerName = "DisSysKeyContainer",
                Flags = CspProviderFlags.UseMachineKeyStore
            };
            // New instance of RSACryptoServiceProvider to generate public and private key data.
            using var rsa = new RSACryptoServiceProvider(_cspParams)
            {
                KeySize = 1024
            };           
            _rsaParams = rsa.ExportParameters(true);
        }
        public static RsaCryptography GetRsaInstance()
        {
            if (_rsaInstance != null) return _rsaInstance; // Make instance if not present
            lock (Lock) // Lock the creation of the instance, as multiple clients could pass initial check on startup.
            {
                // Because multiple can get through first check, we have to check again.
                _rsaInstance ??= new RsaCryptography();
            }

            return _rsaInstance;
        }
        public string GetPublicKey()
        {
            try
            {
                using var rsa = new RSACryptoServiceProvider(_cspParams);
                rsa.ImportParameters(_rsaParams);
                return rsa.ToXmlString(false);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public byte[] Encrypt(byte[] plainText)
        {
            try
            {
                using var rsa = new RSACryptoServiceProvider(_cspParams);
                rsa.ImportParameters(_rsaParams);
                return rsa.Encrypt(plainText, false);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public byte[] Decrypt(byte[] cypherText)
        {
            try
            {
                using var rsa = new RSACryptoServiceProvider(_cspParams);
                rsa.ImportParameters(_rsaParams);
                return rsa.Decrypt(cypherText, false);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string Sign(string message)
        {
            try
            {
                using var rsa = new RSACryptoServiceProvider(_cspParams);
                rsa.ImportParameters(_rsaParams);
                // Computes the hash value of the specified byte array using the specified hash algorithm, and signs the resulting hash value.
                // Then converts it to Hex.
                return BitConverter.ToString(rsa.SignData(Encoding.ASCII.GetBytes(message),
                    CryptoConfig.CreateFromName("SHA1")));
            }
            catch (Exception)
            {
                return null;
            }
        }
        #region 

        public string AddFifty(string encryptedHexInteger, string encryptedHexSymkey, string encryptedHexIv)
        {
            var encryptedSymkeyBytes = HexToByteConverter.HexStringToBytes(encryptedHexSymkey);
            var encryptedIntegerBytes = HexToByteConverter.HexStringToBytes(encryptedHexInteger);
            var encryptedIvBytes = HexToByteConverter.HexStringToBytes(encryptedHexIv);
            var decryptedInteger = Decrypt(encryptedIntegerBytes); // Decrypt RSA encrypted strings.
            var decryptedSymkey = Decrypt(encryptedSymkeyBytes);
            var decryptedIv = Decrypt(encryptedIvBytes);
            if (decryptedInteger is null || decryptedSymkey is null 
                                         || decryptedIv is null) return null;
            var integer = BitConverter.ToInt32(decryptedInteger, 0); // Get int and add fifty.
            integer += 50;
            var aesInt = AesCryptography.Encrypt(integer.ToString(), decryptedSymkey, decryptedIv); // Aes encrypt int w/h supplied Key and IV.
            if (aesInt is null) return null;
            return BitConverter.ToString(aesInt); // Hex encode and return.
        }
        #endregion
    }
}
