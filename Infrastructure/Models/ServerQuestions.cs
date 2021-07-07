using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infrastructure.Models
{
    public class ServerQuestions
    {
        [Key]
        public long ServerQuestionID { get; set; }
        public long ServerId { get; set; }
        public long QuestionId { get; set; }
    }
}
