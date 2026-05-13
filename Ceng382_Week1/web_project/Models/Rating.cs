using System;
using System.ComponentModel.DataAnnotations;

namespace tastemam.Models
{
    public class Rating
    {
        public int ID { get; set; }

        public int? MenuID { get; set; }
        public Menu Menu { get; set; }

        public string CaretakerID { get; set; }

        public string UserID { get; set; }

        public int OrderID { get; set; }
        public Order Order { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }

        public string Comment { get; set; }

        public DateTime Date { get; set; }

        public string Type { get; set; } // menu or caretaker
    }
}