(function () {
    'use strict';

    var ctrl = ['$scope', '$http', function ($scope, $http) {
        $scope.trades = [];
        $scope.portflio = "kdj15min";

        $http.get(root + 'api/trades/' + portflio)
           .then(function (res) {
               $scope.trades = res.data;
           }, function (res) {

           });
    }];

    angular.module('app').controller('strategyCtrl', ctrl)
})();