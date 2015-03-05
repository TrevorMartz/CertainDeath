var mgw; // = View.MainGameWindow; Filled in with game.js
$(function () {
    $("#shop-window").on("click", ".exit-button", function () {
        $("#shop-window").hide();
    });

    $("#shop-window").on("click", ".purchase-button", function () {
        if (mgw) {
            $("#shop-window").hide();
            mgw.placeBuilding($(this).parent().attr("id"));
        }
    });
});