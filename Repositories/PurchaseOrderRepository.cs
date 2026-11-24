using System;
using System.Threading.Tasks;
using AcuPay.Data;
using AcuPay.Models;
using Microsoft.Data.SqlClient;
 
namespace AcuPay.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly DbConnectionFactory _factory;
 
        public PurchaseOrderRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }
 
        public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int poId)
        {
            using SqlConnection connection = _factory.CreateConnection();
 
            await connection.OpenAsync();
 
            string query = @"
                SELECT PoId, PoNumber, VendorId, PoAmount, PoDate, Status
                FROM PurchaseOrders
                WHERE PoId = @PoId;
            ";
 
            using SqlCommand command = new SqlCommand(query, connection);
 
            command.Parameters.AddWithValue("@PoId", poId);
 
            using SqlDataReader reader = await command.ExecuteReaderAsync();
 
            if (await reader.ReadAsync())
            {
                PurchaseOrder purchaseOrder = new PurchaseOrder
                {
                    PoId = Convert.ToInt32(reader["PoId"]),
                    PoNumber = reader["PoNumber"].ToString()!,
                    VendorId = Convert.ToInt32(reader["VendorId"]),
                    PoAmount = Convert.ToDecimal(reader["PoAmount"]),
                    PoDate = Convert.ToDateTime(reader["PoDate"]),
                    Status = reader["Status"].ToString()!
                };
 
                return purchaseOrder;
            }
 
            return null;
        }
    }
}
