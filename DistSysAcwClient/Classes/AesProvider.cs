using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace DistSysAcwClient.Class
{
    internal class AesProvider
    {     
        public static List<byte[]> GetAesInfo() // Return a list of newly generated key and initialization vector (IV).
        {         
            using var myAes = Aes.Create(); // Create a new instance of the Aes class.
            myAes.GenerateIV();
            myAes.GenerateKey();
          
            var aesInfo = new List<byte[]>
            {
                myAes.Key,
                myAes.IV
            };
            return aesInfo;
        }
        internal static string Decrypt(byte[] key, byte[] iV, byte[] cipherText)
        {
            try
            {               
                using var myAes = Aes.Create(); // Create an Aes object with the specified key and IV.
                myAes.Key = key;
                myAes.IV = iV;
                var decryptor = myAes.CreateDecryptor(myAes.Key, myAes.IV); // Create a decryptor for stream transform.
                using var msDecrypt = new MemoryStream(cipherText);  // Create the decryption stream.
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                return srDecrypt.ReadToEnd(); // Read the decrypted bytes from the decrypting stream and return them.
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
