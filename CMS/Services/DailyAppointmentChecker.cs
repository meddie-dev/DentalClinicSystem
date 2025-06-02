using CMS.Data;
using CMS.Models;
using Microsoft.EntityFrameworkCore;

namespace CMS.Services
{
    public class DailyAppointmentChecker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DailyAppointmentChecker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                /*
                if (now.Hour == 21 && now.Minute == 0)
                {
                    await ProcessAppointments();
                }
                */

                await ProcessAppointments(); // Run always for testing
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        var now = DateTime.Now;

        //        if (now.Hour == 21 && now.Minute == 0)
        //        {
        //            await ProcessAppointments();

        //            await Task.Delay(TimeSpan.FromSeconds(61), stoppingToken);
        //        }
        //        else
        //        {
        //            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        //        }
        //    }
        //}


        private async Task ProcessAppointments()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var today = DateTime.Today;

            var appointments = await context.Appointments
                .Where(a => a.AppointmentDate.Date <= today)
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                bool hasVisit = await context.Visits
                    .AnyAsync(v => v.AppointmentId == appointment.Id);

                if (!hasVisit)
                {
                    if (appointment.AppointmentDate.Date < today &&
                        appointment.Status == AppointmentStatus.Pending)
                    {
                        appointment.Status = AppointmentStatus.NoShow;

                        int missedCount = await context.Appointments
                            .CountAsync(a => a.PatientId == appointment.PatientId && a.Status == AppointmentStatus.NoShow);

                        if (missedCount >= 5)
                        {
                            var patient = await context.Patients.FindAsync(appointment.PatientId);
                            if (patient != null)
                            {
                                patient.IsArchived = true;
                            }
                        }
                    }

                    if (appointment.Status == AppointmentStatus.Completed)
                    {
                        context.Visits.Add(new Visit
                        {
                            PatientId = appointment.PatientId,
                            AppointmentId = appointment.Id,
                            DoctorId = appointment.DoctorId,
                            VisitDate = DateTime.Now
                        });
                    }
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
