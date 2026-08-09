using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartExpenseTracker.Data;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.CQRS.Queries
{
    /// <summary>
    /// Handler for GetExpensesQuery that retrieves all expenses from the database.
    /// </summary>
    public class GetExpensesHandler : IRequestHandler<GetExpensesQuery, List<Expense>>
    {
        private readonly AppDbContext _context;

        public GetExpensesHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Expense>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Expenses
                .Where(e => e.UserId == request.UserId)
                .ToListAsync(cancellationToken);
        }
    }
}
