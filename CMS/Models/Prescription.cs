using System.ComponentModel.DataAnnotations;

namespace CMS.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        [Required]
        public int VisitId { get; set; }
        public Visit Visit { get; set; }

        public int DentalExaminationId { get; set; }
        public DentalExamination DentalExamination { get; set; }

        public ICollection<Payment> Payments { get; set; }
        public ICollection<PrescriptionItem> PrescriptionItems { get; set; }


        public string Instructions { get; set; }

        public Billing Billing { get; set; }

    }
}
