using MediatR;
using SmartExpenseTracker.Data;

namespace SmartExpenseTracker.CQRS.Commands
{
    public class UpdateExpenseHandler : IRequestHandler<UpdateExpenseCommand, bool>
    {
        private readonly AppDbContext _context;

        public UpdateExpenseHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateExpenseCommand command, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses.FindAsync(new object[] { command.Id }, cancellationToken);
            if (expense is null || expense.UserId != command.UserId)
                return false;

            expense.Title = command.Title;
            expense.Amount = command.Amount;
            expense.Category = command.Category;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
