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

namespace GamesBot.Modules
{
    public class Admin : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<Admin> _logger;
        private readonly Servers _servers;
        private readonly Questions _questions;
        private readonly IConfiguration _config;
        public Admin(ILogger<Admin> logger, Servers servers, IConfiguration config, Questions questions)
        {
            _logger = logger;
            _servers = servers;
            _config = config;
            _questions = questions;
        }


        [Command("newQuestion")]
        [Summary("Add new question to DB.")]
        [Alias("nq")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task AddQuestion(string Lang = null, [Remainder] string strQuestion = null)
        {
            if (Context.Guild.Id == 481566772206632970)
            {
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
