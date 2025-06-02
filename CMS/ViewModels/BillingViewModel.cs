namespace CMS.ViewModels
{
    public class BillingViewModel
    {
        public int BillingId { get; set; }
        public string PatientName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }

        public int DentalExaminationId { get; set; }
    }
}
