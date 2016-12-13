(function () {
    'use strict';

    var ctrl = ['$scope', '$http', function ($scope, $http) {
        $scope.sectors = [];
        $scope.analyticsList = [];
        $scope.select = function (ktype, name) {
            var obj = { name: name, analytics: [] };
            $scope.analyticsList.push(obj);
            for (var i = 0; i < $scope.sectors.length; ++i) {
                $http
                    .get(root + 'api/analytic/' + $scope.sectors[i] + '/' + ktype)
                    .then(function (res) {
                        obj.analytics = obj.analytics.concat(res.data).sort(function(a, b) {
                            return b.change-a.change;
                        });
                    }, function (res) { });
            }
        }

        $http
            .get(root + 'api/basics/sectors')
            .then(function (res) {
                $scope.sectors = res.data || [];
                $scope.select("D", "日级别");
                $scope.select("W", "周级别");
                $scope.select("M", "月级别");
                $scope.select("5", "5分钟级别");
                $scope.select("15", "15分钟级别");
                $scope.select("30", "30分钟级别");
                $scope.select("60", "60分钟级别");
            }, function (res) { });
    }];

    angular.module('app').controller('analyzeSectorCtrl', ctrl)
})();