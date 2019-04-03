using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using King_of_the_Garbage_Hill.BotFramework.Extensions;
using Lamar;
using Microsoft.Extensions.DependencyInjection;

namespace SharkFramework
{
    public class ProgramSharkFramework
    {
        private readonly int[] _shardIds = {0};// have to increment by 1 starting from 0. 1 shard for 1000 servers | if you have 2856 servers you need fill it like this {0, 1, 2}
        private DiscordShardedClient _client;
        private Container _services;

        private static void Main()
        {
            new ProgramSharkFramework().RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {
            _client = new DiscordShardedClient(_shardIds, new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRetryMode = RetryMode.AlwaysRetry,
                MessageCacheSize = 50,
                TotalShards = 1 // have to be the same number as the length of _shards
            });

            _services = new Container(x =>
            {
                x.AddSingleton(_client)
                    .AddSingleton<CancellationTokenSource>()
                    .AddSingleton<CommandService>()
                    .AddSingleton<HttpClient>()
                    .AutoAddSingleton()
                    .AutoAddTransient()
                    .BuildServiceProvider();
            });

            await _services.InitializeServicesAsync();

            await _client.SetGameAsync("Playing something");

            await _client.LoginAsync(TokenType.Bot, _services.GetRequiredService<Config>().Token);
            await _client.StartAsync();


            try
            {
                await Task.Delay(-1, _services.GetRequiredService<CancellationTokenSource>().Token);
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
