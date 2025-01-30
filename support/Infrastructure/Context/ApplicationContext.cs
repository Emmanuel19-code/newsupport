using Microsoft.EntityFrameworkCore;
using support.Domain;

namespace support.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<ConversationParticipants> Participants { get; set; }
        public DbSet<PasswordResetLogs> PasswordResetLogs { get; set; }
        public DbSet<RefreshTokenDb> RefreshTokenDb { get; set; }
        public DbSet<SystemAdminDb> SystemAdmins { get; set; }
       
    }
}
