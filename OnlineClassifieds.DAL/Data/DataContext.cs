using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using OnlineClassifieds.Models;


namespace OnlineClassifieds.DAL.Data
{
    public class DataContext : IdentityDbContext
    {
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<User> _Users { get; set; } = null!;
        public DbSet<Announcement> Announcements { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
