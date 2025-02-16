using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.TwitchApi.Models;
using Refit;

namespace BlackLegionBot.TwitchApi
{
    public interface ITwitchAuthApi
    {
        [Post("/token")]
        Task<AuthorizationResult> Authorize([Query] string client_id, [Query] string client_secret, [Query] string code, [Query] string grant_type = "authorization_code", [Query] string redirect_uri = "", [Query] string force_verify = "true", [Query] string state = "JustSomeRandomState");

        [Post("/token")]
        Task<TokenRefreshResult> RefreshTokens([Query] string client_id, [Query] string client_secret, [Query] string refresh_token, [Query] string grant_type = "refresh_token");
    }
}
