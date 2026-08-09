using ExpenseTrackerUI.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace ExpenseTrackerUI.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly HttpClient _http;

        public ExpenseService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Expense>> GetExpenses(string userEmail)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/expense");
            request.Headers.Add("X-User-Email", userEmail);
            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Expense>>() ?? new();
        }

        public async Task AddExpense(Expense expense, string userEmail)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/expense");
            request.Headers.Add("X-User-Email", userEmail);
            request.Content = JsonContent.Create(new
            {
                expense.Title,
                expense.Amount,
                expense.Category
            });
            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteExpense(int id, string userEmail)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/expense/{id}");
            request.Headers.Add("X-User-Email", userEmail);
            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
