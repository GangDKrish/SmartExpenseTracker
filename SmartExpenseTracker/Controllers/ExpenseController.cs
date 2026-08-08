using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartExpenseTracker.CQRS.Commands;
using SmartExpenseTracker.CQRS.Queries;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.Controllers
{
    /// <summary>
    /// API controller for expense operations. Uses MediatR for CQRS pattern.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpenseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var expenses = await _mediator.Send(new GetExpensesQuery());
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var expenses = await _mediator.Send(new GetExpensesQuery());
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

            var expenses = await _mediator.Send(new GetExpensesQuery());
            var total = expenses
                .Where(e => e.Date.Month == month && e.Date.Year == year)
                .Sum(e => e.Amount);
            return Ok(new { month, year, total });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddExpenseCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _mediator.Send(new DeleteExpenseCommand(id));
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
