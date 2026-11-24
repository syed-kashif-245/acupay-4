 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcuPay.Data;
using AcuPay.Enums;
using AcuPay.Models;
using Microsoft.Data.SqlClient;
 
namespace AcuPay.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DbConnectionFactory _factory;
 
        public InvoiceRepository(DbConnectionFactory factory)
        {
            _factory = factory;
        }
 
        public async Task<int> AddInvoiceAsync(Invoice invoice)
        {
            using SqlConnection connection = _factory.CreateConnection();
 
            await connection.OpenAsync();
 
            string query = @"
                INSERT INTO Invoices
                (InvoiceNumber, VendorId, PoId, InvoiceAmount,
                 Status, MatchNote, CreatedBy, InvoiceDate)
                VALUES
                (@InvoiceNumber, @VendorId, @PoId, @InvoiceAmount,
                 @Status, @MatchNote, @CreatedBy, @InvoiceDate);
 
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";
 
            using SqlCommand command = new SqlCommand(query, connection);
 
            command.Parameters.AddWithValue("@InvoiceNumber", invoice.InvoiceNumber);
            command.Parameters.AddWithValue("@VendorId", invoice.VendorId);
 
            command.Parameters.AddWithValue("@PoId",
                (object?)invoice.PoId ?? DBNull.Value);
 
            command.Parameters.AddWithValue("@InvoiceAmount", invoice.InvoiceAmount);
            command.Parameters.AddWithValue("@Status", invoice.Status.ToString());
 
            command.Parameters.AddWithValue("@MatchNote",
                (object?)invoice.MatchNote ?? DBNull.Value);
 
            command.Parameters.AddWithValue("@CreatedBy", invoice.CreatedBy);
            command.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);
 
            int invoiceId = Convert.ToInt32(
                await command.ExecuteScalarAsync());
 
            return invoiceId;
        }
 
       
        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
            using SqlConnection connection = _factory.CreateConnection();
 
            await connection.OpenAsync();
 
            string query = @"
                SELECT *
                FROM Invoices
                WHERE InvoiceId = @InvoiceId;
            ";
 
            using SqlCommand command = new SqlCommand(query, connection);
 
            command.Parameters.AddWithValue("@InvoiceId", invoiceId);
 
            using SqlDataReader reader = await command.ExecuteReaderAsync();
 
            if (await reader.ReadAsync())
            {
                Invoice invoice = new Invoice
                {
                    InvoiceId = Convert.ToInt32(reader["InvoiceId"]),
                    InvoiceNumber = reader["InvoiceNumber"].ToString()!,
                    VendorId = Convert.ToInt32(reader["VendorId"]),
 
                    PoId = reader["PoId"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(reader["PoId"]),
 
                    InvoiceAmount = Convert.ToDecimal(reader["InvoiceAmount"]),
 
                    Status = Enum.Parse<InvoiceStatus>(
                        reader["Status"].ToString()!),
 
                    MatchNote = reader["MatchNote"] == DBNull.Value
                        ? null
                        : reader["MatchNote"].ToString(),
 
                    CreatedBy = reader["CreatedBy"].ToString()!,
 
                    InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
 
                    CreatedAtUtc = Convert.ToDateTime(reader["CreatedAtUtc"]),
 
                    DecidedBy = reader["DecidedBy"] == DBNull.Value
                        ? null
                        : reader["DecidedBy"].ToString(),
 
                    DecidedAtUtc = reader["DecidedAtUtc"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["DecidedAtUtc"]),
 
                    DecisionReason = reader["DecisionReason"] == DBNull.Value
                        ? null
                        : reader["DecisionReason"].ToString()
                };
 
                return invoice;
            }
 
            return null;
        }
 
 
       
        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            List<Invoice> invoices = new List<Invoice>();
 
            using SqlConnection connection = _factory.CreateConnection();
 
            await connection.OpenAsync();
 
            string query = "SELECT * FROM Invoices";
 
            using SqlCommand command = new SqlCommand(query, connection);
 
            using SqlDataReader reader = await command.ExecuteReaderAsync();
 
            while (await reader.ReadAsync())
            {
                Invoice invoice = new Invoice
                {
                    InvoiceId = Convert.ToInt32(reader["InvoiceId"]),
 
                    InvoiceNumber = reader["InvoiceNumber"].ToString()!,
 
                    VendorId = Convert.ToInt32(reader["VendorId"]),
 
                    PoId = reader["PoId"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(reader["PoId"]),
 
                    InvoiceAmount = Convert.ToDecimal(reader["InvoiceAmount"]),
 
                    Status = Enum.Parse<InvoiceStatus>(
                        reader["Status"].ToString()!),
 
                    MatchNote = reader["MatchNote"] == DBNull.Value
                        ? null
                        : reader["MatchNote"].ToString(),
 
                    CreatedBy = reader["CreatedBy"].ToString()!,
 
                    InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
 
                    CreatedAtUtc = Convert.ToDateTime(reader["CreatedAtUtc"]),
 
                    DecidedBy = reader["DecidedBy"] == DBNull.Value
                        ? null
                        : reader["DecidedBy"].ToString(),
 
                    DecidedAtUtc = reader["DecidedAtUtc"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["DecidedAtUtc"]),
 
                    DecisionReason = reader["DecisionReason"] == DBNull.Value
                        ? null
                        : reader["DecisionReason"].ToString()
                };
 
                invoices.Add(invoice);
            }
 
            return invoices;
        }
 
 
        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            using SqlConnection connection = _factory.CreateConnection();
            await connection.OpenAsync();
 
            string query = @"
                UPDATE Invoices
                SET
                    Status = @Status,
                    MatchNote = @MatchNote,
                    DecidedBy = @DecidedBy,
                    DecidedAtUtc = @DecidedAtUtc,
                    DecisionReason = @DecisionReason
                WHERE InvoiceId = @InvoiceId;
            ";
 
            using SqlCommand command = new SqlCommand(query, connection);
 
            command.Parameters.AddWithValue("@Status", invoice.Status.ToString());
 
            command.Parameters.AddWithValue(
                "@MatchNote",
                (object?)invoice.MatchNote ?? DBNull.Value
            );
 
            command.Parameters.AddWithValue(
                "@DecidedBy",
                (object?)invoice.DecidedBy ?? DBNull.Value
            );
 
            command.Parameters.AddWithValue(
                "@DecidedAtUtc",
                (object?)invoice.DecidedAtUtc ?? DBNull.Value
            );
 
            command.Parameters.AddWithValue(
                "@DecisionReason",
                (object?)invoice.DecisionReason ?? DBNull.Value
            );
 
            command.Parameters.AddWithValue("@InvoiceId", invoice.InvoiceId);
 
            int rowsAffected = await command.ExecuteNonQueryAsync();
 
            if (rowsAffected == 0)
            {
                throw new KeyNotFoundException(
                    $"Invoice with ID {invoice.InvoiceId} was not found."
                );
            }
        }
    }
}
 
 