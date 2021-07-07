using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Discord.Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Template.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<CommandModule> _logger;
        private readonly Servers _servers;
        private readonly Questions _questions;
        private readonly IConfiguration _config;
        public CommandModule(ILogger<CommandModule> logger, Servers servers, IConfiguration config, Questions questions)
        {
            _logger = logger;
            _servers = servers;
            _config = config;
            _questions = questions;
        }


        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
        }

        //[Command("echo")]
        //public async Task EchoAsync([Remainder] string text)
        //{
        //    await ReplyAsync(text);
        //}

        //[Command("math")]
        //public async Task MathAsync([Remainder] string math)
        //{
        //    var dt = new DataTable();
        //    var result = dt.Compute(math, null);

        //    await ReplyAsync($"Result: {result}");
        //}

        [Command("prefix")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task Prefix(string prefix = null)
        {
            if (prefix == null)
            {
                var guildPrefix = await _servers.GetGuildPrefix(Context.Guild.Id) ?? _config["prefix"];
                await ReplyAsync($"The current prefix of this bot is `{guildPrefix}`.");
                return;
            }

            if (prefix.Length > 4) {
                await ReplyAsync("The length of the new prefix is too long! \n Must be less than or equal to 4 characters.");
                return;
            }
            await _servers.ModifyGuildPrefix(Context.Guild.Id, prefix);
            await ReplyAsync($"The prefix has been changed to `{prefix}`.");
        }

        [Command("questions")]
        [Summary("Returns a question for a user.")]
        [Alias("q", "qs" , "كت")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task Questions(string lang = null)
        {
            List<string> languages = new List<string>(){ "ar", "en" };
            if (lang == null)
            {
                lang = "ar";
            }
            else {
                if (languages.Contains(lang.ToLower()) == false) {
                    await ReplyAsync($"The value for the language parameter you selected is invalid , valid options include (ar , en).");
                    return;
                }
            }
            var question = await _questions.GetQuestion(Context.Guild.Id , "ar");
            await ReplyAsync($"`{question}`.");
            return;
        }

    }
}