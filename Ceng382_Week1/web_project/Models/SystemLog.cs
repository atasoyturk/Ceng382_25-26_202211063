using System;

namespace tastemam.Models
{
    public class SystemLog
    {
        public int ID { get; set; }

        public string EventType { get; set; } // Auth, Order, Payment, Rating, Error

        public string Message { get; set; }

        public string UserEmail { get; set; }

        public DateTime Date { get; set; }

        public string Level { get; set; } // Info, Warning, Error
    }
}