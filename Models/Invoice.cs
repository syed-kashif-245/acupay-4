 
using AcuPay.Enums;
 
namespace AcuPay.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
 
        public string InvoiceNumber { get; set; } = string.Empty;
 
        public int VendorId { get; set; }
 
        public int? PoId { get; set; }
 
        public decimal InvoiceAmount { get; set; }
 
        public DateTime InvoiceDate { get; set; }
 
        public InvoiceStatus Status { get; set; }
 
        public string? MatchNote { get; set; }
 
        public string CreatedBy { get; set; } = string.Empty;
 
        public DateTime CreatedAtUtc { get; set; }
 
        public string? DecidedBy { get; set; }
 
        public DateTime? DecidedAtUtc { get; set; }
 
        public string? DecisionReason { get; set; }
    }
}
 
 