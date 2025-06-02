namespace CMS.ViewModels
{
    public class PrescriptionDetailsViewModel
    {
        public int PrescriptionId { get; set; }
        public int ExaminationId { get; set; }
        public string PatientName { get; set; }
        public string Instructions { get; set; }

        public List<PrescriptionItemViewModel> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public bool BillingExists { get; set; }

    }

    public class PrescriptionItemViewModel
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }

}
