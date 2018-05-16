using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ExampleClient
{
    class Program
    {
        private static HttpClient _client;

        static void Main(string[] args)
        {
            Scenario_0();
            //Scenario_1();

            Console.Write("Press Enter to exit ...");
            Console.ReadLine();
        }

        private static void Scenario_0()
        {
            _client = new HttpClient();

            var username = "username";
            var password = "password";

            System.Threading.Thread.Sleep(5000);

            #region Get token

            Token token1 = null;
            try
            {
                Console.WriteLine("#01 Send request to custom OAuth server for a token (JWT)");

                Task.Run(async () =>
                {
                    token1 = await GetToken(username, password);
                }).GetAwaiter().GetResult();

                Console.WriteLine("#02 Token retrieved successfully:");
                Console.WriteLine("  Token: {0}", token1.AccessToken);
                Console.WriteLine("  Expires in (sec): {0}", token1.ExpiresIn);
            }
            catch(Exception ex)
            {
                Console.WriteLine("#02 Token retreive failed");
                Console.WriteLine();
                Console.Write("Press Enter to exit ...");
                Console.ReadLine();
                return;
            }

            #endregion

            int secToSleep = 1;
            Console.WriteLine("#03 Let's sleep for {0} sec", secToSleep);
            System.Threading.Thread.Sleep(secToSleep * 1000);
            Console.WriteLine("#04 Wake up");

            string values = "";
            try
            {
                Console.WriteLine("#05 Send request to source server WebAPI secured method");

                #region Get data (attempt #1)

                Task.Run(async () =>
                {
                    values = await GetAsync(ExampleCommon.Constants.BASE_URL_SOURCE_SERVER + "/api/values", token1);
                }).GetAwaiter().GetResult();

                #endregion

                Console.WriteLine("#06 Response received successfully:");
                Console.WriteLine("  " + values);
            }
            catch
            {
                Console.WriteLine("#06 Response recieve failed, cause token expired");

                #region Refresh token

                Token token2 = null;
                try
                {
                    Console.WriteLine("#07 Refresh token");

                    Task.Run(async () =>
                    {
                        token2 = await RefreshToken(token1);
                    }).GetAwaiter().GetResult();

                    Console.WriteLine("#08 Token refreshed successfully");
                    Console.WriteLine("  Token: {0}", token2.AccessToken);
                    Console.WriteLine("  Expires in (sec): {0}", token2.ExpiresIn);
                }
                catch
                {
                    Console.WriteLine("#08 Token refresh failed");
                    Console.WriteLine();
                    Console.Write("Press Enter to exit ...");
                    Console.ReadLine();
                    return;
                }

                #endregion

                #region Get data (attempt #2)

                try
                {
                    Console.WriteLine("#09 Send request to source server WebAPI secured method");

                    Task.Run(async () =>
                    {
                        values = await GetAsync(ExampleCommon.Constants.BASE_URL_SOURCE_SERVER + "/api/values", token2);
                    }).GetAwaiter().GetResult();

                    Console.WriteLine("#10 Response received successfully:");
                    Console.WriteLine("  " + values);
                }
                catch
                {
                    Console.WriteLine("#10 Response recieve failed, cause token expired");
                    Console.WriteLine();
                    Console.Write("Press Enter to exit ...");
                    Console.ReadLine();
                    return;
                }

                #endregion
            }
        }

        // Scenario for windows authentication
        private static void Scenario_1()
        {
            var username = "username";
            var password = "password";

            var httpClientHandler = new HttpClientHandler()
            {
                Credentials = new NetworkCredential(username, password),
            };

            _client = new HttpClient(httpClientHandler);

            string values;

            #region Get data

            try
            {
                Task.Run(async () =>
                {
                    values = await GetWindowsAsync(ExampleCommon.Constants.BASE_URL_SOURCE_SERVER + "/api/values");
                }).GetAwaiter().GetResult();
                Console.WriteLine("Attempt #1 successful");
            }
            catch
            {
                Console.WriteLine("Attempt #1 failed");
            }

            #endregion
        }

        private static async Task<Token> GetToken(string username, string password)
        {
            #region Get salt

            string salt = null;
            string saltUrl = ExampleCommon.Constants.BASE_URL_OAUTH_SERVER + "/api/auth/salt/?username=" + username;
            Task.Run(async () => { salt = await GetAsync(saltUrl); }).GetAwaiter().GetResult();
            salt = JsonConvert.DeserializeObject(salt).ToString();

            #endregion

            #region Get password hash

            var passwordHash = string.Join("", ExampleClient.Crypto.ComputeSha512Hash(Encoding.UTF8.GetBytes(password + salt)).Select(b => b.ToString("x2")).ToArray());

            #endregion

            var body = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", username },
                { "password", passwordHash }
            };

            var url = ExampleCommon.Constants.BASE_URL_OAUTH_SERVER + ExampleCommon.Constants.TOKEN_ENDPOINT_PATH;
            var json = await PostAsync(url, body);
            var t = JsonConvert.DeserializeObject<Token>(json);

            return t;
        }



        private static async Task<Token> RefreshToken(Token token)
        {
            var body = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", token.RefreshToken }
            };

            var url = ExampleCommon.Constants.BASE_URL_OAUTH_SERVER + ExampleCommon.Constants.TOKEN_ENDPOINT_PATH;
            var json = await PostAsync(url, body);
            var t = JsonConvert.DeserializeObject<Token>(json);

            return t;
        }

        private static string Get(string url, Token token = null)
        {
            /** Usage:
             * 
             * var values = Get("http://localhost:801/api/values", token2.AccessToken);
             * 
             */

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";

            if (token != null && !string.IsNullOrEmpty(token.AccessToken))
            {
                req.Headers.Add("Authorization", "Bearer " + token.AccessToken);
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            string res = string.Empty;
            using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
            {
                res = stream.ReadToEnd();
            }

            return res;
        }

        private static string Post(string url, string body)
        {
            /** Usage:
             * 
             * var res = Post("http://localhost:801/token", "grant_type=password&username=ab&password=abc");
             * var res = Post("http://localhost:801/token", "grant_type=refresh_token&refresh_token=" + token.RefreshToken);
             * 
             */

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            var data = Encoding.ASCII.GetBytes(body);

            using (var stream = req.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var tmp = req.GetResponse();
            HttpWebResponse resp = (HttpWebResponse)tmp;

            string res = string.Empty;
            using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
            {
                res = stream.ReadToEnd();
            }

            return res;
        }

        private static async Task<string> PostAsync(string url, Dictionary<string, string> body)
        {
            var content = new FormUrlEncodedContent(body);
            var responseMessage = await _client.PostAsync(url, content);
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            if (!responseMessage.IsSuccessStatusCode)
                throw new CustomException((int)responseMessage.StatusCode, responseMessage.ReasonPhrase, responseString);

            return responseString;
        }

        private static async Task<string> GetAsync(string url, Token token = null)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            if(token != null && !string.IsNullOrEmpty(token.AccessToken))
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);

            var responseMessage = await _client.GetAsync(url);
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            if (!responseMessage.IsSuccessStatusCode)
                throw new CustomException((int)responseMessage.StatusCode, responseMessage.ReasonPhrase, responseString);

            return responseString;
        }

        private static async Task<string> GetWindowsAsync(string url)
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var responseMessage = await _client.GetAsync(url);
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            if (!responseMessage.IsSuccessStatusCode)
                throw new CustomException((int)responseMessage.StatusCode, responseMessage.ReasonPhrase, responseString);

            return responseString;
        }
    }
}
