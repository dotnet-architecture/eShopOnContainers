using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TenantACustomisations.Database;
using TenantACustomisations.IntegrationEvents.Events;

namespace TenantACustomisations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedEventsController : ControllerBase
    {
        private readonly TenantAContext _context;
        private readonly ILogger<SavedEventsController> _logger;
        private readonly IEventBus _eventBus;

        private List<Type> types = new List<Type>()
        {
            typeof(OrderStatusChangedToAwaitingValidationIntegrationEvent)
        };

        public SavedEventsController(TenantAContext context, ILogger<SavedEventsController> logger, IEventBus eventBus)
        {
            _context = context;
            _logger = logger;
            _eventBus = eventBus;
        }

        // GET: api/SavedEvents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SavedEvent>>> GetSavedEvent(String orderId)
        {
            if (String.IsNullOrEmpty(orderId))
            {
                return await _context.SavedEvent.ToListAsync();
            }

            //Getting saved events
            var savedEvents = await _context.SavedEvent.ToListAsync();

            //Returning if list is empty
            if (savedEvents.Count == 0)
            {
                return NotFound();
            }
            List<IntegrationEvent> events = new List<IntegrationEvent>();
            
            //Converting events to actual type
            savedEvents.ForEach(e =>
            {
                var integrationEvent =JsonConvert.DeserializeObject(e.Content, GetEventTypeByName(e.EventName));
                IntegrationEvent evt = (IntegrationEvent)integrationEvent;
                events.Add(evt);
            });

            bool found = false;
            //Casting to class to check the orderId
            events.ForEach(e =>
            {
                if(e is OrderStatusChangedToAwaitingValidationIntegrationEvent)
                {
                    OrderStatusChangedToAwaitingValidationIntegrationEvent evt = (OrderStatusChangedToAwaitingValidationIntegrationEvent)e;
                    if (evt.OrderId == Int32.Parse(orderId))
                    {
                        found = true;
                    }
                }
            });

            if (!found)
            {
                return NotFound();
            }

            return savedEvents;
        }

        // PUT: api/SavedEvents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSavedEvent(string id, SavedEvent savedEvent)
        {
            if (id != savedEvent.SavedEventId)
            {
                return BadRequest();
            }

            _context.Entry(savedEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SavedEventExists(id))
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

        // POST: api/SavedEvents
        [HttpPost]
        public async Task<ActionResult<SavedEvent>> PostSavedEvent(SavedEvent savedEvent)
        {
            _context.SavedEvent.Add(savedEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSavedEvent", new {id = savedEvent.SavedEventId}, savedEvent);
        }

        // DELETE: api/SavedEvents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SavedEvent>> DeleteSavedEvent(string id)
        {
            var savedEvent = await _context.SavedEvent.FindAsync(id);
            if (savedEvent == null)
            {
                return NotFound();
            }

            var integrationEvent =
                JsonConvert.DeserializeObject(savedEvent.Content, GetEventTypeByName(savedEvent.EventName));
            IntegrationEvent evt = (IntegrationEvent) integrationEvent;
            try
            {
                _logger.LogInformation(
                    "----- Publishing integration event: {IntegrationEventId} from OrderStatusChangedToSubmittedIntegrationEventsController - ({@IntegrationEvent})",
                    evt.Id, evt);
                evt.CheckForCustomisation = false;
                _eventBus.Publish(evt);
                _context.SavedEvent.Remove(savedEvent);
                await _context.SaveChangesAsync();
                return savedEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "ERROR Publishing integration event: {IntegrationEventId} from OrderStatusChangedToSubmittedIntegrationEventsController",
                    evt.Id);

                throw;
            }


            _context.SavedEvent.Remove(savedEvent);
            await _context.SaveChangesAsync();

            return savedEvent;
        }

        private bool SavedEventExists(string id)
        {
            return _context.SavedEvent.Any(e => e.SavedEventId == id);
        }

        private Type GetEventTypeByName(string eventName) => types.SingleOrDefault(t => t.Name == eventName);
    }
}