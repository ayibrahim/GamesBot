using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Template.Services
{
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _config;
        private readonly Servers _servers;
        private readonly ILogger<CommandHandler> _logger;
        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration config , Servers servers , ILogger<CommandHandler> logger)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _config = config;
            _servers = servers;
            _logger = logger;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += OnMessageReceived;
           // _client.ChannelCreated += OnChannelCreated;
            _client.JoinedGuild += OnBotJoinedGuild;
            _client.LeftGuild += OnBotLeftGuild;
            _service.CommandExecuted += OnCommandExecuted;
            _client.Ready += StatusAsync;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        public async Task StatusAsync()
        {
            var status = $"Listening To ##help";
            await _client.SetGameAsync(status, "https://www.twitch.tv/", ActivityType.Streaming);
        }

        private async Task OnBotLeftGuild(SocketGuild arg)
        {
            await _servers.RemoveGuild((long)(arg.Id));
            //Bot Left server
        }

        private async Task OnBotJoinedGuild(SocketGuild arg)
        {
            await _servers.AddNewGuild((long)(arg.Id) , _config["prefix"]);
            //Bot Joined server
        }

       // private async Task OnChannelCreated(SocketChannel arg)
        //{
            //if ((arg as ITextChannel) == null) return;
            //var channel = arg as ITextChannel;
            //await channel.SendMessageAsync("The event was called!");
        //}

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            var prefix = await _servers.GetGuildPrefix((long)(message.Channel as SocketGuildChannel).Guild.Id) ?? _config["prefix"];
            if (!message.HasStringPrefix(prefix, ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _service.ExecuteAsync(context, argPos, _provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess) await context.Channel.SendMessageAsync($"Error: {result}");
        }

       
    }
}