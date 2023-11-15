using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace rabbitMqExample.WebExcelCreate.Models {
    public class AppDbContext : IdentityDbContext {
        public AppDbContext(DbContextOptions options) : base(options) {
        }

        public DbSet<UserFile> UserFiles { get; set; }

    }
}
