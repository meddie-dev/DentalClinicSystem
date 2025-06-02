using CMS.Data;
using CMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class VisitsController : Controller
{
    private readonly AppDbContext _context;

    public VisitsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Details(int id)
    {
        var visit = await _context.Visits.Include(v => v.DentalExaminations).Include(v => v.Prescriptions).ThenInclude(p => p.PrescriptionItems).ThenInclude(pi => pi.InventoryItem).Include(v => v.Doctor).Include(v => v.Patient).FirstOrDefaultAsync(v => v.Id == id);

        if (visit == null)
        {
            return NotFound();
        }

        var prescriptionIds = visit.Prescriptions.Select(p => p.Id).ToList();

        var billings = await _context.Billings.Where(b => prescriptionIds.Contains(b.PrescriptionId)).Include(b => b.Payments).ToListAsync();

        var billingByPrescriptionId = billings.ToDictionary(b => b.PrescriptionId);

        ViewData["BillingByPrescriptionId"] = billingByPrescriptionId;

        return View("~/Views/Visit/Details.cshtml", visit);
    }
}
