using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BlackLegionBot.TwitchApi;
using Refit;

namespace BlackLegionBot.NonCommandBased
{
    public class CommercialManager
    {
        private readonly TwitchApiManager _apiClient;
        private Timer _commercialTimer;
        private readonly int _secondsBetweenCommercials = 30 * 60;
        private readonly LiveStatusManager _liveStatusManager;

        public CommercialManager(TwitchApiManager apiClient, LiveStatusManager liveStatusManager)
        {
            this._apiClient = apiClient;
            _liveStatusManager = liveStatusManager;
            SetTimer();
        }

        public async Task StartCommercial(ECommercialLength length = ECommercialLength.L30)
        {
            if (await _liveStatusManager.IsLive())
            {
                try
                {
                    await this._apiClient.StartCommercial(length);
                    Console.WriteLine($"Started commercial with length: {length}");
                }
                catch (ApiException)
                {
                    Console.WriteLine("Something went wrong with starting the commercial");
                }
            }
        }

        private void SetTimer()
        {
            this._commercialTimer = new Timer(_secondsBetweenCommercials * 1000)
            {
                AutoReset = true
            };
            this._commercialTimer.Elapsed += async (sender, args) => await StartCommercial(ECommercialLength.L30);
            this._commercialTimer.Start();
        }
    }

    public enum ECommercialLength
    {
        L30 = 30,
        L60 = 60,
        L90 = 90,
        L120 = 120,
        L150 = 150,
        L180 = 180
    }
}
