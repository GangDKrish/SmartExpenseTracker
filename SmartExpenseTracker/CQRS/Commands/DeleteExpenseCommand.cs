using MediatR;

namespace SmartExpenseTracker.CQRS.Commands
{
    public class DeleteExpenseCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        public DeleteExpenseCommand(int id, string userId = "")
        {
            Id = id;
            UserId = userId;
        }
    }
}
