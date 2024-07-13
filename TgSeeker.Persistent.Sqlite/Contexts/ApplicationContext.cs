using Microsoft.EntityFrameworkCore;
using TgSeeker.Persistent.Entities;

namespace TgSeeker.Persistent.Contexts
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<Option> Options { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=tgseeker.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.HasIndex(i => i.Id);
                entity.HasIndex(i => i.ChatId);
            });

            modelBuilder.Entity<Option>(entity =>
            {
                entity.HasKey(i => i.Key);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
