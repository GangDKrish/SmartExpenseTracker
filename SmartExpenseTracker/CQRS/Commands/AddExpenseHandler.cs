using AutoMapper;
using MediatR;
using SmartExpenseTracker.Data;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.CQRS.Commands
{
    /// <summary>
    /// Handler for AddExpenseCommand that creates a new expense in the database.
    /// </summary>
    /// <remarks>
    /// ARCHITECTURE NOTE: This handler directly uses AppDbContext instead of a repository layer.
    /// This is intentional and follows CQRS best practices:
    /// 
    /// - EF Core DbContext already implements Repository and Unit of Work patterns
    /// - CQRS handlers provide operation-specific abstractions (better than generic repositories)
    /// - Controllers depend on IMediator abstraction, maintaining separation of concerns
    /// - Each handler has single responsibility (Add, Get, Delete operations isolated)
    /// - Testable via mocking AppDbContext or using in-memory database
    /// 
    /// Adding a repository layer on top of DbContext + CQRS would be redundant abstraction.
    /// </remarks>
    public class AddExpenseHandler : IRequestHandler<AddExpenseCommand, Expense>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AddExpenseHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Expense> Handle(AddExpenseCommand command, CancellationToken cancellationToken)
        {
            var expense = _mapper.Map<Expense>(command);
            expense.UserId = command.UserId;
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync(cancellationToken);
            return expense;
        }
    }
}
