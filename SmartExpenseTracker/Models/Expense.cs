using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartExpenseTracker.Models
{
    /// <summary>
    /// Represents an expense entry. Note: ID gaps after deletion are normal database behavior
    /// as primary keys should never be reused for data integrity and audit trail purposes.
    /// </summary>
    public class Expense
    {
        /// <summary>
        /// Unique identifier (auto-generated). Gaps in sequence are expected after deletions.
        /// </summary>
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

        public DateTime Date { get; set; }
    }
}
