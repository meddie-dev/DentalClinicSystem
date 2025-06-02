using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CMS.Models
{
    public enum AppointmentStatus
    {
        Pending,
        Completed,
        NoShow,
        Cancelled
    }

    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }
        public string Notes { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    }

}
