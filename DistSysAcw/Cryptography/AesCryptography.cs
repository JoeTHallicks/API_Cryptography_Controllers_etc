using System;
using System.IO;
using System.Security.Cryptography;

namespace DistSysAcw.Cryptography
{
    public class AesCryptography
    {
            // Create an Aes object, key and IV.
        public static byte[] Encrypt(string Text,byte[] iv , byte[] key)
        {
            try
            {
                byte[] encrypted;
                using (var myAes = Aes.Create())
                {
                    myAes.Key = key;
                    myAes.IV = iv;

                    var Encrypt = myAes.CreateEncryptor(myAes.Key, myAes.IV);
                    using var mStreamEncrypt = new MemoryStream();
                    using var csEncrypt = new CryptoStream(mStreamEncrypt,
                        Encrypt, CryptoStreamMode.Write);
                    using (var swEncrypt = new StreamWriter(csEncrypt))                     //outward encryption stream.
                    {
                       
                        swEncrypt.Write(Text);                                              //written to outward stream.
                    }
                    encrypted = mStreamEncrypt.ToArray();
                }
                return encrypted;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
