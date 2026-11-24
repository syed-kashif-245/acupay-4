using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace AcuPay.Data
{
   public class DbConnectionFactory
    {
        private readonly IConfiguration _configuration;
 
        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
 
        public SqlConnection CreateConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection")!;
 
            return new SqlConnection(connectionString);
        }
    }
}
 