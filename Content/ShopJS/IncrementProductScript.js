$(function () {

    $("a.incproduct").click(function (e) {
        e.preventDefault();

        var productId = $(this).data("id");
        var url = "/cart/IncrementProduct";

        $.getJSON(url, { productId: productId }, function (data) {
            $("td.qty" + productId).html(data.qty);

            var price = data.qty * data.price;
            var priceHtml = price.toFixed(2) + "$";

            $("td.total" + productId).html(priceHtml);

            var gt = parseFloat($("td.grandtotal span").text());
            var grandtotal = (gt + data.price).toFixed(2);

            $("td.grandtotal span").text(grandtotal);

        }).done(function (data) {
            var url2 = "/cart/PaypalPartial";

            $.get(url2,
                {},
                function (data) {
                    $("div.paypal").html(data);
                });
        });
    });
});
