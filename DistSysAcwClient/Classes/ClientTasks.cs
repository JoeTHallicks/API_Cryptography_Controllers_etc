using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DistSysAcwClient.Class
{
    internal class ClientTasks
    {
        //static string uri = "http://150.237.94.9/557982/Api/";
        //static readonly string uri = "https://localhost:44394/api/";
        private const string BaseDomain = "http://150.237.94.9/5579282/";
        private static readonly HttpClient Client = new HttpClient();
        public static string UserName = string.Empty;
        public static string ApiKey = string.Empty;
        public static string PublicKey = string.Empty;

        internal static async Task<string> TalkBackHello()
        {
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseDomain}api/talkback/hello"),
                Method = HttpMethod.Get
            };
            var httpResponse = await Client.SendAsync(httpRequest);
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return responseString;
        }
        internal static async Task<string> TalkBackSort(string integers)
        {
            if (!string.IsNullOrEmpty(integers))
            {
                integers = "&integers=" + integers;
            }
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseDomain}api/talkback/sort?{integers}"),
                Method = HttpMethod.Get
            };
            var httpResponse = await Client.SendAsync(httpRequest);
            return await httpResponse.Content.ReadAsStringAsync();
        }

        internal static async Task<string> UserGet(string userName)
        {
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseDomain}api/user/new?username={userName}"),
                Method = HttpMethod.Get
            };
            var httpResponse = await Client.SendAsync(httpRequest);

            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return responseString;
        }

        internal static async Task<string> UserPost(string userName)
        {
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseDomain}api/user/new"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(userName), Encoding.UTF8, "application/json")
            };
            var httpResponse = await Client.SendAsync(httpRequest);
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode) return responseString;
            UserName = userName;
            ApiKey = responseString;
            return "Got API Key";
        }
        internal static void UserSet(string userName, string apiKey)
        {
            UserName = userName;
            ApiKey = apiKey;
        }
        internal static async Task<bool> UserDelete()
        {
            if (string.IsNullOrWhiteSpace(UserName) 
                || string.IsNullOrWhiteSpace(ApiKey))
            {
                Console.WriteLine("You need to do a User Post or User Set first");
                return false;
            }
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseDomain}api/user/removeuser?username={UserName}"),
                Method = HttpMethod.Delete
            };
            httpRequest.Headers.Add("ApiKey", ApiKey);
            var httpResponse = await Client.SendAsync(httpRequest);
            var content = await httpResponse.Content.ReadAsStringAsync();
            return content.ToLower().Contains("true");
        }
        internal static async Task<string> ChangeUserRole(string userName, string role)
        {
            if (string.IsNullOrWhiteSpace(ApiKey)) return "You need to do a User Post or User Set first";

            var changeRole = new ChangeRole(userName, role);
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseDomain}api/user/changerole"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(changeRole), Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Add("ApiKey", ApiKey);
            var httpResponse = await Client.SendAsync(httpRequest);
            return await httpResponse.Content.ReadAsStringAsync();
        }
        internal static async Task<string> ProtectedHello()
        {
            if (string.IsNullOrWhiteSpace(ApiKey)) return "You need to do a User Post or User Set first";
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseDomain}api/protected/hello"),
                Method = HttpMethod.Get
            };
            httpRequest.Headers.Add("ApiKey", ApiKey);
            var httpResponse = await Client.SendAsync(httpRequest);
            return await httpResponse.Content.ReadAsStringAsync();
        }
        internal static async Task<string> ProtectedSha1(string message)
        {
            if (string.IsNullOrWhiteSpace(ApiKey)) return "You need to do a User Post or User Set first";
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseDomain}api/protected/sha1?message={message}"),
                Method = HttpMethod.Get
            };
            httpRequest.Headers.Add("ApiKey", ApiKey);
            var httpResponse = await Client.SendAsync(httpRequest);
            return await httpResponse.Content.ReadAsStringAsync();
        }
        internal static async Task<string> ProtectedSha256(string message)
        {
            if (string.IsNullOrWhiteSpace(ApiKey)) return "You need to do a User Post or User Set first";
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseDomain}api/protected/sha256?message={message}"),
                Method = HttpMethod.Get
            };
            httpRequest.Headers.Add("ApiKey", ApiKey);
            var httpResponse = await Client.SendAsync(httpRequest);
            return await httpResponse.Content.ReadAsStringAsync();
        }
        internal static async Task<string> GetPublicKey()
        {
            if (string.IsNullOrWhiteSpace(ApiKey)) return "You need to do a User Post or User Set first";
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseDomain}api/protected/getpublickey"),
                Method = HttpMethod.Get
            };
            httpRequest.Headers.Add("ApiKey", ApiKey);
            var httpResponse = await Client.SendAsync(httpRequest);
            if (!httpResponse.IsSuccessStatusCode) return "Couldn’t Get the Public Key";
            PublicKey = await httpResponse.Content.ReadAsStringAsync();
            return "Got Public Key";
        }
        internal static async Task<string> ProtectedSign(string message)
        {
            if (string.IsNullOrWhiteSpace(ApiKey)) return "You need to do a User Post or User Set first";
            if (string.IsNullOrWhiteSpace(PublicKey)) return "Client does not yet have the public key";
            var httpRequest = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseDomain}api/protected/sign?message={message}"),
                Method = HttpMethod.Get
            };
            httpRequest.Headers.Add("ApiKey", ApiKey);
            var httpResponse = await Client.SendAsync(httpRequest);
            if (!httpResponse.IsSuccessStatusCode) return "Message was not successfully signed";
            var signedHexMessage = await httpResponse.Content.ReadAsStringAsync();
            var originalMessageBytes = Encoding.ASCII.GetBytes(message);
            var signedBytes = HexToByteConverter.HexStringToBytes(signedHexMessage);
            var verified = RsaCsp.Verify(PublicKey, originalMessageBytes, signedBytes);
            return verified ? "Message was successfully signed" : "Message was not successfully signed";
        }
        internal static async Task<string> ProtectedAddFifty(string stringInt)
        {
            if (string.IsNullOrWhiteSpace(ApiKey)) return "You need to do a User Post or User Set first";
            if (string.IsNullOrWhiteSpace(PublicKey)) return "Client doesn’t yet have the public key";
            var bytesToEncrypt = AesProvider.GetAesInfo();
            int.TryParse(stringInt, out var integer);
            bytesToEncrypt.Add(BitConverter.GetBytes(integer));
            var encryptedList = RsaCsp.EncryptList(PublicKey, bytesToEncrypt);
            if (encryptedList is null) return "An error occurred!";
            var encryptedHexIv = BitConverter.ToString(encryptedList[1]); // IV
            var encryptedHexSymKey = BitConverter.ToString(encryptedList[0]); // Key            
            var encryptedHexInteger = BitConverter.ToString(encryptedList[2]); // Intege
            var httpRequest = new HttpRequestMessage
            {
                RequestUri =
                    new Uri(
                        $"{BaseDomain}api/protected/addfifty?encryptedInteger={encryptedHexInteger}" +
                        $"&encryptedsymkey={encryptedHexSymKey}&encryptedIV={encryptedHexIv}"),
                Method = HttpMethod.Get
            };
            httpRequest.Headers.Add("ApiKey", ApiKey);
            var httpResponse = await Client.SendAsync(httpRequest);
            if (!httpResponse.IsSuccessStatusCode) return "An error occurred!";
            var aesEncryptedHexInteger = await httpResponse.Content.ReadAsStringAsync();
            var aesEncryptedInteger = HexToByteConverter.HexStringToBytes(aesEncryptedHexInteger);
            var decryptedInteger = AesProvider.Decrypt(bytesToEncrypt[0], bytesToEncrypt[1], aesEncryptedInteger);
            return decryptedInteger ?? "An error occurred!";
        }
    }
}
