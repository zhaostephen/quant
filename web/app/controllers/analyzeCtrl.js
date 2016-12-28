(function () {
    'use strict';

    var ctrl = ['$scope', '$http', function ($scope, $http) {
        $scope.analytic = {};
        $scope.getAnalytic = function (code, period) {
            $http
                .get(root + 'api/analytic/' + code + '/' + period)
                .then(function (res) {
                    $scope.analytic = res.data;
                }, function (res) { });
        }
        $scope.select = function (code) {
            if (!code || code == "")
                return;
            $scope.code = code;
            candlestick($scope.code, "D");
            $scope.getAnalytic($scope.code, "D");
        };
        setupcharts(code, "D", function (code, period) {
            $scope.getAnalytic(code, period);
        });
        $scope.select(code);
    }];

    angular.module('app').controller('analyzeCtrl', ctrl)
})();