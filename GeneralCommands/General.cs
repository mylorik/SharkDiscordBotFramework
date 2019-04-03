using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SharkFramework.BotFramework.Extensions;
using SharkFramework.LocalPersistentData.UsersAccounts;
using Newtonsoft.Json;

namespace SharkFramework.GeneralCommands
{
    public class General : ModuleBaseCustom
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.


        private readonly UserAccounts _accounts;
        private readonly CommandsInMemory _commandsInMemory;
        private readonly Global _global;
      

        public General(UserAccounts accounts, CommandsInMemory commandsInMemory, Global global)
        {
            _accounts = accounts;
            _commandsInMemory = commandsInMemory;
            _global = global;
        }

        [Command("runtime")]
        [Alias("uptime")]
        [Summary("Bot Sktatistics")]
        public async Task UpTime()
        {
            _global.TimeSpendOnLastMessage.TryGetValue(Context.User.Id, out var watch);

            var time = DateTime.Now - _global.TimeBotStarted;
            var ramUsage = Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024);

            var embed = new EmbedBuilder()
                .WithColor(Color.DarkGreen)
                .WithCurrentTimestamp()
                .WithFooter("Version: 0.0a | Thank you for using me!")
                .AddField("Numbers:", "" +
                                      $"Working for: {time.Days}d {time.Hours}h {time.Minutes}m and {time:ss\\.fff}s\n" +
                                      $"Total Games Started: {_global.GetLastGamePlayingAndId()}\n" +
                                      $"Total Commands issued while running: {_global.TotalCommandsIssued}\n" +
                                      $"Total Commands changed: {_global.TotalCommandsChanged}\n" +
                                      $"Total Commands deleted: {_global.TotalCommandsDeleted}\n" +
                                      $"Total Commands in memory: {_commandsInMemory.CommandList.Count} (max {_commandsInMemory.MaximumCommandsInRam})\n" +
                                      $"Client Latency: {_global.Client.Latency}\n" +
                                      $"Time I spend processing your command: {watch?.Elapsed:m\\:ss\\.ffff}s\n" +
                                      "This time counts from from the moment he receives this command.\n" +
                                      $"Memory Used: {ramUsage}");


            SendMessAsync(embed);

        }

        [Command("myPrefix")]
        [Summary("Will set you own prefix for this bot")]
        public async Task SetMyPrefix([Remainder] string prefix = null)
        {
            var account = _accounts.GetAccount(Context.User);

            if (prefix == null)
            {
                var myAccountPrefix = account.MyPrefix ?? "You don't have own prefix yet";

                await SendMessAsync(
                    $"Your prefix: **{myAccountPrefix}**");
                return;
            }

            if (prefix.Length < 100)
            {
                account.MyPrefix = prefix;
                if (prefix.Contains("everyone") || prefix.Contains("here"))
                {
                    await SendMessAsync(
                        "No `here` or `everyone` prefix allowed.");
                    return;
                }

                _accounts.SaveAccounts(Context.User);
                await SendMessAsync(
                    $"Your own prefix is now **{prefix}**");
            }
            else
            {
                await SendMessAsync(
                    "Prefix have to be less than 100 characters");
            }
        }


       


        [Command("updMaxRam")]
        [RequireOwner]
        [Summary(
            "updates maximum number of commands bot will save in memory (default 1000 every time you launch this app)")]
        public async Task ChangeMaxNumberOfCommandsInRam(uint number)
        {
            _commandsInMemory.MaximumCommandsInRam = number;
            SendMessAsync($"now I will store {number} of commands");
        }

        [Command("clearMaxRam")]
        [RequireOwner]
        [Summary("CAREFUL! This will delete ALL commands in ram")]
        public async Task ClearCommandsInRam()
        {
            var toBeDeleted = _commandsInMemory.CommandList.Count;
            _commandsInMemory.CommandList.Clear();
            SendMessAsync($"I have deleted {toBeDeleted} commands");
        }
    }
}
