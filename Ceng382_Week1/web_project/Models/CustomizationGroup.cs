using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tastemam.Models
{
    public class CustomizationGroup
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; } // "sauces"

        public string Type { get; set; } // "removable"

        public int MenuID { get; set; }
        public Menu Menu { get; set; }

        public ICollection<CustomizationOption> Options { get; set; }
    }
}