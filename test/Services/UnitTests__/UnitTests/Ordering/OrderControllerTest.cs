using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using System.Threading.Tasks;
using Moq;
using MediatR;

namespace UnitTests
{
    public class OrderControllerTest
    {
        private readonly Mock<IMediator> _mock;

        public OrderControllerTest()
        {
            //config mock;
            _mock = new Mock<IMediator>();

            
        }

        [Fact]
        public async Task AddOrder_ReturnsBadRequestResult_WhenPersitenceOperationFails()
        {
            //Add order:
            var orderRequest = new object() as IAsyncRequest<bool>;
            _mock.Setup(mediator => mediator.SendAsync(orderRequest))
                .Returns(Task.FromResult(false));
            
            // Arrange
            var controller = new OrdersController(mockRepo.Object);
            controller.ModelState.AddModelError("SessionName", "Required");
            var newSession = new HomeController.NewSessionModel();

            // Act
            var result = await controller.Index(newSession);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }


        // Implement Fake method for mock. 
        private MediatorMockForAddOrder()
        {

        }
    }
}
