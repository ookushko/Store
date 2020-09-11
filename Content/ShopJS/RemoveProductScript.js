$(function () {

    $("a.removeproduct").click(function (e) {
        e.preventDefault();

        var $this = $(this);
        var productId = $(this).data("id");
        var url = "/cart/RemoveProduct";

        $.get(url, { productId: productId }, function (data) {
            location.reload();
        });
    });
});