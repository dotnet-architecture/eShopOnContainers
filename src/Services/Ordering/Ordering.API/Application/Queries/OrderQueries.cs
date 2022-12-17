namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries;
    
public class OrderQueries
    : IOrderQueries
{
    private string _connectionString = string.Empty;

    public OrderQueries(string constr)
    {
        _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
    }


    public async Task<Order> GetOrderAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var result = await connection.QueryAsync<dynamic>(
            @"select o.[Id] as ordernumber,o.OrderDate as date, o.Description as description,
                    o.Address_City as city, o.Address_Country as country, o.Address_State as state, o.Address_Street as street, o.Address_ZipCode as zipcode,
                    os.Name as status, 
                    oi.ProductName as productname, oi.Units as units, oi.UnitPrice as unitprice, oi.PictureUrl as pictureurl,
                    o.DiscountCode as coupon,
                    o.Discount as discount
                    FROM ordering.Orders o
                    LEFT JOIN ordering.Orderitems oi ON o.Id = oi.orderid 
                    LEFT JOIN ordering.orderstatus os on o.OrderStatusId = os.Id
                    WHERE o.Id=@id"
                , new { id }
            );

        if (result.AsList().Count == 0)
            throw new KeyNotFoundException();

        return MapOrderItems(result);
    }

    public async Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(Guid userId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        return await connection.QueryAsync<OrderSummary>(@"
                    with OrderTotal as (
                        select 
                            OrderId, 
                            sum(Units * UnitPrice) subtotal
                        from ordering.orderitems
                        group by OrderId)
                    select
                        o.Id ordernumber,
                        o.OrderDate date,
                        os.Name status,
                        case
                            when ot.subtotal > isnull(o.Discount, 0) 
                                then ot.subtotal - isnull(o.Discount, 0)
                            else 0
                        end total
                    from ordering.orders o
                    join OrderTotal ot on ot.OrderId = o.Id
                    join ordering.orderstatus os on os.Id = o.OrderStatusId
                    join ordering.buyers b on b.Id = o.BuyerId", 
                    new { userId });
    }

    public async Task<IEnumerable<CardType>> GetCardTypesAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        return await connection.QueryAsync<CardType>("SELECT * FROM ordering.cardtypes");
    }

    private Order MapOrderItems(dynamic result)
    {
        var order = new Order
        {
            ordernumber = result[0].ordernumber,
            date = result[0].date,
            status = result[0].status,
            description = result[0].description,
            street = result[0].street,
            city = result[0].city,
            zipcode = result[0].zipcode,
            country = result[0].country,
            orderitems = new List<Orderitem>(),
            subtotal = 0,
            coupon = result[0].coupon,
            discount = result[0].discount ?? 0m,
            total = 0
        };

        foreach (dynamic item in result)
        {
            var orderitem = new Orderitem
            {
                productname = item.productname,
                units = item.units,
                unitprice = (double)item.unitprice,
                pictureurl = item.pictureurl
            };

            order.subtotal += item.units * item.unitprice;
            order.orderitems.Add(orderitem);
        }

        order.total = order.discount < order.subtotal
              ? order.subtotal - order.discount
              : 0;

        return order;
    }
}
