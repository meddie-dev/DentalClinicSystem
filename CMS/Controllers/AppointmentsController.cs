
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using CMS.Models;
using System.Security.Claims;

namespace CMS.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

                if (doctor != null)
                {
                    var doctorAppointments = await _context.Appointments.Include(a => a.Patient).Where(a => a.DoctorId == doctor.Id).ToListAsync();

                    return View(doctorAppointments);
                }

                return NotFound("No doctor profile found for this user.");
            }

            var allAppointments = await _context.Appointments.Include(a => a.Patient).ToListAsync();

            return View(allAppointments);
        }


        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.Include(a => a.Patient).Include(a => a.Doctor).FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create(DateTime? appointmentDate)
        {
            ViewBag.PatientId = new SelectList(_context.Patients, "Id", "FullName");

            if (appointmentDate.HasValue)
            {
                var date = appointmentDate.Value.Date;

                var unavailableDoctorIds = _context.DoctorUnavailability.Where(d => d.Date == date).Select(d => d.DoctorId).Distinct();

                var availableDoctors = _context.Doctors.Where(d => !unavailableDoctorIds.Contains(d.Id)).ToList();

                ViewBag.DoctorId = new SelectList(availableDoctors, "Id", "FullName");
            }
            else
            {
                ViewBag.DoctorId = new SelectList(_context.Doctors, "Id", "FullName");
            }

            ViewData["StatusList"] = new SelectList(Enum.GetValues(typeof(AppointmentStatus)).Cast<AppointmentStatus>().Select(s => new { Value = (int)s, Text = s.ToString() }), "Value", "Text");

            return View();
        }


        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PatientId,DoctorId,AppointmentDate,Notes,Status")] Appointment appointment)
        {
            ModelState.Remove("Patient");
            ModelState.Remove("Doctor");


            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();

                var patient = await _context.Patients.FindAsync(appointment.PatientId);
                if (patient != null && patient.DoctorId != appointment.DoctorId)
                {
                    patient.DoctorId = appointment.DoctorId;
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.PatientId = new SelectList(_context.Patients, "Id", "FullName", appointment?.PatientId);
            ViewBag.DoctorId = new SelectList(_context.Doctors, "Id", "FullName", appointment?.DoctorId);
            ViewData["StatusList"] = new SelectList(Enum.GetValues(typeof(AppointmentStatus)).Cast<AppointmentStatus>().Select(s => new { Value = (int)s, Text = s.ToString() }), "Value", "Text", appointment.Status);

            var existingAppointments = await _context.Appointments.CountAsync(a => a.AppointmentDate.Date == appointment.AppointmentDate.Date);

            if (existingAppointments >= 10)
            {
                ModelState.AddModelError("AppointmentDate", "Selected date is fully booked (10 appointments max).");
            }
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            var patientFullName = await _context.Patients.Where(p => p.Id == appointment.PatientId).Select(p => p.FullName).FirstOrDefaultAsync();

            ViewBag.PatientFullName = patientFullName ?? "Unknown";
            ViewBag.PatientId = new SelectList(_context.Patients, "Id", "FullName", appointment?.PatientId);
            ViewBag.DoctorId = new SelectList(_context.Doctors, "Id", "FullName", appointment?.DoctorId);
            ViewData["StatusList"] = new SelectList(Enum.GetValues(typeof(AppointmentStatus)).Cast<AppointmentStatus>().Select(s => new { Value = (int)s, Text = s.ToString() }), "Value", "Text", appointment.Status);

            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,DoctorId,AppointmentDate,Notes,Status")] Appointment appointment)
        {
            ModelState.Remove("Patient");

            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PatientId = new SelectList(_context.Patients, "Id", "FullName", appointment?.PatientId);
            ViewBag.DoctorId = new SelectList(_context.Doctors, "Id", "FullName", appointment?.DoctorId);
            ViewData["StatusList"] = new SelectList(Enum.GetValues(typeof(AppointmentStatus)).Cast<AppointmentStatus>().Select(s => new { Value = (int)s, Text = s.ToString() }), "Value", "Text", appointment.Status);

            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.Include(a => a.Patient).FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointmentsJson()
        {
            var appointments = await _context.Appointments.GroupBy(a => a.AppointmentDate.Date)
                .Select(g => new
                {
                    date = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            var events = appointments.Select(a => new
            {
                title = $"{a.count} Appointments",
                start = a.date.ToString("yyyy-MM-dd"),
                allDay = true,
                backgroundColor = a.count >= 5 ? "#6c757d" : "#28a745",
                borderColor = "transparent"
            });

            return Json(events);
        }

        [HttpGet]
        public async Task<IActionResult> CountByDate(string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                return BadRequest("Invalid date");

            var count = await _context.Appointments.CountAsync(a => a.AppointmentDate.Date == parsedDate.Date);

            return Json(new { count });
        }


        [HttpGet]
        public async Task<IActionResult> GetAvailableDoctors(DateTime date)
        {
            var unavailableDoctorIds = await _context.DoctorUnavailability.Where(d => d.Date == date.Date).Select(d => d.DoctorId).Distinct().ToListAsync();
            var availableDoctors = await _context.Doctors.Where(d => !unavailableDoctorIds.Contains(d.Id)).Select(d => new { id = d.Id, fullName = d.FullName }).ToListAsync();

            return Json(availableDoctors);
        }
    }
}
