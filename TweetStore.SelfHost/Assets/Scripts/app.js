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
		return tweet.AuthorId || tweet.AuthorName;
	};

	$scope.getContent = function (tweet) {
		return tweet.TweetHtmlContent || tweet.TweetTextContent;
	};

	$scope.showTweet = function (tweet) {
		var pairs = [];
		for (var key in tweet) {
			if (key !== "$$hashKey") {
				pairs.push({ Key: key, Value: tweet[key] });
			}
		}
		return pairs;
	};

	$scope.toggle = function (tweet) {
		$scope.activeTweet = $scope.isActiveTweet(tweet) ? null : tweet.Id;
	};

	$scope.remove = function (tweet) {
		var url = '/api/tweets/' + tweet.Id;
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