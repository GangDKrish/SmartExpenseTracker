using SmartExpenseTracker.Data;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.CQRS.Commands
{
    public class AddExpenseHandler
    {
        private readonly AppDbContext _context;

        public AddExpenseHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Expense> Handle(AddExpenseCommand command)
        {
            var expense = new Expense
            {
                Title = command.Title,
                Amount = command.Amount,
                Category = command.Category,
                Date = DateTime.Now
            };
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return expense;
        }
    }
}
