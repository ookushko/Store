$(function () {

    /* Add new category JS script */
    var newCategoryA = $("a#newcategorya"); /*Класс линка добавления*/
    var newCategoryTextInput = $("#newcategoryname"); /*Класс текстового поля ввода*/
    var ajaxText = $("span.ajax-text"); /*Класс картинки загрузки*/
    var table = $("table#pages tbody"); /*Класс таблицы вывода*/

    /* Функция на отлов нажатия Enter */
    newCategoryTextInput.keyup(function (e) {
        if (e.keyCode == 13) {
            newCategoryA.click();
        }
    });

    /* Функция Click */
    newCategoryA.click(function (e) {
        e.preventDefault();

        var categoryName = newCategoryTextInput.val();

        if (categoryName.length < 3) {
            alert("Category name must be at least 3 characters long.");
            return false;
        }

        ajaxText.show();

        var url = "/admin/shop/AddNewCategory";

        $.post(url, { categoryName: categoryName }, function (data) {
            var response = data.trim();

            if (response == "titletaken") {
                ajaxText.html("<span class='alert alert-danger'>That title is taken.</span>");
                setTimeout(function () {
                    ajaxText.fadeOut("fast", function () {
                        ajaxText.html("<img src='/Content/img/ajax-loader.gif' height='50' />");
                    });
                }, 2000);
                return false;
            }
            else {
                if (!$("table#pages").length) {
                    location.reload();
                }
                else {
                    ajaxText.html("<span class='alert alert-success'>The category has been added.</span>");
                    setTimeout(function () {
                        ajaxText.fadeOut("fast", function () {
                            ajaxText.html("<img src='/Content/img/ajax-loader.gif' height='50' />");
                        });
                    }, 2000);

                    newCategoryTextInput.val("");

                    var toAppend = $("table#pages tbody tr:last").clone();
                    toAppend.attr("id", "id_" + data);
                    toAppend.find("#item_Name").val(categoryName);
                    toAppend.find("a.delete").attr("href", "/admin/shop/DeleteCategory/" + data);
                    table.append(toAppend);
                    table.sortable("refresh");
                }
            }
        });
    });

    /* Confirm record deletion */
    $("body").on("click", "a.delete", function () {
        if (!confirm("Confirm category deletion")) return false;
    });

    /* Sorting script */
    $("table#pages tbody").sortable({
        items: "tr:not(.headline)",
        placeholder: "ui-state-highlight",
        update: function () {
            var ids = $("table#pages tbody").sortable("serialize");
            var url = "/Admin/Shop/ReorderCategories";

            $.post(url, ids, function (data) {

            });
        }
    });

    /* Rename category script */
    var originalTextBoxValue;

    $("table#pages input.text-box").dblclick(function () {
        originalTextBoxValue = $(this).val();
        $(this).attr("readonly", false);
    });

    $("table#pages input.text-box").keyup(function (e) {
        if (e.keyCode == 13) {
            $(this).blur();
        }
    });

    $("table#pages input.text-box").blur(function () {
        var $this = $(this);
        var ajaxdiv = $this.parent().parent().parent().find(".ajaxdivtd");
        var newCatName = $this.val();
        var id = $this.parent().parent().parent().parent().parent().attr("id").substring(3);
        var url = "/admin/shop/RenameCategory";

        if (newCatName.length < 3) {
            alert("Category name must be at least 3 characters long.");
            $this.attr("readonly", true);
            return false;
        }

        $.post(url, { newCatName: newCatName, id: id }, function (data) {
            var response = data.trim();

            if (response == "titletaken") {
                $this.val(originalTextBoxValue);
                ajaxdiv.html("<div class='alert alert-danger'>That title is taken.</div>").show();
            }
            else {
                ajaxdiv.html("<div class='alert alert-success'>The category name has been changed.</div>").show();
            }

            setTimeout(function () {
                ajaxdiv.fadeOut("fast", function () {
                    ajaxdiv.html("");
                });
            }, 3000); /* Время жизни уведомления в мс */
        }).done(function () {
            $this.attr("readonly", true);
        });
    });
});