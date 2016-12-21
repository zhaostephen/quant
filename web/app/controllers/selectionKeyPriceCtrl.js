(function () {
    'use strict';

    var ctrl = ['$scope', '$http', '$interval', function ($scope, $http, $interval) {
        $scope.universe = [];

        $scope.refresh = function () {
            $scope.universe = [];

            var selections = ['keyprice'];

            for (var i = 0; i < selections.length; ++i) {
                $http.get(root + 'api/selections/' + selections[i])
                   .then(function (res) {
                       $scope.universe = $scope.universe.concat(res.data || []);
                   }, function (res) { });
            }
        }

        $scope.refresh();
    }];

    angular.module('app').controller('selectionKeyPriceCtrl', ctrl)
})();