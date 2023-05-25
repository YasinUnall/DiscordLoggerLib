using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordLoggerLib.Models
{
    public class DiscordDto
    {
        public class ConfigModel
        {
            public string Token { get; set; } = string.Empty;
            public string ServerId { get; set; } = string.Empty;
            public string DefaultCategoryName { get; set; } = string.Empty;
            public int RetryCountOnFail { get; set; } = 20;
        }

        public enum LogType
        {
            Info = 1,
            Warning = 2,
            Error = 3,
            Fatal = 4,
        }
    }
}
