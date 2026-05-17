using System.ComponentModel.DataAnnotations;

namespace tastemam.Models
{
    public class TwoFactorViewModel
    {
        [Required]
        public string Code { get; set; }
        public string Email { get; set; }
        public bool RememberMe { get; set; }
    }
}