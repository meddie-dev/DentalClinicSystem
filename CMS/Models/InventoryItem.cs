using System.ComponentModel.DataAnnotations;

namespace CMS.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        [Required]
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}
