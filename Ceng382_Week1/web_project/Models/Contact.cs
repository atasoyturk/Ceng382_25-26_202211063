using System;
using System.ComponentModel.DataAnnotations;

namespace tastemam.Models
{
    public class Contact
    {
        public int ID{ get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email{ get; set; }

        public string Message { get; set; }

        public DateTime Date{ get; set; }
    }
}