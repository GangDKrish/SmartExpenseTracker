using ExpenseTrackerUI.Models;

namespace ExpenseTrackerUI.Services
{
    /// <summary>
    /// Service contract for expense-related operations.
    /// Abstracts HTTP communication with the backend API.
    /// </summary>
    public interface IExpenseService
    {
        Task<List<Expense>> GetExpenses(string userEmail);
        Task AddExpense(Expense expense, string userEmail);
        Task DeleteExpense(int id, string userEmail);
    }
}
