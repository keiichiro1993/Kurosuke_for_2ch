using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Sqlite;

namespace Kurosuke_for_2ch.Models
{
    public class ThreadContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ita> Itas { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new SqliteConnectionStringBuilder { DataSource = "threads.db" }.ToString();
            optionsBuilder.UseSqlite(new SqliteConnection(connectionString));
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Make Blog.Url required
            modelBuilder.Entity<Category>()
                .Property(b => b.Name)
                .IsRequired();
        }
    }
}
