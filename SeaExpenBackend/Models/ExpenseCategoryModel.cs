using System.ComponentModel.DataAnnotations;

namespace SeaExpenBackend.Models
{
    public class ExpenseCategoryModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Category Name is required.")]
        public string? Category { get; set; }
    }
}
