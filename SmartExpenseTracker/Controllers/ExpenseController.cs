using Microsoft.AspNetCore.Mvc;
using SmartExpenseTracker.CQRS.Commands;
using SmartExpenseTracker.CQRS.Queries;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly AddExpenseHandler _addHandler;
        private readonly GetExpensesHandler _getHandler;
        private readonly DeleteExpenseHandler _deleteHandler;

        public ExpenseController(AddExpenseHandler addHandler, GetExpensesHandler getHandler, DeleteExpenseHandler deleteHandler)
        {
            _addHandler = addHandler;
            _getHandler = getHandler;
            _deleteHandler = deleteHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var expenses = await _getHandler.Handle();
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var expenses = await _getHandler.Handle();
            var expense = expenses.FirstOrDefault(e => e.Id == id);
            if (expense is null)
                return NotFound();
            return Ok(expense);
        }

        [HttpGet("monthly-total")]
        public async Task<IActionResult> GetMonthlyTotal([FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12 || year < 1)
                return BadRequest("Invalid month or year.");

            var expenses = await _getHandler.Handle();
            var total = expenses
                .Where(e => e.Date.Month == month && e.Date.Year == year)
                .Sum(e => e.Amount);
            return Ok(new { month, year, total });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddExpenseCommand command)
        {
            var result = await _addHandler.Handle(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _deleteHandler.Handle(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
