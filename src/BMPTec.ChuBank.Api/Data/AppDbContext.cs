using BMPTec.ChuBank.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BMPTec.ChuBank.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Transfer> Transfers => Set<Transfer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(eb =>
            {
                eb.HasKey(a => a.Id);
                eb.Property(a => a.CPF).IsRequired();
                eb.HasIndex(a => a.CPF).IsUnique();
                eb.Property(a => a.Balance).HasColumnType("numeric(18,2)");
            });

            modelBuilder.Entity<Transfer>(eb =>
            {
                eb.HasKey(t => t.Id);
                eb.Property(t => t.Amount).HasColumnType("numeric(18,2)");
                eb.HasOne(t => t.FromAccount).WithMany().HasForeignKey(t => t.FromAccountId).OnDelete(DeleteBehavior.Restrict);
                eb.HasOne(t => t.ToAccount).WithMany().HasForeignKey(t => t.ToAccountId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
