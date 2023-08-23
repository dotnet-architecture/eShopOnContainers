using CardType = Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries.CardType;
using Order = Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries.Order;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IOrderQueries _orderQueries;
    private readonly IIdentityService _identityService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IMediator mediator,
        IOrderQueries orderQueries,
        IIdentityService identityService,
        ILogger<OrdersController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Route("cancel")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelOrderAsync([FromBody] CancelOrderCommand command, [FromHeader(Name = "x-requestid")] string requestId)
    {
        bool commandResult = false;

        if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
        {
            var requestCancelOrder = new IdentifiedCommand<CancelOrderCommand, bool>(command, guid);

            _logger.LogInformation(
                "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                requestCancelOrder.GetGenericTypeName(),
                nameof(requestCancelOrder.Command.OrderNumber),
                requestCancelOrder.Command.OrderNumber,
                requestCancelOrder);

            commandResult = await _mediator.Send(requestCancelOrder);
        }

        if (!commandResult)
        {
            return BadRequest();
        }

        return Ok();
    }

    [Route("ship")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ShipOrderAsync([FromBody] ShipOrderCommand command, [FromHeader(Name = "x-requestid")] string requestId)
    {
        bool commandResult = false;

        if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
        {
            var requestShipOrder = new IdentifiedCommand<ShipOrderCommand, bool>(command, guid);

            _logger.LogInformation(
                "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                requestShipOrder.GetGenericTypeName(),
                nameof(requestShipOrder.Command.OrderNumber),
                requestShipOrder.Command.OrderNumber,
                requestShipOrder);

            commandResult = await _mediator.Send(requestShipOrder);
        }

        if (!commandResult)
        {
            return BadRequest();
        }

        return Ok();
    }

    [Route("{orderId:int}")]
    [HttpGet]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Order>> GetOrderAsync(int orderId)
    {
        try
        {
            //Todo: It's good idea to take advantage of GetOrderByIdQuery and handle by GetCustomerByIdQueryHandler
            //var order customer = await _mediator.Send(new GetOrderByIdQuery(orderId));
            var order = await _orderQueries.GetOrderAsync(orderId);

            return order;
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderSummary>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderSummary>>> GetOrdersAsync()
    {
        var userid = _identityService.GetUserIdentity();
        var orders = await _orderQueries.GetOrdersFromUserAsync(Guid.Parse(userid));

        return Ok(orders);
    }

    [Route("cardtypes")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CardType>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CardType>>> GetCardTypesAsync()
    {
        var cardTypes = await _orderQueries.GetCardTypesAsync();

        return Ok(cardTypes);
    }

    [Route("draft")]
    [HttpPost]
    public async Task<ActionResult<OrderDraftDTO>> CreateOrderDraftFromBasketDataAsync([FromBody] CreateOrderDraftCommand createOrderDraftCommand)
    {
        _logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            createOrderDraftCommand.GetGenericTypeName(),
            nameof(createOrderDraftCommand.BuyerId),
            createOrderDraftCommand.BuyerId,
            createOrderDraftCommand);

        return await _mediator.Send(createOrderDraftCommand);
    }
}
