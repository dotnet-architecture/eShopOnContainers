namespace Ordering.API.Application.Commands;

public class CouponConfirmedCommand : IRequest<bool>
{

    [DataMember]
    public int OrderNumber { get; private set; }

    [DataMember]
    public int Discount { get; private set; }

    public CouponConfirmedCommand(int orderNumber, int discount)
    {
        OrderNumber = orderNumber;
        Discount = discount;
    }
}
