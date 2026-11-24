namespace AcuPay.Models
{
    public class PurchaseOrder
    {
        public int PoId { get; set; }
 
        public string PoNumber { get; set; } = string.Empty;
 
        public int VendorId { get; set; }
 
        public decimal PoAmount { get; set; }
 
        public DateTime PoDate { get; set; }
 
        public string Status { get; set; } = string.Empty;
    }
}