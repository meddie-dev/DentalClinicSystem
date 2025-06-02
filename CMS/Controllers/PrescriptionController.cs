using CMS.Data;
using CMS.Models;
using CMS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly AppDbContext _context;

        public PrescriptionController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var today = DateTime.Today;

            var exams = _context.DentalExamination.Include(e => e.Visit).ThenInclude(v => v.Patient).Where(e => e.ExaminationDate.Date == today)
                .Select(e => new ExaminationIndexViewModel
                {
                    ExaminationId = e.Id,
                    ExaminationDate = e.ExaminationDate,
                    PatientName = e.Visit.Patient.FullName,
                    BillingExists = _context.Billings.Any(b => b.Prescription.DentalExaminationId == e.Id)
                }).ToList();

            return View(exams);
        }



        public IActionResult Create(int examId)
        {
            var exam = _context.DentalExamination.Include(e => e.Visit).FirstOrDefault(e => e.Id == examId);

            if (exam == null) return NotFound();

            var inventoryItems = _context.InventoryItems.ToList();

            var viewModel = new PrescriptionCreateViewModel
            {
                DentalExaminationId = exam.Id,
                VisitId = exam.VisitId.Value,
                SelectedItems = inventoryItems.Select(i => new InventoryItemSelection
                {
                    InventoryItemId = i.Id,
                    ItemName = i.ItemName,
                    UnitPrice = 10m // P10.00 Pesos
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Create(PrescriptionCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var prescription = new Prescription
            {
                VisitId = model.VisitId,
                DentalExaminationId = model.DentalExaminationId,
                Instructions = model.Instructions,
                PrescriptionItems = new List<PrescriptionItem>()
            };

            decimal totalAmount = 0m;

            foreach (var item in model.SelectedItems.Where(i => i.Selected && i.Quantity.HasValue && i.Quantity > 0))
            {
                var subTotal = item.UnitPrice * item.Quantity.Value;
                prescription.PrescriptionItems.Add(new PrescriptionItem
                {
                    InventoryItemId = item.InventoryItemId,
                    Quantity = item.Quantity.Value,
                    UnitPrice = item.UnitPrice
                });

                totalAmount += subTotal;
            }


            _context.Prescriptions.Add(prescription);
            _context.SaveChanges();

            var billing = new Billing
            {
                PatientId = _context.Visits.Where(v => v.Id == model.VisitId).Select(v => v.PatientId).FirstOrDefault(),
                PrescriptionId = prescription.Id,
                InvoiceDate = DateTime.Now,
                TotalAmount = totalAmount
            };

            _context.Billings.Add(billing);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = prescription.Id });

        }

        public IActionResult Details(int examId)
        {
            var prescription = _context.Prescriptions.Include(p => p.PrescriptionItems).ThenInclude(pi => pi.InventoryItem).Include(p => p.Visit).ThenInclude(v => v.Patient).FirstOrDefault(p => p.DentalExaminationId == examId);

            if (prescription == null)
                return NotFound();

            var billingExists = _context.Billings
                .Any(b => b.PrescriptionId == prescription.Id);

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
                BillingExists = billingExists
            };

            viewModel.TotalAmount = viewModel.Items.Sum(i => i.SubTotal);

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult GenerateBilling(int prescriptionId)
        {
            var prescription = _context.Prescriptions.Include(p => p.Visit).Include(p => p.PrescriptionItems).FirstOrDefault(p => p.Id == prescriptionId);

            if (prescription == null) return NotFound();

            var totalAmount = prescription.PrescriptionItems.Sum(i => i.UnitPrice * i.Quantity);

            var billing = new Billing
            {
                PatientId = prescription.Visit.PatientId,
                PrescriptionId = prescription.Id,
                InvoiceDate = DateTime.Now,
                TotalAmount = totalAmount
            };

            _context.Billings.Add(billing);
            _context.SaveChanges();

            return RedirectToAction("Index", "Billing"); 
        }


    }
}
