namespace CMS.ViewModels
{
    public class ExaminationIndexViewModel
    {
        public int ExaminationId { get; set; }
        public string PatientName { get; set; }
        public DateTime ExaminationDate { get; set; }

        public bool BillingExists { get; set; }
    }
}
