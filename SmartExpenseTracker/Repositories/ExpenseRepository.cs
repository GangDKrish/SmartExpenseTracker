using SmartExpenseTracker.Models;
using SmartExpenseTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace SmartExpenseTracker.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext _context;

        public ExpenseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Expense expense)
        {
            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Expense>> GetAllAsync()
        {
            return await _context.Expenses.ToListAsync();
        }

        public async Task<Expense> GetByIdAsync(int id)
        {
            return await _context.Expenses.FindAsync(id);
        }

        public async Task<decimal> GetMonthlyTotal(int month, int year)
        {
            return await _context.Expenses
                .Where(e => e.Date.Month == month && e.Date.Year == year)
                .SumAsync(e => e.Amount);
        }
    }
}
