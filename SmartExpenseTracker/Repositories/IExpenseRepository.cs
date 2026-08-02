using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.Repositories
{
    public interface IExpenseRepository
    {
        Task<List<Expense>> GetAllAsync();
        Task<Expense> GetByIdAsync(int id);
        Task AddAsync(Expense expense);
        Task DeleteAsync(int id);

        Task<decimal> GetMonthlyTotal(int month, int year);
    }
}
