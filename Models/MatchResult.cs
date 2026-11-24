using AcuPay.Enums;
 
namespace AcuPay.Models
{
    public class MatchResult
    {
        public decimal VariancePercent { get; set; }
 
        public InvoiceStatus Status { get; set; }
 
        public string MatchNote { get; set; } = string. Empty;
    }
}