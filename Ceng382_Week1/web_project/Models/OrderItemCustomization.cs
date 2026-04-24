namespace tastemam.Models
{
    public class OrderItemCustomization
    {
        public int ID { get; set; }

        public int OrderItemID { get; set; }
        public OrderItem OrderItem { get; set; }

        public int CustomizationOptionID { get; set; }
        public CustomizationOption CustomizationOption { get; set; }
    }
}