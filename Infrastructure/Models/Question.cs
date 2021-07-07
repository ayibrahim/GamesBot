using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infrastructure.Models
{
    public class Question
    {
        [Key]
        public long QuestionsID { get; set; }
        public string Lang { get; set; }
        public string strQuestion { get; set; }

    }
}
