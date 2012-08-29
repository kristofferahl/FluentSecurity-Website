function TweetsCtrl($scope, $http) {
	$scope.tweets = [];
	$scope.activeTweet = null;

	$scope.isActiveTweet = function (tweet) {
		return tweet.Id === $scope.activeTweet;
	};

	$scope.getStatus = function (tweet) {
		return tweet.Private ? "private" : "public";
	};

	$scope.getAuthor = function (tweet) {
		if (tweet.AuthorId) return tweet.AuthorId;
		if (tweet.AuthorName) return tweet.AuthorName;
		return "-";
	};

	$scope.getContent = function (tweet) {
		if (tweet.TweetHtmlContent) return tweet.TweetHtmlContent;
		if (tweet.TweetTextContent) return tweet.TweetTextContent;
		return "-";
	};

	$scope.showTweet = function (tweet) {
		var pairs = [];
		for (var key in tweet) {
			if (key !== "$$hashKey") {
				var val = tweet[key];
				pairs.push({
					Key: key,
					Value: val
				});
			}
		}
		return pairs;
	};

	$scope.toggle = function (tweet) {
		if ($scope.isActiveTweet(tweet)) {
			$scope.activeTweet = null;
		} else {
			$scope.activeTweet = tweet.Id;
		}
	};

	$scope.remove = function (tweet) {
		var url = '/api/tweets/' + tweet.Id;
		console.log(url);
		$http.delete(url).success(function () {
			for (var i=0; i < $scope.tweets.length; i++) {
				if ($scope.tweets[i].Id === tweet.Id) {
					$scope.tweets.splice(i, 1);
					if ($scope.activeTweet === tweet.Id) {
						$scope.activeTweet = null;
					}
					return;
				}
			}
		});
	};

	$http.get('/api/tweets').success(function (data) {
		$scope.tweets = data;
	});
}