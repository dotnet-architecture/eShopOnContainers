using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Application.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly OrderingContext _dbContext;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public TransactionBehaviour(OrderingContext dbContext,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(OrderingContext));
            _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentException(nameof(orderingIntegrationEventService));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse response = default(TResponse);

            try
            {
                var strategy = _dbContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () => 
                {
                    _logger.LogInformation($"Begin transaction {typeof(TRequest).Name}");

                    await _dbContext.BeginTransactionAsync();

                    response = await next();

                    await _dbContext.CommitTransactionAsync();

                    _logger.LogInformation($"Committed transaction {typeof(TRequest).Name}");

                    await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync();
                });

                return response;
            }
            catch (Exception)
            {
                _logger.LogInformation($"Rollback transaction executed {typeof(TRequest).Name}");

                _dbContext.RollbackTransaction();
                throw;
            }
        }
    }
}
