using System.Threading.Tasks;
using AcuPay.Models;
 
namespace AcuPay.Repositories
{
    public interface IPurchaseOrderRepository
    {
        Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int poId);
    }
}