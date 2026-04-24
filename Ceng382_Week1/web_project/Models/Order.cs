using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tastemam.Models
{
    public class Order
    {
        public int ID { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string Phone { get; set; }

        public DateTime Date { get; set; }

        public decimal TotalPrice { get; set; }

        public string State { get; set; } // pending, completed

        public string UserID { get; set; } // IdentityUser ID

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}