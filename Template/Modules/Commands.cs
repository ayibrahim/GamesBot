using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Discord.Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Discord;

namespace Template.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<Commands> _logger;
        private readonly Servers _servers;
        private readonly Questions _questions;
        private readonly IConfiguration _config;
        public Commands(ILogger<Commands> logger, Servers servers, IConfiguration config, Questions questions)
        {
            _logger = logger;
            _servers = servers;
            _config = config;
            _questions = questions;
        }



        [Command("ping")]
        [Summary("Checks bot reply time.")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
        }

        [Command("prefix")]
        [Summary("Returns prefix of guild and lets you change your prefix.")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task Prefix(string prefix = null)
        {
            if (prefix == null)
            {
                var guildPrefix = await _servers.GetGuildPrefix((long)Context.Guild.Id) ?? _config["prefix"];
                await ReplyAsync($"The current prefix of this bot is `{guildPrefix}`.");
                return;
            }

            if (prefix.Length > 4) {
                await ReplyAsync("The length of the new prefix is too long! \n Must be less than or equal to 4 characters.");
                return;
            }
            await _servers.ModifyGuildPrefix((long)Context.Guild.Id, prefix);
            await ReplyAsync($"The prefix has been changed to `{prefix}`.");
        }

        [Command("lang")]
        [Summary("Returns language of guild and lets you change language (ar , en).")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task Language(string lang = null)
        {
            List<string> languages = new List<string>() { "ar", "en" };
            if (lang == null)
            {
                var guildLang = await _servers.GetGuildLang((long)Context.Guild.Id) ?? "ar";
                await ReplyAsync($"The current Language of this bot is `{guildLang}`.");
                return;
            }
            if (languages.Contains(lang.ToLower()) == false)
            {
                await ReplyAsync($"The value for the language parameter you selected is invalid , valid options include (ar , en).");
                return;
            }
            await _servers.ModifyGuildLang((long)Context.Guild.Id, lang);
            await ReplyAsync($"The Language has been changed to `{lang}`.");
        }

        [Command("questions")]
        [Summary("Returns a question")]
        [Alias("q", "qs", "كت")]
        public async Task Questions(string lang = null)
        {
            List<string> languages = new List<string>() { "ar", "en" };
            if (lang == null)
            {
                lang = await _servers.GetGuildLang((long)Context.Guild.Id) ?? "ar";
            }
            else {
                if (languages.Contains(lang.ToLower()) == false) {
                    await ReplyAsync($"The value for the language parameter you selected is invalid , valid options include (ar , en).");
                    return;
                }
            }
            var question = await _questions.GetQuestion(Context.Guild.Id, lang);

            var builder = new EmbedBuilder()
            {
                //Optional color
                Color = Discord.Color.Red,
                Description = "**Question**"
            };
            if (lang == "ar")
            {
                builder.AddField(x =>
                {
                    x.Name = question + " ؟ \n";
                    x.Value = Context.Guild.Name;
                    x.IsInline = false;
                });
            }
            else if (lang == "en") {
                builder.AddField(x =>
                {
                    x.Name = question + " ?\n";
                    x.Value = Context.Guild.Name;
                    x.IsInline = false;
                });
            }
            
            await ReplyAsync("", false, builder.Build());
            return;
        }


    }
}