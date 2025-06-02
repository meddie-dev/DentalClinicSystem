using System.ComponentModel.DataAnnotations;

namespace CMS.ViewModels
{
    public class PrescriptionCreateViewModel
    {
        public int VisitId { get; set; }
        public int DentalExaminationId { get; set; }
        public string Instructions { get; set; }

        public List<InventoryItemSelection> SelectedItems { get; set; } = new List<InventoryItemSelection>(); 
    }

    public class InventoryItemSelection
    {
        public int InventoryItemId { get; set; }
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
       
        public bool Selected { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to 1.")]
        public int? Quantity { get; set; } // Nullable

    }



}
