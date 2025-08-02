using System.ComponentModel.DataAnnotations;

namespace MinimalAPI.Models
{
    public class Product
    {
        [Required(ErrorMessage = "Id is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Id should be between 0 to maximum value of int")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]

        public string? Name { get; set; }

        public override string ToString()
        {
            return $"Product ID: {Id}, Product Name: {Name}";
        }
    }
}
