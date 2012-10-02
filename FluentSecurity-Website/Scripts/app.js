$(document).ready(function () {
	var getRandomNumber = function (items, exclude) {
		var ran = Math.floor(Math.random() * items) + 1;
		if (ran === exclude || ran >= items) return getRandomNumber(items, exclude);
		return ran;
	};

	/*
	* You'll need your own API key, don't abuse mine please!
	* Get yours here: http://www.websnapr.com/free_services/
	*/
	var apiKey = "2b8M1d2Xsc0b";

	// Notice the use of the each method to gain access to each element individually
	var blogposts = $("#extras .blogposts li").hide();

	var random1 = getRandomNumber(blogposts.size());
	var random2 = getRandomNumber(blogposts.size(), random1);

	blogposts.each(function (index) {
		if (index !== random1 && index !== random2) return this;

		var link = $(this).show().find("a");
		var url = encodeURIComponent(link.attr('href'));

		var thumbnail = $('<img />', {
			src: 'http://images.websnapr.com/?url=' + url + '&key=' + apiKey + '&hash=' + encodeURIComponent(websnapr_hash),
			alt: 'Loading thumbnail...',
			title: 'Thumnail ' + index,
			width: 120
		});

		link.before(thumbnail);

		return this;
	});

	console.log("Ran1:" + random1);
	console.log("Ran2:" + random2);
});