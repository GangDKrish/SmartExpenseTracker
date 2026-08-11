using ExpenseTrackerUI.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ExpenseTrackerUI.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly HttpClient _http;

        public ExpenseService(HttpClient http)
        {
            _http = http;
        }

        private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string uri, string accessToken)
        {
            var request = new HttpRequestMessage(method, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return request;
        }

        public async Task<List<Expense>> GetExpenses(string accessToken)
        {
            var request = CreateAuthorizedRequest(HttpMethod.Get, "api/expense", accessToken);
            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Expense>>() ?? new();
        }

        public async Task AddExpense(Expense expense, string accessToken)
        {
            var request = CreateAuthorizedRequest(HttpMethod.Post, "api/expense", accessToken);
            request.Content = JsonContent.Create(new
            {
                expense.Title,
                expense.Amount,
                expense.Category
            });
            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateExpense(Expense expense, string accessToken)
        {
            var request = CreateAuthorizedRequest(HttpMethod.Put, $"api/expense/{expense.Id}", accessToken);
            request.Content = JsonContent.Create(new
            {
                expense.Title,
                expense.Amount,
                expense.Category
            });
            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteExpense(int id, string accessToken)
        {
            var request = CreateAuthorizedRequest(HttpMethod.Delete, $"api/expense/{id}", accessToken);
            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
