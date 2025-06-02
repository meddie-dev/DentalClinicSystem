using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using CMS.Models;

namespace CMS.Controllers
{
    public class DentalExaminationsController : Controller
    {
        private readonly AppDbContext _context;

        public DentalExaminationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DentalExaminations
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var visitsToday = await _context.Visits.Include(v => v.Patient).Include(v => v.Doctor).Where(v => v.VisitDate.Date == today).ToListAsync();

            return View(visitsToday);

        }

        // GET: DentalExamination/CreateOrEdit?patientId=1&visitId=2
        public async Task<IActionResult> CreateOrEdit(int patientId, int visitId)
        {
            var dentalExam = await _context.DentalExamination.FirstOrDefaultAsync(d => d.PatientId == patientId && d.VisitId == visitId);

            if (dentalExam == null)
            {
                dentalExam = new DentalExamination
                {
                    PatientId = patientId,
                    VisitId = visitId,
                    ExaminationDate = DateTime.Today
                };
                ViewData["IsEdit"] = false;
            }
            else
            {
                ViewData["IsEdit"] = true;
            }

            return View(dentalExam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(DentalExamination model)
        {

            ModelState.Remove("Patient");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingExam = await _context.DentalExamination.FirstOrDefaultAsync(d => d.PatientId == model.PatientId && d.VisitId == model.VisitId);

            if (existingExam == null)
            {
                _context.DentalExamination.Add(model);
            }
            else
            {
                existingExam.ExaminationDate = model.ExaminationDate;
                existingExam.Notes = model.Notes;
                _context.DentalExamination.Update(existingExam);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "DentalExaminations"); 
        }


    }
}
