namespace CMS.ViewModels
{
    public class BillingDetailsViewModel
    {
        public int BillingId { get; set; }
        public string PatientName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public List<PrescriptionItemViewModel> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
}
