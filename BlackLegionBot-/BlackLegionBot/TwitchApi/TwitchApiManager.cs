using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackLegionBot.CommandHandling;
using BlackLegionBot.NonCommandBased;
using BlackLegionBot.TwitchApi.Models;
using Newtonsoft.Json;
using Refit;

namespace BlackLegionBot.TwitchApi
{
    public class TwitchApiManager
    {
        public readonly TwitchAuthManager AuthManager;
        private readonly ITwitchApiManager _apiClient;
        private readonly UserInfo _userInfo;

        public TwitchApiManager(TwitchAuthManager authManager, ITwitchApiManager apiClient, UserInfo userInfo)
        {
            this.AuthManager = authManager;
            this._apiClient = apiClient;
            this._userInfo = userInfo;
        }

        public async Task Initialize()
        {
            await this.AuthManager.RefreshToken();
        }

        public async Task<IEnumerable<BroadcasterSubscription>> GetSubscriptions()
        {
            string pageToLoad = null;
            bool shouldLoadNextPage;
            var subs = new List<BroadcasterSubscription>();
            do
            {
                var nextPageOfSubs = await this._apiClient.GetBroadcasterSubscriptions(this.AuthManager.GetAccessToken(), this._userInfo.ClientId, this._userInfo.UserId, pageToLoad);
                subs.AddRange(nextPageOfSubs.Data);
                pageToLoad = nextPageOfSubs.Pagination.Cursor;
                shouldLoadNextPage = nextPageOfSubs.Data.Any();
            } 
            while (shouldLoadNextPage);

            return subs;
        }

        public async Task<StreamData> GetStreamData() =>
            (await this._apiClient.GetStreamData(this.AuthManager.GetAccessToken(), this._userInfo.ClientId, this._userInfo.UserId)).Data.FirstOrDefault();

        public async Task<GameInfo> GetGameInfo(string id = null, string name = null) =>
            (await this._apiClient.GetGameInfo(this.AuthManager.GetAccessToken(), this._userInfo.ClientId, id, name)).Data.FirstOrDefault();

        public async Task<int> GetFollowCount() =>
            (await this._apiClient.GetFollowerInfo(this.AuthManager.GetAccessToken(), this._userInfo.ClientId, this._userInfo.UserId, null, null, 1)).Total;

        public async Task<FollowerInfo> GetFollowerInfo(string userIdToRetrieve) =>
            (await this._apiClient.GetFollowerInfo(this.AuthManager.GetAccessToken(), this._userInfo.ClientId, this._userInfo.UserId, userIdToRetrieve, null, 1)).Data.First();

        public async Task<UserDetails> GetUserDetails(string name) =>
            (await this._apiClient.GetUserDetails(this.AuthManager.GetAccessToken(), this._userInfo.ClientId, null, name)).Data.FirstOrDefault();

        public async Task<ChannelInfo> GetChannelInfo()
        {
            var channelInfoList = await this._apiClient.GetChannelInfo(this.AuthManager.GetAccessToken(), this._userInfo.ClientId,
                this._userInfo.UserId);
            Console.WriteLine($"Channel info result: {JsonConvert.SerializeObject(channelInfoList)}");
            return channelInfoList.Data.First();
        }

        public async Task UpdateChannelInfo(ChannelInfo channelInfo) =>
            await this._apiClient.UpdateChannelInfo(this.AuthManager.GetAccessToken(), this._userInfo.ClientId, this._userInfo.UserId, channelInfo);

        public async Task<CommercialStartOutput> StartCommercial(ECommercialLength length) =>
            await this._apiClient.StartCommercial(this.AuthManager.GetAccessToken(), this._userInfo.ClientId,
                new CommercialStartInput()
                {
                    BroadcasterId = this._userInfo.UserId,
                    Length = (int)length
                });

        public async Task<bool> IsLive() =>
            (await this._apiClient.GetStreamData(this.AuthManager.GetAccessToken(), this._userInfo.ClientId,
                this._userInfo.UserId)).Data.Any();

        public string GetAccessToken() => this.AuthManager.GetAccessToken();
    }
}
