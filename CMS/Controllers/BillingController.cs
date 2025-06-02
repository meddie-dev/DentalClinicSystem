using CMS.Data;
using CMS.Models;
using CMS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CMS.Controllers
{
    public class BillingController : Controller
    {
        private readonly AppDbContext _context;

        public BillingController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var billings = _context.Billings.Include(b => b.Patient).Include(b => b.Prescription)
                .Select(b => new BillingViewModel
                {
                    BillingId = b.Id,
                    PatientName = b.Patient.FullName,
                    InvoiceDate = b.InvoiceDate,
                    TotalAmount = b.TotalAmount,
                    DentalExaminationId = b.Prescription.DentalExaminationId 
                }).ToList();

            return View(billings);
        }

        public IActionResult Details(int examId)
        {
            var prescription = _context.Prescriptions.Include(p => p.PrescriptionItems).ThenInclude(pi => pi.InventoryItem).Include(p => p.Visit).ThenInclude(v => v.Patient).FirstOrDefault(p => p.DentalExaminationId == examId);

            if (prescription == null) return NotFound();

            var viewModel = new PrescriptionDetailsViewModel
            {
                PrescriptionId = prescription.Id,
                PatientName = prescription.Visit.Patient.FullName,
                ExaminationId = prescription.DentalExaminationId,
                Instructions = prescription.Instructions,
                Items = prescription.PrescriptionItems.Select(pi => new PrescriptionItemViewModel
                {
                    ItemName = pi.InventoryItem.ItemName,
                    Quantity = pi.Quantity,
                    UnitPrice = pi.UnitPrice,
                    SubTotal = pi.Quantity * pi.UnitPrice
                }).ToList(),
                BillingExists = _context.Billings.Any(b => b.PrescriptionId == prescription.Id)
            };

            viewModel.TotalAmount = viewModel.Items.Sum(i => i.SubTotal);

            return View(viewModel);
        }

    }
}
