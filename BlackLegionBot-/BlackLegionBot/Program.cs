using System;
using System.Text.Json;
using BlackLegionBot.CommandHandling;
using BlackLegionBot.CommandStorage;
using BlackLegionBot.Credentials;
using BlackLegionBot.NonCommandBased;
using BlackLegionBot.TwitchApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;

namespace BlackLegionBot
{
    static class Program
    {
        static void Main(string[] args) =>

            CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", true, true)
                        .Build();

                    // configs
                    var blbApiConfig = new BlbApiConfig(config.GetSection("BlbApi"));
                    services.AddSingleton<BlbApiConfig>(blbApiConfig);
                    var ircCredentials = new IrcCredentials(config.GetSection("Irc"));
                    services.AddSingleton(ircCredentials);
                    var userInfo = new UserInfo(config.GetSection("UserConfig"));
                    services.AddSingleton(userInfo);

                    var refitSettings = new RefitSettings()
                    {
                        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions()
                        {
                            PropertyNameCaseInsensitive = true
                        })
                    };

                    services.AddRefitClient<ITwitchApiManager>(refitSettings).ConfigureHttpClient(httpClient =>
                        httpClient.BaseAddress = new Uri("https://api.twitch.tv"));

                    services.AddRefitClient<ITwitchAuthApi>(refitSettings)
                        .ConfigureHttpClient(httpClient => 
                            httpClient.BaseAddress = new Uri("https://id.twitch.tv/oauth2"));

                    services.AddRefitClient<IBlbApi>(refitSettings).ConfigureHttpClient(httpClient =>
                    {
                        httpClient.BaseAddress = new Uri(blbApiConfig.Url);
                        httpClient.DefaultRequestHeaders.Add("X-Access-Token", blbApiConfig.JWT);
                    });

                    services.AddSingleton<CommercialManager>();
                    services.AddSingleton<CooldownManager>();
                    services.AddSingleton<TwitchAuthManager>();
                    services.AddSingleton<TwitchApiManager>();
                    services.AddSingleton<BlbApiHandler>();
                    services.AddSingleton<ICommandRetriever, CommandCache>();
                    services.AddHostedService<Bot>();
                });
    }
}
