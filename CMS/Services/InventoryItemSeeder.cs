using CMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CMS.Data
{
    public static class InventoryItemSeeder
    {
        public static void SeedInventoryItems(AppDbContext context)
        {
            if (!context.InventoryItems.Any())
            {
                var items = new[]
                {
                    new InventoryItem { ItemName = "Amoxicillin", Description = "Antibiotic used for dental infections.", Quantity = 200, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Mefenamic Acid", Description = "NSAID for dental pain and inflammation.", Quantity = 300, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Ibuprofen", Description = "Pain reliever and anti-inflammatory.", Quantity = 500, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Paracetamol", Description = "Analgesic and antipyretic used in dentistry.", Quantity = 600, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Metronidazole", Description = "Antibiotic for anaerobic bacterial infections.", Quantity = 150, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Clindamycin", Description = "Alternative antibiotic for penicillin-allergic patients.", Quantity = 100, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Ketorolac", Description = "Powerful NSAID for post-operative pain.", Quantity = 120, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Chlorhexidine Mouthwash", Description = "Antiseptic rinse used after dental procedures.", Quantity = 250, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Hydrogen Peroxide 3%", Description = "Used as a mild antiseptic for oral rinses.", Quantity = 300, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Lidocaine Gel", Description = "Topical anesthetic for gum and soft tissue.", Quantity = 400, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Benzocaine Oral Gel", Description = "Local anesthetic for temporary pain relief.", Quantity = 200, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Articaine with Epinephrine", Description = "Injectable local anesthetic for dental procedures.", Quantity = 180, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Fluoride Varnish", Description = "Applied topically for cavity prevention.", Quantity = 160, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Calcium Hydroxide Paste", Description = "Used for pulp capping and root canal dressing.", Quantity = 100, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Sodium Hypochlorite", Description = "Used as a root canal irrigant.", Quantity = 75, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Zinc Oxide Eugenol", Description = "Used as a temporary filling material.", Quantity = 90, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Silver Diamine Fluoride", Description = "Used for caries arrest and sensitivity treatment.", Quantity = 60, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Prednisolone", Description = "Steroid used for inflammation management.", Quantity = 100, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Epinephrine Ampoules", Description = "Used in emergencies such as anaphylaxis.", Quantity = 30, LastUpdated = DateTime.Now },
                    new InventoryItem { ItemName = "Diazepam Tablets", Description = "Used for dental anxiety or muscle relaxation.", Quantity = 50, LastUpdated = DateTime.Now },

                };

                context.InventoryItems.AddRange(items);
                context.SaveChanges();
            }
        }
    }
}
