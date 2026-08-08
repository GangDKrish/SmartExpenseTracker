using MediatR;
using SmartExpenseTracker.Data;

namespace SmartExpenseTracker.CQRS.Commands
{
    /// <summary>
    /// Handler for DeleteExpenseCommand that removes an expense from the database.
    /// </summary>
    public class DeleteExpenseHandler : IRequestHandler<DeleteExpenseCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteExpenseHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteExpenseCommand command, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses.FindAsync(new object[] { command.Id }, cancellationToken);
            if (expense is null)
                return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
