using Microsoft.EntityFrameworkCore;
using SmartExpenseTracker.Data;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.CQRS.Queries
{
    public class GetExpensesHandler
    {
        private readonly AppDbContext _context;

        public GetExpensesHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Expense>> Handle()
        {
            return await _context.Expenses.ToListAsync();
        }
    }
}
