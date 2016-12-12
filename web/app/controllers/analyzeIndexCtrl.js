(function () {
    'use strict';

    var ctrl = ['$scope', '$http', function ($scope, $http) {
        $scope.indexes = ['sh', 'sz', 'cyb'];//, 'hs300', 'sz50'
        $scope.analyticsList = [];
        $scope.select = function (ktype, name) {
            var obj = { name: name, analytics: [] };
            $scope.analyticsList.push(obj);
            for (var i = 0; i < $scope.indexes.length; ++i) {
                $http
                    .get(root + 'api/analytic/' + $scope.indexes[i] + '/' + ktype)
                    .then(function (res) {
                        obj.analytics = obj.analytics.concat(res.data);
                    }, function (res) { });
            }
        }
        $scope.select("D", "日级别");
        $scope.select("W", "周级别");
        $scope.select("M", "月级别");
        $scope.select("5", "5分钟级别");
        $scope.select("15", "15分钟级别");
        $scope.select("30", "30分钟级别");
        $scope.select("60", "60分钟级别");
    }];

    angular.module('app').controller('analyzeIndexCtrl', ctrl)
})();