using MediatR;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.CQRS.Commands
{
    /// <summary>
    /// Command to add a new expense.
    /// </summary>
    public class AddExpenseCommand : IRequest<Expense>
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
    }
}
