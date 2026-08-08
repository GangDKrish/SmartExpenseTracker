using MediatR;

namespace SmartExpenseTracker.CQRS.Commands
{
    /// <summary>
    /// Command to delete an expense by ID.
    /// </summary>
    public class DeleteExpenseCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteExpenseCommand(int id)
        {
            Id = id;
        }
    }
}
