using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CMS.Models
{
    public class Payment
    {
        public int Id { get; set; }
        [Required]
        public int BillingId { get; set; }
        public Billing Billing { get; set; }
        [Precision(18, 2)]
        public decimal AmountPaid { get; set; }

        public DateTime PaymentDate { get; set; }

    }
}
