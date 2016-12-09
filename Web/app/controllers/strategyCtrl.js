(function () {
    'use strict';

    var ctrl = ['$scope', '$http', '$interval', function ($scope, $http, $interval) {
        $scope.trades = [];
        $scope.portflio = "kdj15min";
        $scope.lastupdate = new Date();

        $scope.refresh = function () {
            $http.get(root + 'api/trades/' + $scope.portflio)
               .then(function (res) {
                   $scope.lastupdate = new Date();
                   $scope.trades = res.data;
               }, function (res) { });
        }

        $scope.refresh();

        $interval(function () { $scope.refresh(); }, 5 * 60 * 1000);
    }];

    angular.module('app').controller('strategyCtrl', ctrl)
})();