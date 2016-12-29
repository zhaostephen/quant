(function () {
    'use strict';

    var ctrl = ['$scope', '$http', function ($scope, $http) {
        $scope.code = code;
        $scope.type = "D";
    }];

    angular.module('app').controller('analyzeCtrl', ctrl)
})();