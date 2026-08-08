using ExpenseTrackerUI.Models;
using System.Net.Http.Json;

namespace ExpenseTrackerUI.Services
{
    /// <summary>
    /// Implementation of expense service using HTTP client to communicate with the backend API.
    /// </summary>
    public class ExpenseService : IExpenseService
    {
        private readonly HttpClient _http;

        public ExpenseService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Expense>> GetExpenses()
        {
            return await _http.GetFromJsonAsync<List<Expense>>("api/expense");
        }

        public async Task AddExpense(Expense expense)
        {
            await _http.PostAsJsonAsync("api/expense", new
            {
                expense.Title,
                expense.Amount,
                expense.Category
            });
        }

        public async Task DeleteExpense(int id)
        {
            await _http.DeleteAsync($"api/expense/{id}");
        }
    }
}
