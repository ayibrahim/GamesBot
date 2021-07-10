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

        [Command("prefix")]
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

        [Command("questions")]
        [Summary("Returns a question for a user.")]
        [Alias("q", "qs", "كت")]
        public async Task Questions(string lang = null)
        {
            List<string> languages = new List<string>() { "ar", "en" };
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


        [Command("newQuestion")]
        [Summary("Add new question to DB.")]
        [Alias("nq")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task AddQuestion(string Lang = null, [Remainder]string strQuestion = null)
        {
            if (Context.Guild.Id == 481566772206632970) {
                List<string> languages = new List<string>() { "ar", "en" };
                if (Lang == null)
                {
                    await ReplyAsync($"Please specify Language for the question you are adding.");
                    return;
                }
                else if (strQuestion == null)
                {
                    await ReplyAsync($"Please specify your Quesiton after the language.");
                    return;
                }
                else
                {
                    if (languages.Contains(Lang.ToLower()) == false)
                    {
                        await ReplyAsync($"The value for the language parameter you selected is invalid , valid options include (ar , en).");
                        return;
                    }
                }
                bool isExist = await _questions.checkQuestionExists(strQuestion, Lang);
                if (isExist == true)
                {
                    await ReplyAsync($"The question you are inserting already exists for the following language , Please try again.");
                    return;
                }
                else
                {
                    await _questions.insertNewQuestion(strQuestion, Lang);
                    await ReplyAsync($"Your Question has been added.");
                    return;
                }
            }
        }
    }
}