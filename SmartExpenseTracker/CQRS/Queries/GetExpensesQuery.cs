using MediatR;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.CQRS.Queries
{
    public class GetExpensesQuery : IRequest<List<Expense>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
