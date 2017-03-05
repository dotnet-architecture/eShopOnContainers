using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using System;
using Xunit;

public class BuyerAggregateTest
{
    public BuyerAggregateTest()
    { }

    [Fact]
    public void Create_buyer_item_success()
    {
        //Arrange    
        var identity = new Guid().ToString();

        //Act 
        var fakeBuyerItem = new Buyer(identity);

        //Assert
        Assert.NotNull(fakeBuyerItem);
    }

    [Fact]
    public void Create_buyer_item_fail()
    {
        //Arrange    
        var identity = string.Empty;

        //Act - Assert
        Assert.Throws<ArgumentNullException>(() => new Buyer(identity));
    }

    [Fact]
    public void add_payment_success()
    {        
        //Arrange    
        var cardTypeId = 1;
        var alias = "fakeAlias";
        var cardNumber = "124";
        var securityNumber = "1234";
        var cardHolderName = "FakeHolderNAme";
        var expiration = DateTime.Now.AddYears(1);
        var identity = new Guid().ToString();
        var fakeBuyerItem = new Buyer(identity);

        //Act
        var result = fakeBuyerItem.AddPaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void create_payment_method_success()
    {
        //Arrange    
        var cardTypeId = 1;
        var alias = "fakeAlias";
        var cardNumber = "124";
        var securityNumber = "1234";
        var cardHolderName = "FakeHolderNAme";
        var expiration = DateTime.Now.AddYears(1);
        var fakePaymentMethod = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);

        //Act
        var result = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void create_payment_method_expiration_fail()
    {
        //Arrange    
        var cardTypeId = 1;
        var alias = "fakeAlias";
        var cardNumber = "124";
        var securityNumber = "1234";
        var cardHolderName = "FakeHolderNAme";
        var expiration = DateTime.Now.AddYears(-1);        

        //Act - Assert
        Assert.Throws<ArgumentException>(() => new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration));
    }

    [Fact]
    public void payment_method_isEqualTo()
    {
        //Arrange    
        var cardTypeId = 1;
        var alias = "fakeAlias";
        var cardNumber = "124";
        var securityNumber = "1234";
        var cardHolderName = "FakeHolderNAme";
        var expiration = DateTime.Now.AddYears(1);

        //Act
        var fakePaymentMethod = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);
        var result = fakePaymentMethod.IsEqualTo(cardTypeId, cardNumber, expiration);

        //Assert
        Assert.True(result);
    }
}