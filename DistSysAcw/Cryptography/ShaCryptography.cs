using System;
using System.Security.Cryptography;
using System.Text;

namespace DistSysAcw.Cryptography
{
    #region 

    // https://docs.microsoft.com/en-us/dotnet/api/system.bitconverter.tostring?view=net
    
    public static class ShaCryptography
    {
        public static string Sha1Encrypt(string message)
        {
            using var sha1 = new SHA1CryptoServiceProvider();
            return BitConverter.ToString(sha1.ComputeHash(Encoding.ASCII.GetBytes(message))) .Replace("-", "");         
        }
        public static string Sha256Encrypt(string message)
        {
            using var sha256 = new SHA256CryptoServiceProvider();
            return BitConverter.ToString(sha256.ComputeHash(Encoding.ASCII.GetBytes(message))) .Replace("-", "");
        }
    }
    //Api transfers message to bytes first.
    //one way transformations unable to be decrypted, with hash function.
    // Functionality decoupled.
    #endregion
}
