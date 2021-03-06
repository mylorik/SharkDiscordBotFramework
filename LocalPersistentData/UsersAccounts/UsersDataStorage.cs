﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using SharkFramework.BotFramework;

using Newtonsoft.Json;

namespace SharkFramework.LocalPersistentData.UsersAccounts
{
       public sealed class UserAccountsDataStorage : IServiceSingleton
    {


        private readonly LoginFromConsole _log;

        public UserAccountsDataStorage(LoginFromConsole log)
        {
            _log = log;
        }

        public async Task InitializeAsync()
            => await Task.CompletedTask;


        public void SaveAccountSettings(DiscordAccountClass accounts, string idString, string json)
        {
            var filePath = $@"DataBase/OctoDataBase/UserAccounts/discordAccount-{idString}.json";
            try
            {
                File.WriteAllText(filePath, json);
            }
            catch(Exception e)
            {
                _log.Critical($"Save USER DiscordAccountClass (3 params): {e.Message}");
              
            }
        }


        public void SaveAccountSettings(DiscordAccountClass accounts, ulong userId)
        {
            var filePath = $@"DataBase/OctoDataBase/UserAccounts/discordAccount-{userId}.json";
            try
            {
                var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                _log.Critical($"Save USER DiscordAccountClass (2 params): {e.Message}");

            }
        }


        public ConcurrentDictionary<ulong, DiscordAccountClass>LoadAllAccounts()
        {
            var dick = new ConcurrentDictionary<ulong, DiscordAccountClass>();
            var filePaths = Directory.GetFiles(@"DataBase/OctoDataBase/UserAccounts");

            foreach (var file in filePaths)
            {
                var json = File.ReadAllText(file);

                var id = Convert.ToUInt64(file.Split("-")[1].Split(".")[0]);

                if(id == 0) continue;

                try
                {
                    var acc = JsonConvert.DeserializeObject<DiscordAccountClass>(json);
                    dick.GetOrAdd(id,acc);
                }
                catch (Exception e)
                {
                    _log.Critical($"LoadAccountSettings, BACK UP CREATED: {e}");
            
                    var newList =new DiscordAccountClass();
                    SaveAccountSettings(newList, $"{id}-BACK_UP", json);
                    dick.GetOrAdd(id, x => newList);
                }
            }
            return dick;
        }
    }
}
