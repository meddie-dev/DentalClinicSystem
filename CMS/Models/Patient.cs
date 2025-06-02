using CMS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace CMS.Models;

public class Patient
{
    public int Id { get; set; }
    [Required]
    public string FullName { get; set; }
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    [Phone]
    public string ContactNumber { get; set; }
    public string Address { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }

   
    public int? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }


    // Initialize collections to empty lists to avoid null validation errors
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    public ICollection<Billing> Billings { get; set; } = new List<Billing>();
}
