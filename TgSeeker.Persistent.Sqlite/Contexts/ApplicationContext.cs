using Microsoft.EntityFrameworkCore;
using TgSeeker.Persistent.Entities;

namespace TgSeeker.Persistent.Contexts
{
    public class ApplicationContext : DbContext
    {
        public DbSet<TgsMessage> Messages { get; set; } = null!;
        public DbSet<Option> Options { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=tgseeker.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TgsMessage>(entity =>
            {
                entity.Property(i => i.Id).ValueGeneratedNever();
                entity.HasIndex(i => i.Id);
                entity.HasIndex(i => i.ChatId);
            });

            modelBuilder.Entity<TgsTextMessage>().HasBaseType<TgsMessage>();
            modelBuilder.Entity<TgsVoiceMessage>().HasBaseType<TgsMessage>();

            modelBuilder.Entity<Option>(entity =>
            {
                entity.HasKey(i => i.Key);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
