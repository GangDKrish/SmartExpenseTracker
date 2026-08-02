namespace SmartExpenseTracker.CQRS.Commands
{
    public class AddExpenseCommand
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
    }
}
