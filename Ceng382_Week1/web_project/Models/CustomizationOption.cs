using System.ComponentModel.DataAnnotations;

namespace tastemam.Models
{
    public class CustomizationOption
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; } 

        public decimal PriceModifier { get; set; } // if 0, free

        public bool IsDefault { get; set; } 

        public int CustomizationGroupID { get; set; }
        public CustomizationGroup CustomizationGroup { get; set; }
    }
}