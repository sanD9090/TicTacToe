using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TicTacToe.Models;

namespace TicTacToe.Data
{
    

    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
       : base(options)
        {
        }
        public DbSet<TicTacToeGame> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=tictactoe.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicTacToeGame>()
                .Property(g => g.Board)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<string>(v));
        }
    }

}
