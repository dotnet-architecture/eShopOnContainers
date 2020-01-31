using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using TenantACustomisations.Database;
using TenantACustomisations.IntegrationEvents.Events;

namespace TenantACustomisations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusChangedToSubmittedIntegrationEventsController : ControllerBase
    {
        private readonly TenantAContext _context;
        private readonly IEventBus _eventBus;
        private readonly ILogger<OrderStatusChangedToSubmittedIntegrationEventsController> _logger;


        public OrderStatusChangedToSubmittedIntegrationEventsController(TenantAContext context, IEventBus eventBus, ILogger<OrderStatusChangedToSubmittedIntegrationEventsController> logger)
        {
            _context = context;
            _eventBus = eventBus;
            _logger = logger;
        }

        // GET: api/OrderStatusChangedToSubmittedIntegrationEvents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderStatusChangedToSubmittedIntegrationEvent>>> GetOrderStatusChangedToSubmittedIntegrationEvent(String orderId)
        {
            if (String.IsNullOrEmpty(orderId))
            {
                return await _context.OrderStatusChangedToSubmittedIntegrationEvent.ToListAsync();
            }
            else
            {
                var orderStatusChangedToSubmittedIntegrationEvent =  _context.OrderStatusChangedToSubmittedIntegrationEvent.Where(x => x.OrderId == Int32.Parse(orderId)).ToListAsync();

                return await orderStatusChangedToSubmittedIntegrationEvent;
            }
        }

        // GET: api/OrderStatusChangedToSubmittedIntegrationEvents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderStatusChangedToSubmittedIntegrationEvent>> GetOrderStatusChangedToSubmittedIntegrationEvent(Guid id)
        {
            var orderStatusChangedToSubmittedIntegrationEvent = await _context.OrderStatusChangedToSubmittedIntegrationEvent.FindAsync(id);

            if (orderStatusChangedToSubmittedIntegrationEvent == null)
            {
                return NotFound();
            }

            return orderStatusChangedToSubmittedIntegrationEvent;
        }

        // PUT: api/OrderStatusChangedToSubmittedIntegrationEvents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderStatusChangedToSubmittedIntegrationEvent(Guid id, OrderStatusChangedToSubmittedIntegrationEvent orderStatusChangedToSubmittedIntegrationEvent)
        {
            if (id != orderStatusChangedToSubmittedIntegrationEvent.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderStatusChangedToSubmittedIntegrationEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderStatusChangedToSubmittedIntegrationEventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/OrderStatusChangedToSubmittedIntegrationEvents
        [HttpPost]
        public async Task<ActionResult<OrderStatusChangedToSubmittedIntegrationEvent>> PostOrderStatusChangedToSubmittedIntegrationEvent(OrderStatusChangedToSubmittedIntegrationEvent orderStatusChangedToSubmittedIntegrationEvent)
        {
            _context.OrderStatusChangedToSubmittedIntegrationEvent.Add(orderStatusChangedToSubmittedIntegrationEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderStatusChangedToSubmittedIntegrationEvent", new { id = orderStatusChangedToSubmittedIntegrationEvent.Id }, orderStatusChangedToSubmittedIntegrationEvent);
        }

        // DELETE: api/OrderStatusChangedToSubmittedIntegrationEvents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<OrderStatusChangedToSubmittedIntegrationEvent>> DeleteOrderStatusChangedToSubmittedIntegrationEvent(Guid id)
        {
            var orderStatusChangedToSubmittedIntegrationEvent = await _context.OrderStatusChangedToSubmittedIntegrationEvent.FindAsync(id);
            if (orderStatusChangedToSubmittedIntegrationEvent == null)
            {
                return NotFound();
            }

            try
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from OrderStatusChangedToSubmittedIntegrationEventsController - ({@IntegrationEvent})", orderStatusChangedToSubmittedIntegrationEvent.Id, orderStatusChangedToSubmittedIntegrationEvent);
                orderStatusChangedToSubmittedIntegrationEvent.CheckForCustomisation = false;
                _eventBus.Publish(orderStatusChangedToSubmittedIntegrationEvent);
                _context.OrderStatusChangedToSubmittedIntegrationEvent.Remove(orderStatusChangedToSubmittedIntegrationEvent);
                await _context.SaveChangesAsync();
                return orderStatusChangedToSubmittedIntegrationEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from OrderStatusChangedToSubmittedIntegrationEventsController", orderStatusChangedToSubmittedIntegrationEvent.Id);

                throw;
            }
        }

        private bool OrderStatusChangedToSubmittedIntegrationEventExists(Guid id)
        {
            return _context.OrderStatusChangedToSubmittedIntegrationEvent.Any(e => e.Id == id);
        }
    }
}
