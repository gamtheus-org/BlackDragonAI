using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.TwitchApi.Models;
using Refit;

namespace BlackLegionBot.TwitchApi
{
    public interface ITwitchApiManager
    {
        [Get("/helix/subscriptions")]
        Task<ListResultWithPagination<BroadcasterSubscription>> GetBroadcasterSubscriptions([Header("Authorization")] string authHeader, [Header("client-id")] string clientId, [Query] string broadcaster_id, [Query] string after = null);

        [Get("/helix/streams")]
        Task<ListResultWithPagination<StreamData>> GetStreamData([Header("Authorization")] string authHeader, [Header("client-id")] string clientId, [Query] string user_id, [Query] int first = 1);

        /// <summary>
        /// Either the id of the game or the name should be given.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Get("/helix/games")]
        Task<ListResultWithPagination<GameInfo>> GetGameInfo([Header("Authorization")] string authHeader, [Header("client-id")] string clientId, [Query] string id = null, [Query] string name = null);

        // [Get("/helix/users/follows")]
        // Task<ListResultWithPaginationWithTotal<FollowerInfo>> GetFollowerInfo([Header("Authorization")] string authHeader, [Header("client-id")] string clientId, [Query] string to_id, [Query] string from_id = null,
        //     [Query] string after = null, [Query] int? first = null);

        [Get("/helix/channels/followers")]
        Task<ListResultWithPagination<FollowerInfo>> GetFollowerInfo([Header("Authorization")] string authHeader,
            [Header("client-id")] string clientId, [Query] string broadcaster_id, [Query] string user_id = null,
            [Query] int? first = null, string after = null);

        [Get("/helix/users")]
        Task<ListResultPure<UserDetails>> GetUserDetails([Header("Authorization")] string authHeader, [Header("client-id")] string clientId, [Query] string id = null, [Query] string login = null);

        [Get("/helix/channels")]
        Task<ListResultPure<ChannelInfo>> GetChannelInfo([Header("Authorization")] string authHeader, [Header("client-id")] string clientId, [Query] string broadcaster_id);

        [Patch("/helix/channels")]
        Task UpdateChannelInfo([Header("Authorization")] string authHeader, [Header("client-id")] string clientId, [Query] string broadcaster_id, [Body] ChannelInfo channelInfo);

        [Post("/helix/channels/commercial")]
        Task<CommercialStartOutput> StartCommercial([Header("Authorization")] string authHeader, [Header("client-id")] string clientId, [Body] CommercialStartInput commercialDetails);

        [Post("/helix/moderation/bans")]
        Task BanUser([Header("Authorization")] string authHeader, [Header("client-id")] string clientId, [Query] string broadcaster_id, [Query] string moderator_id, [Body] BanUserInputWrapper banDetails);
    }
}
