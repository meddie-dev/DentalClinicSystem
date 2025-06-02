namespace CMS.Models
{
    public class InventoryLog
    {
        public int Id { get; set; }
        public int InventoryItemId { get; set; }
        public string ItemName { get; set; }
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
