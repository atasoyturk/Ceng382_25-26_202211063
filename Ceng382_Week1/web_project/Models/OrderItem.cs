namespace tastemam.Models
{
    public class OrderItem
    {
        public int ID { get; set; }

        public int OrderID { get; set; }
        public Order Order{ get; set; }

        public int MenuID { get; set; }
        public Menu Menu { get; set; }

        public int Pieces { get; set; }
    }
}