using System;

namespace tastemam.Models
{
    public class CaretakerAgreement
    {
        public int ID { get; set; }

        public string CaretakerID { get; set; }

        public string CaretakerEmail { get; set; }

        public DateTime SignedDate { get; set; }

        public bool IsApproved { get; set; }
    }
}