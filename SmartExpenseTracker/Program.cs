using Microsoft.EntityFrameworkCore;
using SmartExpenseTracker.CQRS.Commands;
using SmartExpenseTracker.CQRS.Queries;
using SmartExpenseTracker.Data;
using SmartExpenseTracker.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowExpenseTrackerUI", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7184",
                "http://localhost:5140")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<AddExpenseHandler>();
builder.Services.AddScoped<GetExpensesHandler>();
builder.Services.AddScoped<DeleteExpenseHandler>();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowExpenseTrackerUI");

app.UseAuthorization();

app.MapControllers();

app.Run();
