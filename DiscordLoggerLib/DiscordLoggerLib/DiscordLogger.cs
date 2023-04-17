using DiscordLoggerLib;
using System.Text.Json;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using static DiscordLoggerLib.Models.DiscordDto;
using System.Threading.Tasks;
using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DiscordLoggerLib
{
    public interface IDiscordLogger
    {
        public Task DiscordLogAsync(string message, string channelName);
        public Task DiscordLogAsync(object message, string channelName, bool beautifyLog = true);
        public Task DiscordLogAsync(string message, LogType type = LogType.Info);
        public Task DiscordLogAsync(object message, LogType type = LogType.Info, bool beautifyLog = true);
        public Task DiscordLogAsync(string title, string message, LogType type = LogType.Info);
        public Task DiscordLogAsync(string title, object message, LogType type = LogType.Info,  bool beautifyLog = true);
        public Task DiscordLogAsync(Exception exp, string pretext = "", LogType type = LogType.Error, string channel = "", bool beautify = true);
    }

    public class DiscordLogger : IDiscordLogger
    {
        private readonly ConfigModel _config;
        private DiscordMain _discord;

        public DiscordLogger(ConfigModel config)
        {
            _config = config;
            DiscordLoggerLibInit();
        }

        private void DiscordLoggerLibInit()
        {
            var serviceProvider = new ServiceCollection()
                        .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                        {
                            AlwaysDownloadUsers = true,
                            GatewayIntents = Discord.GatewayIntents.AllUnprivileged,
                        }))
                        .AddSingleton(x =>
                            new DiscordMain(x.GetRequiredService<DiscordSocketClient>(), _config)
                        )
                        .BuildServiceProvider();

            var client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            _discord = serviceProvider.GetRequiredService<DiscordMain>();
        }

        public async Task DiscordLogAsync(string message, string channelName)
        {
            await SendLogs(message, channelName);
        }

        public async Task DiscordLogAsync(object message, string channelName, bool beautifyLog = true)
        {
            string formattedMessage = JsonConvert.SerializeObject(message, Formatting.Indented);
            await SendLogs(formattedMessage, channelName);
        }

        public async Task DiscordLogAsync(string message, LogType type = LogType.Info)
        {
            await SendLogs(message, type.ToString());
        }

        public async Task DiscordLogAsync(object message, LogType type = LogType.Info, bool beautifyLog = true)
        {
            string formattedMessage = JsonConvert.SerializeObject(message, Formatting.Indented);
            await SendLogs(formattedMessage, type.ToString());
        }

        public async Task DiscordLogAsync(string title, string message, LogType type = LogType.Info)
        {
            message = $"{title}\n" + message;
            await SendLogs(message, type.ToString());
        }

        public async Task DiscordLogAsync(string title, object message, LogType type = LogType.Info, bool beautifyLog = true)
        {
            Formatting formatting = beautifyLog ? Formatting.Indented : Formatting.None;
            string formattedMessage = JsonConvert.SerializeObject(message, formatting);
            formattedMessage = $"{title}\n" + formattedMessage;
            await SendLogs(formattedMessage, type.ToString());
        }

        public async Task DiscordLogAsync(Exception exp, string pretext = "", LogType type = LogType.Error, string channel = "", bool beautifyLog = true)
        {
            Formatting formatting = beautifyLog ? Formatting.Indented : Formatting.None;
            string formattedMessage = JsonConvert.SerializeObject(exp, formatting);
            formattedMessage = $"{pretext} - " + formattedMessage;
            string channelToLog = string.IsNullOrEmpty(channel) ? type.ToString() : channel;

            await SendLogs(formattedMessage, channelToLog);
        }

        private async Task SendLogs(string message, string channelName, string categoryName = "")
        {
            if (message.Length > 2000)
            {
                message = message.Replace("\\\\", "\\").Replace("\\n", "\n").Replace("\\r", "\r");
                List<string> splitedMessages = await SplitAccordingToDcCharLimit(message);
                lock (_discord)
                {
                    foreach (string logMessage in splitedMessages)
                    {
                        _discord.DiscordLog(logMessage, channelName, categoryName).GetAwaiter().GetResult();
                    }
                }
            }
            else _discord.DiscordLog(message, channelName, categoryName).GetAwaiter().GetResult();
        }

        private async Task<List<string>> SplitAccordingToDcCharLimit(string message)
        {
            List<string> lines = message.Split("\n").ToList();
            List<string> messages = new List<string> { "" };
            int lastMessageInd = 0;
            //int length = 0;
            foreach (var line in lines)
            {
                //int lineLength = JsonConvert.SerializeObject(line).Length;
                if ((messages[lastMessageInd].Length + line.Length) > 2000)
                {
                    messages.Add(line + "\n");
                    lastMessageInd++;
                    //length = 0;
                }
                else messages[lastMessageInd] += line + "\n";
                //length += lineLength;
            }

            return messages;
        }
    }
}