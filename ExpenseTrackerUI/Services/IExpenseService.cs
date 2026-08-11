using ExpenseTrackerUI.Models;

namespace ExpenseTrackerUI.Services
{
    /// <summary>
    /// Service contract for expense-related operations.
    /// Abstracts HTTP communication with the backend API.
    /// </summary>
    public interface IExpenseService
    {
        Task<List<Expense>> GetExpenses(string accessToken);
        Task AddExpense(Expense expense, string accessToken);
        Task UpdateExpense(Expense expense, string accessToken);
        Task DeleteExpense(int id, string accessToken);
    }
}
