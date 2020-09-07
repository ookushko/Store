$(function () {

    /* Confirm page deletion */
    $("a.delete").click(function () {
        if (!confirm("Confirm page deletion")) return false;
    });

    /* Sorting script */
    $("table#pages tbody").sortable({
        items: "tr:not(.headline)",
        placeholder: "ui-state-highlight",
        update: function () {
            var ids = $("table#pages tbody").sortable("serialize");
            var url = "/Admin/Pages/ReorderPages";

            $.post(url, ids, function (data) {

            });
        }
    });
});