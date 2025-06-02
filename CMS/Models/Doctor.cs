using System.ComponentModel.DataAnnotations;

namespace CMS.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }


        [Phone]
        public string ContactNumber { get; set; }

        public string Email { get; set; }

        public bool IsArchived { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Patient> Patients { get; set; } = new List<Patient>();

        public ICollection<Visit> Visits { get; set; } = new List<Visit>();
        public ICollection<DoctorUnavailability> Availabilities { get; set; } = new List<DoctorUnavailability>();

        // Link to Identity User
        public string UserId { get; set; }
        public Users User { get; set; }
    }
}

