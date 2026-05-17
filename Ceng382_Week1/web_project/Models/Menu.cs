using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tastemam.Models
{
    public class Menu
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }
        public string Category { get; set; }
        public string ImagePath { get; set; } 
        public string CaretakerID { get; set; } // IdentityUser ID
        public ICollection<CustomizationGroup> CustomizationGroups { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}