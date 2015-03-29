$('.removeFromCart').click(function () {
    var albumId = $(this).attr("data-id");
    var albumTitle = $(this).closest('tr').find('td:first-child > a').html();

    $.post("/cart/remove/" + albumId, function(data) {
        $('#container').html(data);
        $('#update-message').html(albumTitle + ' has been removed from your shopping cart.')
    });
});