using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infrastructure.Models
{
    public class Server
    {
        [Key]
        public long Id { get; set; }
        public string Prefix {get; set;}
        public string Lang { get; set; }
    }
}
