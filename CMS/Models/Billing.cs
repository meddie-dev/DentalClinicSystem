using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CMS.Models
{
    public class Billing
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }

        public DateTime InvoiceDate { get; set; }

        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
