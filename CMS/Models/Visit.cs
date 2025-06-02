using System.ComponentModel.DataAnnotations;

namespace CMS.Models
{
    public class Visit
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int DoctorId { get; set; }          
        public Doctor? Doctor { get; set; }

        public DateTime VisitDate { get; set; }



        public ICollection<Prescription> Prescriptions { get; set; }
        public ICollection<DentalExamination> DentalExaminations { get; set; }

    }
}
