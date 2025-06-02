using CMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMS.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }

        public DbSet<CMS.Models.DoctorUnavailability> DoctorUnavailability { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Doctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Patient)
                .WithMany(p => p.Visits)
                .HasForeignKey(v => v.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Visit>()
                .HasOne(v => v.Doctor)
                .WithMany(d => d.Visits)
                .HasForeignKey(v => v.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Billing>()
                .HasOne(b => b.Prescription)
                .WithMany()
                .HasForeignKey(b => b.PrescriptionId)
                .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction

            modelBuilder.Entity<Billing>()
                .HasOne(b => b.Patient)
                .WithMany(p => p.Billings)
                .HasForeignKey(b => b.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            // optional if needed

            modelBuilder.Entity<Prescription>()
               .HasOne(p => p.Billing)
               .WithOne(b => b.Prescription)
               .HasForeignKey<Billing>(b => b.PrescriptionId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Billing)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BillingId);



        }
        public DbSet<CMS.Models.DentalExamination> DentalExamination { get; set; } = default!;



    }
}
