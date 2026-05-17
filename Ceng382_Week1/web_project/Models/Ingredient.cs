namespace tastemam.Models
{
    public class Ingredient
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsRemovable { get; set; }
        public int MenuID { get; set; }
        public Menu Menu { get; set; }
    }
}