# DiscordLoggerLib
This is a library that used for writing logs to discord channels.

## Prerequisites
- A Discord bot. You can create it from discord panel.
- A Discord server for logs to send.

## Dependencies
- .Net Core 3.1 or greater versions.

Following packages will be installed automatically by NuGet: 

- Discord.Net >= 3.8.0
- Newtonsoft.Json >= 13.0.3
- Microsoft.Extensions.DependencyInjection >= 6.0.0

## Features
- You can send messages that has more than 2000 characters. In this case message will be splitted and it will be sended to discord as separate messages.
- Your logs will be formatted/beautified so you can see it better.
- If there is no category or channel in the discord server, it will be created automatically and then send the log to that category channel.


## Implementation Example

You need to create an instance of discord logger. It will be added to Startup.cs or Program.cs file for .Net depending on the version of the framework.

```csharp
services.AddSingleton(x => new DiscordLogger(new ConfigModel
{
    Token = "", // Your Discord bot token.
    ServerId = "", // Server's Id
    DefaultCategoryName = "" // Category name that will be used when no category name provided.
}));
````

If you are using the example LogService you may want to add below part too for dependency injection.

```csharp
services.AddSingleton<ILogService, LogService>();
````
