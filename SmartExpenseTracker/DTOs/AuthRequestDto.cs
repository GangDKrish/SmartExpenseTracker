namespace SmartExpenseTracker.DTOs
{
    public class AuthRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Name { get; set; }
    }
}
