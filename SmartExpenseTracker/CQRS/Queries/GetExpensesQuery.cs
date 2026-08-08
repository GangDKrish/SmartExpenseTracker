using MediatR;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.CQRS.Queries
{
    /// <summary>
    /// Query to retrieve all expenses from the database.
    /// </summary>
    public class GetExpensesQuery : IRequest<List<Expense>>
    {
        // No parameters needed for getting all expenses
    }
}
