using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using DiscordLoggerLib.Models;
using Discord.Interactions;
using static DiscordLoggerLib.Models.DiscordDto;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DiscordLoggerLib
{
    internal class DiscordMain
    {
        protected DiscordSocketClient _client;
        private readonly UInt64 _serverId;
        private readonly string _token;
        private readonly string _defaultCategoryName;
        private readonly int _retryCountOnFail = 20;
        private TaskCompletionSource<bool> _readySignal;

        public DiscordMain(DiscordSocketClient client, ConfigModel config)
        {
            _token = config.Token;
            _client = client;
            _serverId = string.IsNullOrEmpty(config.ServerId) ? 0 : Convert.ToUInt64(config.ServerId);
            _defaultCategoryName = config.DefaultCategoryName;
            _retryCountOnFail = config.RetryCountOnFail;

            StartProcess().GetAwaiter().GetResult();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task StartProcess()
        {
            _client.Log += Log;
            _client.Ready += OnClientReady;

            await LoginToBotAsync();
            _readySignal = new TaskCompletionSource<bool>();

            await StartBotAsync();
            await _readySignal.Task; // Waiting for client to finish all of it's configurations
        }

        private async Task LoginToBotAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _token);
        }

        private async Task StartBotAsync()
        {
            await _client.StartAsync();
        }

        private async Task OnClientReady()
        {
            Console.WriteLine("Bot connected!");
            _readySignal.SetResult(true);
        }

        internal async Task DiscordLog(string message, string channelName, string categoryName = "")
        {
            while (_client.ConnectionState != ConnectionState.Connected) { await Task.Delay(1000); }

            SocketGuild guild = _client.GetGuild(_serverId);

            int retryCount = 0;
            while ((!guild.CategoryChannels.Any() || !guild.TextChannels.Any()) && retryCount < _retryCountOnFail)
            {
                await StartProcess();
                await Task.Delay(500);
                guild = _client.GetGuild(_serverId);
                retryCount++;
            }

            SocketGuildChannel? channel = null;
            if (categoryName == "")
                categoryName = _defaultCategoryName;

            var category = guild.CategoryChannels.FirstOrDefault(f => string.Compare(f.Name.Trim(), categoryName.Trim(), StringComparison.OrdinalIgnoreCase) == 0);
            if (category == null) // there is no category with the name of 'categoryName'
            {
                ulong categoryId = (await guild.CreateCategoryChannelAsync(categoryName)).Id;
                category = guild.GetCategoryChannel(categoryId);
            }

            channel = category.Channels.FirstOrDefault(f => string.Compare(f.Name.Trim(), channelName.Trim(), StringComparison.OrdinalIgnoreCase) == 0);

            if (channel == null) // there is no channel with the name of 'channelName'
            {
                // create the channel
                //Discord.Rest.RestTextChannel? newChannel = await guild.CreateTextChannelAsync(channelName, options => options.CategoryId = category.Id);
                Discord.Rest.RestTextChannel? newChannel = await guild.CreateTextChannelAsync(channelName, options => options.CategoryId = category.Id);
                channel = guild.GetChannel(newChannel.Id);
            }

            IMessageChannel? messageChannel = channel as IMessageChannel;
            if (messageChannel != null) await messageChannel.SendMessageAsync(message);
        }
    }
}
}
