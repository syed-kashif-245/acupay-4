using System.Collections.Generic;
using System.Threading.Tasks;
using AcuPay.Models;
 
namespace AcuPay.Repositories
{
    public interface IInvoiceRepository
    {
        Task<int> AddInvoiceAsync(Invoice invoice);
 
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
 
        Task<List<Invoice>> GetAllInvoicesAsync();
 
        Task UpdateInvoiceAsync(Invoice invoice);
    }
}