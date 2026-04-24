using System.Collections.Generic;

namespace tastemam.Models
{
    public class CartItem
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public List<SelectedOption> SelectedOptions { get; set; } = new List<SelectedOption>();

        public decimal TotalPrice => (UnitPrice + SelectedOptions.Sum(o => o.PriceModifier)) * Quantity;
    }

    public class SelectedOption
    {
        public int OptionID { get; set; }
        public string OptionName { get; set; }
        public decimal PriceModifier { get; set; }
    }
}