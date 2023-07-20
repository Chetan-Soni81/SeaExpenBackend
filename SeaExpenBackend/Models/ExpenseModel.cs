using System.ComponentModel.DataAnnotations;

namespace SeaExpenBackend.Models
{
    public class ExpenseModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Category is required.")]
        public int? Category { get; set; }
        public string? CategoryName { get; set; }
        [Required(ErrorMessage ="Amount is required.")]
        public double? Amount { get; set; }
        public DateTimeOffset? RecordedDate { get; set; }
        public string? Note { get; set; }
        public int? UserId { get; set; }
    }
}
