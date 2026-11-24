 
using AcuPay.Models;
using AcuPay.Enums;
using AcuPay.Exceptions;
 
namespace AcuPay.Services
{
    public class InvoiceApprovalService
    {
        private readonly decimal _autoApproveLimit;
        private readonly int _minReasonLength;
 
        public InvoiceApprovalService(
            decimal autoApproveLimit,
            int minReasonLength)
        {
            _autoApproveLimit = autoApproveLimit;
            _minReasonLength = minReasonLength;
        }
 
        // AUTOMATIC APPROVAL
        public void AutoApprove(Invoice invoice)
        {
            // Only Matched invoices can be auto-approved
            if (invoice.Status != InvoiceStatus.Matched)
            {
                return;
            }
 
            // Check the auto-approval limit
            if (invoice.InvoiceAmount <= _autoApproveLimit)
            {
                invoice.Status = InvoiceStatus.Approved;
 
                invoice.DecidedBy = "SYSTEM";
 
                invoice.DecidedAtUtc = DateTime.UtcNow;
 
                invoice.MatchNote = "AUTO_APPROVED";
            }
        }
 
        // MANUAL APPROVAL
        public void Approve(
            Invoice invoice,
            User user,
            string? reason = null)
        {
            ValidateUser(user);
 
            ValidateSelfApproval(invoice, user);
 
            ValidateInvoiceStatus(invoice);
 
            // Exception invoices require Finance approval
            if (invoice.Status == InvoiceStatus.Exception)
            {
                if (user.Role != UserRole.Finance ||
                    string.IsNullOrWhiteSpace(reason) ||
                    reason.Trim().Length < _minReasonLength)
                {
                    throw new BusinessRuleException(
                        "OVERRIDE_REQUIRES_FINANCE",
                        "Exception invoices require Finance approval and a valid reason.");
                }
            }
 
            invoice.Status = InvoiceStatus.Approved;
 
            invoice.DecidedBy = user.Username;
 
            invoice.DecidedAtUtc = DateTime.UtcNow;
 
            invoice.DecisionReason = reason?.Trim();
        }
 
        // MANUAL REJECTION
        public void Reject(
            Invoice invoice,
            User user,
            string reason)
        {
            ValidateUser(user);
 
            ValidateSelfApproval(invoice, user);
 
            ValidateInvoiceStatus(invoice);
 
            // Rejection reason validation
            if (string.IsNullOrWhiteSpace(reason) ||
                reason.Trim().Length < _minReasonLength)
            {
                throw new BusinessRuleException(
                    "REASON_REQUIRED",
                    "Rejection reason must contain at least the minimum required characters.");
            }
 
            invoice.Status = InvoiceStatus.Rejected;
 
            invoice.DecidedBy = user.Username;
 
            invoice.DecidedAtUtc = DateTime.UtcNow;
 
            invoice.DecisionReason = reason.Trim();
        }
 
        // CHECK USER ROLE
        private void ValidateUser(User user)
        {
            if (user.Role != UserRole.Approver &&
                user.Role != UserRole.Finance)
            {
                throw new BusinessRuleException(
                    "FORBIDDEN_ROLE",
                    "Only Approver and Finance users can approve or reject invoices.");
            }
        }
 
        // PREVENT SELF-APPROVAL
        private void ValidateSelfApproval(Invoice invoice, User user)
        {
            if (string.Equals(
                invoice.CreatedBy,
                user.Username,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessRuleException(
                    "SELF_APPROVAL_BLOCKED",
                    "You cannot decide an invoice you created.");
            }
        }
 
        // CHECK INVOICE STATUS
        private void ValidateInvoiceStatus(Invoice invoice)
        {
            if (invoice.Status != InvoiceStatus.Matched &&
                invoice.Status != InvoiceStatus.Exception)
            {
                throw new BusinessRuleException(
                    "INVALID_STATE",
                    "Only Matched and Exception invoices can be decided.");
            }
        }
    }
}
 
 