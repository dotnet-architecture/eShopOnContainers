// Write your Javascript code.
$('#item_Quantity').change(function () {
    $('.cart-refresh-button').removeClass('is-disabled');
});

function checkoutCart(){
    $('#cartForm').attr('action', '/Cart?option=checkout');
    $('#cartForm').submit();
}

function refreshCart() {
    $('#cartForm').attr('action','/Cart?option=refresh');
    $('#cartForm').submit();
}
