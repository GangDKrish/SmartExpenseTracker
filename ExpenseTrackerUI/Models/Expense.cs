using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerUI.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Amount must be between 0.01 and 999,999.99")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        public string Category { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
