$(function () {

    $("a.placeorder").click(function (e) {
        e.preventDefault();

        var $this = $(this);
        var url = "/cart/PlaceOrder";

        $(".ajaxbg").show();

        $.post(url,
            {},
            function (data) {
                $(".ajaxbg span").text("Thank you. You will now be redirected to paypal.");
                setTimeout(function () {
                    $('form input[name = "submit"]').click();
                }, 2000);
            });
    });
});