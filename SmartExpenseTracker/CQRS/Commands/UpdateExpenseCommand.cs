using MediatR;

namespace SmartExpenseTracker.CQRS.Commands
{
    public class UpdateExpenseCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
