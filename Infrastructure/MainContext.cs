using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure
{
    public class MainContext : DbContext
    {
        public DbSet<Server> Servers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ServerQuestions> serverQuestions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
           => options.UseSqlServer("Server=localhost;Database=GamesBot;Trusted_Connection=True;");

    }
}
