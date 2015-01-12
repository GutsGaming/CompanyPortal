$(document).ready(function () {
    $(".moment").each(function () {
        var date = $(this).html();
        $(this).html(moment(date).fromNow());
        $(this).attr("title", moment(date).calendar());
        $(this).tooltip();
    });
});