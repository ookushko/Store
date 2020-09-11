$(function () {

    $("a.decproduct").click(function (e) {
        e.preventDefault();

        var $this = $(this);
        var productId = $(this).data("id");
        var url = "/cart/DecrementProduct";

        $.getJSON(url, { productId: productId }, function (data) {

            if (data.qty == 0) {
                $this.parent().fadeOut("fast", function () {
                    location.reload();
                });
            }
            else {
                $("td.qty" + productId).html(data.qty);

                var price = data.qty * data.price;
                var priceHtml = price.toFixed(2) + "$";

                $("td.total" + productId).html(priceHtml);

                var gt = parseFloat($("td.grandtotal span").text());
                var grandtotal = (gt - data.price).toFixed(2);

                $("td.grandtotal span").text(grandtotal);
            }
        });
    });
});