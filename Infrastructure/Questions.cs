using Infrastructure.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Questions
    {
        private readonly MainContext _context;
        private readonly Random random = new Random();
        private readonly IConfiguration _config;
        public Questions(MainContext context , IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> GetQuestion(ulong id , string lang)
        {
            string output = "";
            long selectedid = 0;
            List<Question> questionsList = getQuestionsPerServerLang(id, lang);
            if (questionsList == null || questionsList.Count <= 0) {
                deleteQuestionsForServer(id);
                questionsList = getQuestionsPerServerLang(id, lang);
                if (questionsList != null || questionsList.Count > 0)
                {
                    int num = random.Next(questionsList.Count);
                    output = questionsList[num].strQuestion;
                    selectedid = questionsList[num].QuestionsID;
                }
            } else {
                int num = random.Next(questionsList.Count);
                output = questionsList[num].strQuestion;
                selectedid = questionsList[num].QuestionsID;
            }
            if (output.Length == 0) {
                output = "Looks like there was an issue retrieving a question , please try again and if this keeps happening please contact " + _config["developer"];
            }
            if (selectedid != 0) {
                insertQuestionAsked(id , selectedid);
            }
            return await Task.FromResult(output);
        }

        private void deleteQuestionsForServer(ulong id) {
            var serverquestionlist = _context.serverQuestions.Where(x => x.ServerId == Convert.ToInt64(id));
            _context.serverQuestions.RemoveRange(serverquestionlist);
            _context.SaveChanges();
        }
        
        private List<Question> getQuestionsPerServerLang(ulong id, string lang) {
            List<Question> questionsList = new List<Question>();
            using (var ctx = new MainContext())
            {
                var parameters = new[] {
                    new SqlParameter("@ServerID", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = id },
                    new SqlParameter("@Lang", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = lang }
                };
                questionsList = ctx.Questions.FromSqlRaw("EXECUTE dbo.GetServerQuestion @ServerID , @Lang", parameters).ToList();

            }
            return questionsList;
        }

        private void insertQuestionAsked(ulong id, long questionId)
        {
            using (var context = new MainContext())
            {
                var commandText = "Insert INTO ServerQuestions(ServerId,QuestionId) Values(@ServerID, @QuestionID)";
                context.Database.ExecuteSqlRaw(commandText, new SqlParameter("ServerID", id.ToString()), new SqlParameter("QuestionID", questionId.ToString()));
            }
        }

        public async Task<bool> checkQuestionExists(string Question , string Lang)
        {
            var questionID = await _context.Questions.Where(x => x.Lang == Lang).Where(x => x.strQuestion == Question).Select(x => x.QuestionsID).FirstOrDefaultAsync();
            if (questionID == 0)
            {
                return await Task.FromResult(false);
            }
            else {
                return await Task.FromResult(true);
            }
           
        }
        public async Task insertNewQuestion(string strQuestion , string Lang)
        {
            using (var context = new MainContext())
            {
                var commandText = "Insert INTO Questions(strQuestion,Lang) Values(@strQuestion, @Lang)";
                context.Database.ExecuteSqlRaw(commandText, new SqlParameter("strQuestion", strQuestion), new SqlParameter("Lang", Lang));
            }
        }
    }
}
