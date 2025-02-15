using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlackLegionBot.CommandHandling;
using BlackLegionBot.TwitchApi.Models;
using Newtonsoft.Json;
using Refit;

namespace BlackLegionBot.TwitchApi
{
    public class TwitchAuthManager
    {
        private const string BaseAuthUrl = "https://id.twitch.tv/oauth2/authorize";
        private const string RedirectUrl = "https://blackdragonai.nl/api/auth/authorized";
        private readonly ITwitchAuthApi _twitchAuthApi;
        private readonly UserInfo _userInfo;
        private readonly AuthTokens _tokens = new AuthTokens();
        public event Action<string> WhisperNeedsToBeSend;
        private System.Timers.Timer _timer;


        public TwitchAuthManager(ITwitchAuthApi twitchAuthApi, UserInfo userInfo)
        {
            this._twitchAuthApi = twitchAuthApi;
            this._userInfo = userInfo;

            ReadTokensFromFile();
        }

        public async Task UseAuthorizationToken(string code)
        {
            try
            {
                Console.WriteLine($"Apply code: {code}");
                var authResult = await this._twitchAuthApi.Authorize(this._userInfo.ClientId, this._userInfo.Secret, code, redirect_uri: RedirectUrl);
                Console.WriteLine($"Get access token and refresh token result: {JsonConvert.SerializeObject(authResult)}");

                _tokens.RefreshToken = authResult.RefreshToken;
                Console.WriteLine($"Refresh token: {authResult.RefreshToken}");
                await RefreshToken();
            }
            catch (ApiException exception)
            {
                Console.Error.WriteLine(exception.Content);
            }
        }

        public async Task RefreshToken()
        {
            try
            {
                Console.WriteLine("Starting Token Refresh");
                var tokenRefreshResult =
                    await this._twitchAuthApi.RefreshTokens(_userInfo.ClientId, _userInfo.Secret, _tokens.RefreshToken);
                _tokens.AccessToken = tokenRefreshResult.AccessToken;
                _tokens.RefreshToken = tokenRefreshResult.RefreshToken;
                Console.WriteLine("Writing tokens");
                Console.WriteLine($"Access token: {_tokens.AccessToken}");
                Console.WriteLine($"Refresh token: {_tokens.RefreshToken}");
                await using var sw = new StreamWriter("AuthTokens.json");
                await sw.WriteAsync(JsonConvert.SerializeObject(_tokens));
                sw.Close();

                _timer = new System.Timers.Timer(tokenRefreshResult.ExpiresIn * 1000);
                this._timer.Elapsed += (obj, args) => RefreshToken().RunSynchronously();
                this._timer.AutoReset = false;
                this._timer.Enabled = true;
                this._timer.Start();
            }
            catch (ApiException exception)
            {
                Console.Error.WriteLine(exception.Content);
            }
        }

        public void ReadTokensFromFile()
        {
            const string authTokensPath = "AuthTokens.json";
            if (!File.Exists(authTokensPath))
            {
                var authToken = new AuthTokens()
                {
                    AccessToken = "",
                    RefreshToken = ""
                };
                File.WriteAllText(authTokensPath, JsonConvert.SerializeObject(authToken));
            }

            using var sr = new StreamReader(authTokensPath);
            var tokens = JsonConvert.DeserializeObject<AuthTokens>(sr.ReadToEnd());
            this._tokens.AccessToken = tokens.AccessToken;
            this._tokens.RefreshToken = tokens.RefreshToken;
            Console.WriteLine($"Tokens:\nAccessToken: {tokens.AccessToken}\nRefreshToken: {tokens.RefreshToken}");
        }

        public async Task Reauthorize()
        {
            var mesg = $"Please click the following url to authorize the bot: {GetAuthorizationUrl()}";
            this.WhisperNeedsToBeSend?.Invoke(mesg);
            await ListenForNewToken();
        }

        public async Task ListenForNewToken()
        {
            using var listener = new HttpListener();
            listener.Prefixes.Add("http://*:80/bot/auth/");
            listener.Start();

            var result = await listener.GetContextAsync();
            var requestUrl = result.Request.Url;
            if (string.IsNullOrEmpty(requestUrl.Query))
            {
                SendResponseOnListener(result.Response, "Something went wrong and was not able to authorize");
                return;
            }
            var indexOfCode = requestUrl.Query.IndexOf("code=", StringComparison.InvariantCultureIgnoreCase);
            if (indexOfCode < 0)
            {
                WhisperNeedsToBeSend?.Invoke("Something went wrong with the attempt to re-authorize.");
            }
            var authToken = requestUrl.Query.Substring(indexOfCode + 5, requestUrl.Query.IndexOf("&") - 6);
            Console.WriteLine($"Token: {authToken}");
            await UseAuthorizationToken(authToken);
            SendResponseOnListener(result.Response, "Processing authorization result");
        }

        private static void SendResponseOnListener(HttpListenerResponse response, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);

            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/plain";
            response.StatusCode = (int)HttpStatusCode.OK;

            // Write response
            using var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            response.Close(); // Close response
        }

        public string GetAccessToken() => $"Bearer {_tokens.AccessToken}";

        public string GetAuthorizationUrl() =>
            $"{BaseAuthUrl}?client_id={this._userInfo.ClientId}&redirect_uri=http://127.0.0.1:11037/&response_type=code&scope={GetAuthScopes().Aggregate((scope1, scope2) => $"{scope1}+{scope2}")}";
        
        private IEnumerable<string> GetAuthScopes() => new[] {
            "analytics:read:extensions", "analytics:read:games", "bits:read", "channel:edit:commercial", 
            "channel:read:hype_train", "channel:read:subscriptions", "clips:edit", "user:edit", "user:edit:broadcast",
            "user:edit:follows", "user:read:broadcast", "user:read:email"
        };
    }
}
