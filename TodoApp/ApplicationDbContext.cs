using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
    }
}
