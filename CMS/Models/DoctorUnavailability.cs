using System.ComponentModel.DataAnnotations;

namespace CMS.Models
{
    public class DoctorUnavailability
    {
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }   
        public Doctor Doctor { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
    }
}
