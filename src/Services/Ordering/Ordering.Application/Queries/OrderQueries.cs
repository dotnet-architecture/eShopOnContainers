namespace Microsoft.eShopOnContainers.Services.Ordering.Application.Queries
{
    using Dapper;
    using Microsoft.Extensions.Configuration;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    public class OrderQueries
        :IOrderQueries
    {
        private string _connectionString = string.Empty;

        public OrderQueries(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString"];
        }

        public async Task<dynamic> GetOrder(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<dynamic>("SELECT * FROM ordering.Orders where Id=@id",new { id });
            }
        }

        public async Task<dynamic> GetPendingOrders()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<dynamic>("SELECT * FROM ordering.Orders");
            }
        }
    }
}
