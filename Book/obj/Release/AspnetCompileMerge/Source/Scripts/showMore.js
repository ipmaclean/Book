// Show more/less on description
$(".showmore-btn").on('click', function () {
    $(this).siblings().toggleClass('hidden');
    let replaceText = $(this).siblings(".full-description").hasClass("hidden") ? "Read more" : "Read Less";
    $(this).text(replaceText);
});