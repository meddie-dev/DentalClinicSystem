using CMS.Data;
using CMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers
{
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var billings = _context.Billings.Include(b => b.Patient).Include(b => b.Prescription).ToList();

            var payments = _context.Payments.ToList();

            ViewBag.Payments = payments;

            return View(billings);
        }


        public IActionResult Create(int billingId)
        {
            var billing = _context.Billings.Include(b => b.Patient).FirstOrDefault(b => b.Id == billingId);

            if (billing == null) return NotFound();

            var payment = new Payment
            {
                BillingId = billing.Id,
                AmountPaid = billing.TotalAmount,
                PaymentDate = DateTime.Now
            };

            return View(payment);
        }

        [HttpPost]
        public IActionResult Create(Payment payment)
        {
            var billing = _context.Billings.Find(payment.BillingId);
            if (billing == null) return NotFound();

            if (payment.AmountPaid < billing.TotalAmount)
            {
                ModelState.AddModelError("AmountPaid", "Amount paid is less than total bill.");
                return View(payment);
            }

            _context.Payments.Add(payment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Details(int billingId)
        {
            var billing = _context.Billings.Include(b => b.Patient).Include(b => b.Payments).FirstOrDefault(b => b.Id == billingId);

            if (billing == null)
                return NotFound();

            return View(billing);
        }

    }
}
