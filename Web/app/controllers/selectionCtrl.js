(function () {
    'use strict';

    var ctrl = ['$scope', '$http', '$interval', function ($scope, $http, $interval) {
        $scope.universe = [];

        $scope.refresh = function () {
            $http.get(root + 'api/selections')
               .then(function (res) {
                   $scope.universe = res.data;
               }, function (res) { });
        }

        $scope.refresh();
    }];

    angular.module('app').controller('selectionCtrl', ctrl)
})();