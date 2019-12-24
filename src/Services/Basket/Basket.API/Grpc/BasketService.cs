using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Basket.API.IntegrationEvents.Events;
using System;
using Microsoft.eShopOnContainers.Services.Basket.API.Services;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.Services.Basket.API;

namespace GrpcBasket
{
    public class BasketService : Basket.BasketBase
    {
        private readonly IBasketRepository _repository;
        private readonly ILogger<BasketService> _logger;
        private readonly IIdentityService _identityService;
        private readonly IEventBus _eventBus;

        public BasketService(IBasketRepository repository, ILogger<BasketService> logger,
            IIdentityService identityService,
            IEventBus eventBus)
        {
            _repository = repository;
            _logger = logger;
            _identityService = identityService;
            _eventBus = eventBus;
        }

        [AllowAnonymous]
        public override async Task<CustomerBasketResponse> GetBasketById(BasketRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call from method {Method} for basket id {Id}", context.Method, request.Id);

            var data = await _repository.GetBasketAsync(request.Id);

            if (data != null)
            {
                context.Status = new Status(StatusCode.OK, $"Basket with id {request.Id} do exist");

                return MapToCustomerBasketResponse(data);
            }
            else
            {
                context.Status = new Status(StatusCode.NotFound, $"Basket with id {request.Id} do not exist");
            }

            return new CustomerBasketResponse();
        }

        public override async Task<CustomerBasketResponse> UpdateBasket(CustomerBasketRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Begin grpc call BasketService.UpdateBasketAsync for buyer id {Buyerid}", request.Buyerid);

            var customerBasket = MapToCustomerBasket(request);

            var response = await _repository.UpdateBasketAsync(customerBasket);

            var buyerName = _identityService.GetUserName();
            var integrationEvent = new UserAddedCatalogItemToBasketIntegrationEvent(buyerName, request.Items.Count);
            try
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", integrationEvent.Id, Program.AppName, integrationEvent);

                _eventBus.Publish(integrationEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", integrationEvent.Id, Program.AppName);

                throw;
            }

            if (response != null)
            {
                return MapToCustomerBasketResponse(response);
            }

            context.Status = new Status(StatusCode.NotFound, $"Basket with buyer id {request.Buyerid} do not exist");

            return null;
        }

        private CustomerBasketResponse MapToCustomerBasketResponse(CustomerBasket customerBasket)
        {
            var response = new CustomerBasketResponse
            {
                Buyerid = customerBasket.BuyerId
            };

            customerBasket.Items.ForEach(item => response.Items.Add(new BasketItemResponse
            {
                Id = item.Id,
                Oldunitprice = (double)item.OldUnitPrice,
                Pictureurl = item.PictureUrl,
                Productid = item.ProductId,
                Productname = item.ProductName,
                Quantity = item.Quantity,
                Unitprice = (double)item.UnitPrice
            }));

            return response;
        }

        private CustomerBasket MapToCustomerBasket(CustomerBasketRequest customerBasketRequest)
        {
            var response = new CustomerBasket
            {
                BuyerId = customerBasketRequest.Buyerid
            };

            customerBasketRequest.Items.ToList().ForEach(item => response.Items.Add(new BasketItem
            {
                Id = item.Id,
                OldUnitPrice = (decimal)item.Oldunitprice,
                PictureUrl = item.Pictureurl,
                ProductId = item.Productid,
                ProductName = item.Productname,
                Quantity = item.Quantity,
                UnitPrice = (decimal)item.Unitprice
            }));

            return response;
        }
    }
}
