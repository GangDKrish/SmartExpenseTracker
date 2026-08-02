using Microsoft.EntityFrameworkCore;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
    }
}
