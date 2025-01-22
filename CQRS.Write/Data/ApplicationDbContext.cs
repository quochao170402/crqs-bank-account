using CQRS.Write.Entities;
using Microsoft.EntityFrameworkCore;

namespace CQRS.Write.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<BankAccount> BankAccounts { get; set; }
}