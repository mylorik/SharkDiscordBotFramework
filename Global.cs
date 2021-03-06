﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord.WebSocket;
using King_of_the_Garbage_Hill.Game.Classes;

namespace SharkFramework
{
    public sealed class Global : IServiceSingleton
    {
        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Global(DiscordShardedClient client)
        {
            Client = client;
            TimeBotStarted = DateTime.Now;
            GamePlayingAndId = 0;
            TotalCommandsIssued = 0;
            TotalCommandsDeleted = 0;
            TotalCommandsChanged = 0;
            TimeSpendOnLastMessage = new ConcurrentDictionary<ulong, Stopwatch>();
            GamesList = new List<GameClass>();
            
        }

        public readonly DiscordShardedClient Client;
        public readonly DateTime TimeBotStarted;
        public uint TotalCommandsIssued;
        public uint TotalCommandsDeleted;
        public uint TotalCommandsChanged;
        public ConcurrentDictionary<ulong, Stopwatch> TimeSpendOnLastMessage;
        //add more global variable here

    }
}
