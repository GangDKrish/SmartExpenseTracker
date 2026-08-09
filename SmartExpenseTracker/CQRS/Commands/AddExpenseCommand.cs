using MediatR;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.CQRS.Commands
{
    public class AddExpenseCommand : IRequest<Expense>
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
