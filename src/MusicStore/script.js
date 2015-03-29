$('#remove').click(function() {
    var album = $(this).attr("data-id");

    if (album != '') {

        $.post("/cart/remove/" + album, function(data) {
            window.location = "/cart";
        });

        // Perform the ajax post
        //$.post("/ShoppingCart/RemoveFromCart", { "id": recordToDelete },
        //    function (data) {
        //        // Successful requests get here
        //        // Update the page elements
        //        if (data.ItemCount == 0) {
        //            $('#row-' + data.DeleteId).fadeOut('slow');
        //        } else {
        //            $('#item-count-' + data.DeleteId).text(data.ItemCount);
        //        }

        //        $('#cart-total').text(data.CartTotal);
        //        $('#update-message').text(data.Message);
        //        $('#cart-status').text('Cart (' + data.CartCount + ')');
        //    });
    }
});