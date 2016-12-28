(function () {
    'use strict';

    var ctrl = ['$scope', '$http', function ($scope, $http) {
        $scope.code = code;
        $scope.type = "D";

        setupcharts($scope.code, $scope.type, function (code, period) {
            $scope.code = code;
            $scope.type = period;
        });

        candlestick($scope.code, $scope.type);
    }];

    angular.module('app').controller('analyzeCtrl', ctrl)
})();