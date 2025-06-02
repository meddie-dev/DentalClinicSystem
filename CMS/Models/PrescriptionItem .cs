using Microsoft.EntityFrameworkCore;

namespace CMS.Models
{
    public class PrescriptionItem
    {
        public int Id { get; set; }

        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }

        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }

        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        [Precision(18, 2)]
        public decimal Subtotal => UnitPrice * Quantity;
    }

}
