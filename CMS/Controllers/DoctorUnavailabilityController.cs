using System.Security.Claims;
using CMS.Data;
using CMS.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers
{
    public class DoctorUnavailabilityController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IAntiforgery _antiforgery;

        public DoctorUnavailabilityController(AppDbContext context, IAntiforgery antiforgery)
        {
            _context = context;
            _antiforgery = antiforgery;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null) return NotFound();

            ViewBag.DoctorId = doctor.Id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUnavailableDates([FromBody] UnavailableDatesUpdate model)
        {
           
            var existing = await _context.DoctorUnavailability.Where(d => d.DoctorId == model.DoctorId && d.Date >= DateTime.Today).ToListAsync();

            _context.DoctorUnavailability.RemoveRange(existing);

            foreach (var dateStr in model.UnavailableDates)
            {
                if (DateTime.TryParse(dateStr, out DateTime date))
                {
                    _context.DoctorUnavailability.Add(new DoctorUnavailability
                    {
                        DoctorId = model.DoctorId,
                        Date = date,
                        StartTime = new TimeSpan(0, 0, 0),
                        EndTime = new TimeSpan(23, 59, 0)
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        public class UnavailableDatesUpdate
        {
            public int DoctorId { get; set; }
            public List<string> UnavailableDates { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(int doctorId)
        {
            var unavailableDates = await _context.DoctorUnavailability.Where(d => d.DoctorId == doctorId).Select(d => d.Date.ToString("yyyy-MM-dd")).Distinct().ToListAsync();

            return Json(unavailableDates);
        }

    }
}
