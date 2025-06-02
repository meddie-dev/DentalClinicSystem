using System.ComponentModel.DataAnnotations;

namespace CMS.Models
{
    public class DentalExamination
    {
        public int Id { get; set; }


        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int? VisitId { get; set; }
        public Visit? Visit { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExaminationDate { get; set; }

        public string Notes { get; set; }
    }
}
