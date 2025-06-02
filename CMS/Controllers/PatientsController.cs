using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using CMS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CMS.Controllers
{
    public class PatientsController : Controller
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (User.IsInRole("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

                if (doctor != null)
                {
                    var patients = await _context.Patients.Where(p => p.DoctorId == doctor.Id).ToListAsync();

                    return View(patients);
                }

                return NotFound("No doctor profile found for this user.");
            }

            return View(await _context.Patients.Where(p => p.IsArchived == false).ToListAsync());
        }

        public async Task<IActionResult> IsArchived()
        {
            var archivedPatients = await _context.Patients.Where(p => p.IsArchived == true).ToListAsync();

            return View(archivedPatients);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Unarchive(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            if (!patient.IsArchived)
            {
                return RedirectToAction("Index");
            }

            patient.IsArchived = false;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.Include(p => p.Appointments.Where(a => a.AppointmentDate.Date == DateTime.Today)).ThenInclude(a => a.Doctor).Include(p => p.Visits).ThenInclude(v => v.Doctor).FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }


            return View("~/Views/Patients/Details.cshtml", patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View("~/Views/Patients/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,DateOfBirth,Gender,ContactNumber,Address,IsArchived")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                patient.CreatedAt = DateTime.Now;
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }



        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View("~/Views/Patients/Edit.cshtml", patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,DateOfBirth,Gender,ContactNumber,Address,IsArchived,CreatedAt")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
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
            return View("~/Views/Patients/Edit.cshtml", patient);
        }

        [Authorize(Roles = "Admin")]
        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View("~/Views/Patients/Delete.cshtml", patient);
        }


        // POST: Patients/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}
