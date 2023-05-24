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

        public DiscordMain(DiscordSocketClient client, ConfigModel config)
        {
            _token = config.Token;
            _client = client;
            _serverId = string.IsNullOrEmpty(config.ServerId) ? 0 : Convert.ToUInt64(config.ServerId);
            _defaultCategoryName = config.DefaultCategoryName;

            StartProcess().Wait();
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
            await StartBotAsync();

            //await Task.Delay(-1);
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
        }

        internal async Task DiscordLog(string message, string channelName, string categoryName = "")
        {
            while (_client.ConnectionState != ConnectionState.Connected) { await Task.Delay(1000); }

            SocketGuild guild = _client.GetGuild(_serverId);
            SocketGuildChannel? channel = null;
            if (categoryName == "")
                categoryName = _defaultCategoryName;

            CultureInfo culture = new CultureInfo("en-GB");

            var category = guild.CategoryChannels.FirstOrDefault(f => f.Name.Trim().ToLower(culture) == categoryName.Trim().ToLower(culture));
            if (category == null) // there is no category with the name of 'categoryName'
            {
                ulong categoryId = (await guild.CreateCategoryChannelAsync(categoryName)).Id;
                category = guild.GetCategoryChannel(categoryId);
            }

            channel = category.Channels.FirstOrDefault(f => f.Name.Trim().ToLower(culture) == channelName.Trim().ToLower(culture));

            if (channel == null) // there is no channel with the name of 'channelName'
            {
                // create the channel
                Discord.Rest.RestTextChannel? newChannel = await guild.CreateTextChannelAsync(channelName, options => options.CategoryId = category.Id);
                channel = guild.GetChannel(newChannel.Id);
            }

            IMessageChannel? messageChannel = channel as IMessageChannel;
            if (messageChannel != null) await messageChannel.SendMessageAsync(message);
        }
    }
}
