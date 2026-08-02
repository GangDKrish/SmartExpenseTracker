using SmartExpenseTracker.Data;

namespace SmartExpenseTracker.CQRS.Commands
{
    public class DeleteExpenseHandler
    {
        private readonly AppDbContext _context;

        public DeleteExpenseHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense is null)
                return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
