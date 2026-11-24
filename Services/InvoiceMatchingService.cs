 
using System;
using System.Threading.Tasks;
using AcuPay.Enums;
using AcuPay.Models;
using AcuPay.Repositories;
 
namespace AcuPay.Services
{
    public class InvoiceMatchingService
    {
        private readonly decimal _priceTolerancePercent;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
 
        public InvoiceMatchingService(
            decimal priceTolerancePercent,
            IInvoiceRepository invoiceRepository,
            IPurchaseOrderRepository purchaseOrderRepository)
        {
            _priceTolerancePercent = priceTolerancePercent;
            _invoiceRepository = invoiceRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
        }
 
        // Existing matching logic
        public MatchResult MatchInvoice(
            Invoice invoice,
            PurchaseOrder? purchaseOrder)
        {
            // 1. Check whether PO exists
            if (invoice.PoId == null)
            {
                return new MatchResult
                {
                    VariancePercent = 0,
                    Status = InvoiceStatus.Exception,
                    MatchNote = "NO_PO"
                };
            }
 
            if (purchaseOrder == null)
            {
                throw new InvalidOperationException(
                    "Purchase order was not found.");
            }
 
            // 2. Check vendor mismatch
            if (invoice.VendorId != purchaseOrder.VendorId)
            {
                return new MatchResult
                {
                    VariancePercent = 0,
                    Status = InvoiceStatus.Exception,
                    MatchNote = "VENDOR_MISMATCH"
                };
            }
 
            // 3. Check whether PO is closed
            if (purchaseOrder.Status.Equals(
                "Closed", StringComparison.OrdinalIgnoreCase))
            {
                return new MatchResult
                {
                    VariancePercent = 0,
                    Status = InvoiceStatus.Exception,
                    MatchNote = "PO_CLOSED"
                };
            }
 
            // Prevent division by zero
            if (purchaseOrder.PoAmount <= 0)
            {
                throw new InvalidOperationException(
                    "Purchase order amount must be greater than zero.");
            }
 
            // 4. Calculate variance
            decimal variance =
                ((invoice.InvoiceAmount - purchaseOrder.PoAmount)
                / purchaseOrder.PoAmount) * 100;
 
            variance = Math.Round(variance, 2);
 
            // 5. Check whether variance exceeds tolerance
            if (variance > _priceTolerancePercent)
            {
                return new MatchResult
                {
                    VariancePercent = variance,
                    Status = InvoiceStatus.Exception,
                    MatchNote = "OVER_PO_AMOUNT"
                };
            }
 
            // 6. Under-billing is acceptable
            if (variance < -_priceTolerancePercent)
            {
                return new MatchResult
                {
                    VariancePercent = variance,
                    Status = InvoiceStatus.Matched,
                    MatchNote = "UNDER_PO_AMOUNT"
                };
            }
 
            // 7. Within tolerance
            return new MatchResult
            {
                VariancePercent = variance,
                Status = InvoiceStatus.Matched,
                MatchNote = "WITHIN_TOLERANCE"
            };
        }
 
        // New database-connected matching method
        public async Task<MatchResult> MatchInvoiceAsync(int invoiceId)
        {
            // 1. Get invoice from database
            Invoice? invoice =
                await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
 
            if (invoice == null)
            {
                throw new InvalidOperationException(
                    $"Invoice with ID {invoiceId} was not found.");
            }
 
            // 2. Get purchase order from database
            PurchaseOrder? purchaseOrder = null;
 
            if (invoice.PoId.HasValue)
            {
                purchaseOrder =
                    await _purchaseOrderRepository
                    .GetPurchaseOrderByIdAsync(invoice.PoId.Value);
 
                if (purchaseOrder == null)
                {
                    throw new InvalidOperationException(
                        $"Purchase order with ID {invoice.PoId.Value} was not found.");
                }
            }
 
            // 3. Perform matching
            MatchResult result = MatchInvoice(invoice, purchaseOrder);
 
            // 4. Update invoice details
            invoice.Status = result.Status;
            invoice.MatchNote = result.MatchNote;
 
            // 5. Save updated invoice in database
            await _invoiceRepository.UpdateInvoiceAsync(invoice);
 
            return result;
        }
    }
}
 
 