using Microsoft.EntityFrameworkCore;
using SecureDocumentStorageSystem.Models;

namespace SecureDocumentStorageSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
    }
}