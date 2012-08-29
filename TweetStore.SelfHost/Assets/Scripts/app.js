function TweetsCtrl($scope, $http) {
	$scope.tweets = [];

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

	$http.get('/api/tweets').success(function (data) {
		$scope.tweets = data;
	});
}