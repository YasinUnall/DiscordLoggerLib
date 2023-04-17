using DiscordLoggerLib;
using DiscordLoggerLib.Models;

namespace portal_service.Services
{
    public interface ILogService
    {
        public void Log(string message, string channelName);
        public void Log(object message, string channelName, bool beautifyLog = true);
        public void Log(string message, DiscordDto.LogType type = DiscordDto.LogType.Info);
        public void Log(object message, DiscordDto.LogType type = DiscordDto.LogType.Info, bool beautifyLog = true);
        public void Log(string title, string message, DiscordDto.LogType type = DiscordDto.LogType.Info);
        public void Log(string title, object message, DiscordDto.LogType type = DiscordDto.LogType.Info, bool beautifyLog = true);
        public void Log(Exception exp, DiscordDto.LogType type = DiscordDto.LogType.Error, string pretext = "", string channelName = "", bool beautifyLog = true);

        public Task LogAsync(string message, string channelName);
        public Task LogAsync(object message, string channelName, bool beautifyLog = true);
        public Task LogAsync(string message, DiscordDto.LogType type = DiscordDto.LogType.Info);
        public Task LogAsync(object message, DiscordDto.LogType type = DiscordDto.LogType.Info, bool beautifyLog = true);
        public Task LogAsync(string title, string message, DiscordDto.LogType type = DiscordDto.LogType.Info);
        public Task LogAsync(string title, object message, DiscordDto.LogType type = DiscordDto.LogType.Info, bool beautifyLog = true);
        public Task LogAsync(Exception exp, DiscordDto.LogType type = DiscordDto.LogType.Error, string pretext = "", string channelName = "", bool beautifyLog = true);
    }

    public class LogService : ILogService
    {
        private DiscordLogger _logger;

        public LogService(DiscordLogger logger)
        {
            _logger = logger;
        }

        public void Log(string message, string channelName)
        {
            _logger.DiscordLogAsync(message, channelName).GetAwaiter().GetResult();
        }

        public void Log(object message, string channelName, bool beautifyLog = true)
        {
            _logger.DiscordLogAsync(message, channelName, beautifyLog).GetAwaiter().GetResult();
        }

        public void Log(string message, DiscordDto.LogType type = DiscordDto.LogType.Info)
        {
            _logger.DiscordLogAsync(message, type).GetAwaiter().GetResult();
        }

        public void Log(object message, DiscordDto.LogType type = DiscordDto.LogType.Info, bool beautifyLog = true)
        {
            _logger.DiscordLogAsync(message, type, beautifyLog).GetAwaiter().GetResult();
        }

        public void Log(string title, string message, DiscordDto.LogType type = DiscordDto.LogType.Info)
        {
            _logger.DiscordLogAsync(title, message, type).GetAwaiter().GetResult();
        }

        public void Log(string title, object message, DiscordDto.LogType type = DiscordDto.LogType.Info, bool beautifyLog = true)
        {
            _logger.DiscordLogAsync(title, message, type, beautifyLog).GetAwaiter().GetResult();
        }

        public void Log(Exception exp, DiscordDto.LogType type = DiscordDto.LogType.Error, string pretext = "", string channelName = "", bool beautifyLog = true)
        {
            _logger.DiscordLogAsync(exp, pretext, type, channelName, beautifyLog).GetAwaiter().GetResult();
        }

        public Task LogAsync(string message, string channelName)
        {
            return _logger.DiscordLogAsync(message, channelName);
        }

        public Task LogAsync(object message, string channelName, bool beautifyLog = true)
        {
            return _logger.DiscordLogAsync(message, channelName, beautifyLog);
        }

        public Task LogAsync(string message, DiscordDto.LogType type = DiscordDto.LogType.Info)
        {
            return _logger.DiscordLogAsync(message, type);
        }

        public Task LogAsync(object message, DiscordDto.LogType type = DiscordDto.LogType.Info, bool beautifyLog = true)
        {
            return _logger.DiscordLogAsync(message, type, beautifyLog);
        }

        public Task LogAsync(string title, string message, DiscordDto.LogType type = DiscordDto.LogType.Info)
        {
            return _logger.DiscordLogAsync(title, message, type);
        }

        public Task LogAsync(string title, object message, DiscordDto.LogType type = DiscordDto.LogType.Info, bool beautifyLog = true)
        {
            return _logger.DiscordLogAsync(title, message, type, beautifyLog);
        }

        public Task LogAsync(Exception exp, DiscordDto.LogType type = DiscordDto.LogType.Error, string pretext = "", string channelName = "", bool beautifyLog = true)
        {
            return _logger.DiscordLogAsync(exp, pretext, type, channelName, beautifyLog);
        }
    }
}
