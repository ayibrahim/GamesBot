using Discord;
using Discord.Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Modules;

namespace GamesBot.Services
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly ILogger<Help> _logger;
        private readonly IConfiguration _config;
        private readonly Servers _servers;

        public Help(CommandService service , ILogger<Help> logger , IConfiguration config , Servers servers)
        {
            _logger = logger;
            _service = service;
            _config = config;
            _servers = servers;
        }

        [Command("help")]
        [Summary("Returns a list of all commands.")]
        public async Task HelpAsync()
        {
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "These are the commands you can use"
            };

            foreach (var module in _service.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    var prefix = await _servers.GetGuildPrefix((long)Context.Guild.Id)?? _config["prefix"];
                    if (result.IsSuccess)
                        description += $"{prefix}{cmd.Aliases.First()} - `{cmd.Summary}`\n";
                }

                if (!string.IsNullOrWhiteSpace(description) && module.Name != "Admin")
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

    }
}
