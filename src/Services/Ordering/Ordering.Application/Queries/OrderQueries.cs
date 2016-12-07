namespace Microsoft.eShopOnContainers.Services.Ordering.Application.Queries
{
    using Dapper;
    using Microsoft.Extensions.Configuration;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using System;

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

        public async Task<dynamic> GetOrders()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<dynamic>(@"SELECT o.[Id] as ordernumber,o.[OrderDate] as [date],os.[Name] as [status],SUM(oi.units*oi.unitprice) as total
                     FROM [ordering].[Orders] o
                     LEFT JOIN[ordering].[orderitems] oi ON  o.Id = oi.orderid 
                     LEFT JOIN[ordering].[orderstatus] os on o.StatusId = os.Id
                     GROUP BY o.[Id], o.[OrderDate], os.[Name]");
            }
        }

        public async Task<dynamic> GetCardTypes()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<dynamic>("SELECT * FROM ordering.cardtypes");
            }
        }
    }
}
