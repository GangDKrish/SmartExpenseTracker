using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartExpenseTracker.CQRS.Commands;
using SmartExpenseTracker.CQRS.Queries;
using SmartExpenseTracker.DTOs;
using SmartExpenseTracker.Models;

namespace SmartExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ExpenseController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        private string GetUserEmail() =>
            Request.Headers["X-User-Email"].FirstOrDefault() ?? string.Empty;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userEmail = GetUserEmail();
            var expenses = await _mediator.Send(new GetExpensesQuery { UserId = userEmail });
            var result = _mapper.Map<List<ExpenseDto>>(expenses);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userEmail = GetUserEmail();
            var expenses = await _mediator.Send(new GetExpensesQuery { UserId = userEmail });
            var expense = expenses.FirstOrDefault(e => e.Id == id);
            if (expense is null)
                return NotFound();
            return Ok(_mapper.Map<ExpenseDto>(expense));
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateExpenseDto dto)
        {
            var command = _mapper.Map<AddExpenseCommand>(dto);
            command.UserId = GetUserEmail();
            var result = await _mediator.Send(command);
            var response = _mapper.Map<ExpenseDto>(result);
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userEmail = GetUserEmail();
            var deleted = await _mediator.Send(new DeleteExpenseCommand(id, userEmail));
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
