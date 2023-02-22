using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChatApp.Entities
{
    public class ChatAppDbContext : DbContext
    {
        private readonly string connectionString =
         $"Server=(localdb)\\mssqllocaldb;Database=AppChatDb;Trusted_Connection=True;";
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
       .HasMany<Chat>(u => u.Chats)
       .WithMany(c => c.ChatUsers);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

    }
}
