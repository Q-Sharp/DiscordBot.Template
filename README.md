# DiscordBot.Template

[![.NET](https://github.com/Q-Sharp/DiscordBot.Template/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Q-Sharp/DiscordBot.Template/actions/workflows/dotnet.yml)

## Features

- NET 7
- DISCORD.NET

## Using the template

### install

```
dotnet new -i DiscordBot.Template
```

### run

```
dotnet new discordbot -n YourBotName
```

Use the `-n` or `--name` parameter to change the name of the output created. This string is also used to substitute the namespace name in the .cs file for the project.

## Setup after installation

Configure your bot settings:

```
 "Discord": {
    "DevToken": "<ENTER YOUR TOKEN>",
    "Token": "<ENTER YOUR TOKEN>",

    "OwnerId": <ENTER YOUR ID HERE>,
    "GameActivity": "Template Writer 2022",

    "Settings": {
      "CommandPrefix": "!", 
      "CaseSensitiveCommands": false,
      "DefaultRunMode": "Async",
      "SeparatorChar": " ",
      "LogLevel": "Debug",
      "MessageCacheSize": 1000
    }

```

Consider to use an UserSecret for development to not push your token to github.

### uninstall

```
dotnet new -u DiscordBot.Template
```

## Development

### build

https://docs.microsoft.com/en-us/dotnet/core/tutorials/create-custom-template

```
dotnet pack -o ./publish --no-build
```

### install developement

Locally built nupkg:

```
dotnet new -i DiscordBot.Template.*.*.*.nupkg
```

Local folder:

```
dotnet new -i <PATH>
```

Where `<PATH>` is the path to the folder containing .template.config.


## Credits, Used NuGet packages + ASP.NET Core 7.0 standard packages

- Discord.Net
- Serilog 

## Links

https://discordnet.dev/

https://serilog.net/
