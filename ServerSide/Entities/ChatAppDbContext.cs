using Microsoft.EntityFrameworkCore;
using ServerSide.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ChatApp.Entities
{
    public class ChatAppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public ChatAppDbContext(DbContextOptions<ChatAppDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Users)
                .WithMany(u => u.Chats)
                .UsingEntity<ChatUser>(
                    j => j
                        .HasOne(cu => cu.User)
                        .WithMany(u => u.ChatUsers)
                        .HasForeignKey(cu => cu.UserId),
                     j => j
                        .HasOne(cu => cu.Chat)
                        .WithMany(c => c.ChatUsers)
                        .HasForeignKey(cu => cu.ChatId),
                    j =>
                    {
                        j.HasKey(cu => new { cu.ChatId, cu.UserId });
                    }
                );
        }
    }
}
