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

namespace CMS.Controllers
{
    public class InventoryItemsController : Controller
    {
        private readonly AppDbContext _context;

        public InventoryItemsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        // GET: InventoryItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.InventoryItems.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        // GET: InventoryItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(m => m.Id == id);

            if (inventoryItem == null)
            {
                return NotFound();
            }

            return View(inventoryItem);
        }

        [Authorize(Roles = "Admin")]
        // GET: InventoryItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InventoryItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemName,Description,Quantity,LastUpdated")] InventoryItem inventoryItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventoryItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventoryItem);
        }

        [Authorize(Roles = "Admin")]
        // GET: InventoryItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryItem = await _context.InventoryItems.FindAsync(id);
            if (inventoryItem == null)
            {
                return NotFound();
            }
            return View(inventoryItem);
        }

        // POST: InventoryItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemName,Description,Quantity,LastUpdated")] InventoryItem inventoryItem)
        {
            if (id != inventoryItem.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingItem = await _context.InventoryItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                if (existingItem == null) return NotFound();

                try
                {
                    _context.Update(inventoryItem);
                    await _context.SaveChangesAsync();

                    var log = new InventoryLog
                    {
                        InventoryItemId = inventoryItem.Id,
                        ItemName = inventoryItem.ItemName,
                        OldQuantity = existingItem.Quantity,
                        NewQuantity = inventoryItem.Quantity,
                        UpdatedBy = User.Identity?.Name ?? "Unknown",
                        UpdatedAt = DateTime.Now
                    };

                    _context.InventoryLogs.Add(log);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryItemExists(inventoryItem.Id))
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
            return View(inventoryItem);
        }


        [Authorize(Roles = "Admin")]
        // GET: InventoryItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(m => m.Id == id);

            if (inventoryItem == null)
            {
                return NotFound();
            }

            return View(inventoryItem);
        }

        // POST: InventoryItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventoryItem = await _context.InventoryItems.FindAsync(id);
            if (inventoryItem != null)
            {
                _context.InventoryItems.Remove(inventoryItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryItemExists(int id)
        {
            return _context.InventoryItems.Any(e => e.Id == id);
        }
    }
}
