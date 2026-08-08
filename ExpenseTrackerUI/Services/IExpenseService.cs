using ExpenseTrackerUI.Models;

namespace ExpenseTrackerUI.Services
{
    /// <summary>
    /// Service contract for expense-related operations.
    /// Abstracts HTTP communication with the backend API.
    /// </summary>
    public interface IExpenseService
    {
        /// <summary>
        /// Retrieves all expenses from the backend API.
        /// </summary>
        /// <returns>A list of all expenses.</returns>
        Task<List<Expense>> GetExpenses();

        /// <summary>
        /// Adds a new expense via the backend API.
        /// </summary>
        /// <param name="expense">The expense to add.</param>
        Task AddExpense(Expense expense);

        /// <summary>
        /// Deletes an expense by ID via the backend API.
        /// </summary>
        /// <param name="id">The ID of the expense to delete.</param>
        Task DeleteExpense(int id);
    }
}
