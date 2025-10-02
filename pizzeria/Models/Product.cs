using System;
using System.ComponentModel.DataAnnotations;

namespace pizzeria.Models
{
    public enum ProductType
    {
        [Display(Name = "Блюдо")]
        Dish,
        [Display(Name = "Напиток")]
        Drink,
        [Display(Name = "Предмет")]
        Subject
    }

    public class Product
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Description { get; set; }
        public float Weight { get; set; }
        public float Calories { get; set; }
        public decimal Price { get; set; }
        public string? Brand { get; set; }
        public string? Image { get; set; }
        public ProductType Type { get; set; }
        public DateTime DateOfPublication { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
