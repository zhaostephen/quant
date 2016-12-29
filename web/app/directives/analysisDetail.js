(function () {
    'use strict';

    angular.module('app').controller('analysisDetailCtrl', ['$scope', '$http', function ($scope, $http) {
        $scope.analytic = {};
        $scope.refresh = function () {
            $http
                .get(root + 'api/analytic/' + $scope.code + '/' + $scope.type)
                .then(function (res) {
                    $scope.analytic = res.data;
                }, function (res) { });
        };
        $scope.refresh();
    }]);

    var analysisDetail = ["$http", function ($http) {
        var directive = {
            link: function (scope, element, attrs) {
            },
            replace: true,
            scope: {
                code: "=",
                type: "="
            },
            controller: 'analysisDetailCtrl',
            templateUrl: root + 'app/directives/analysisDetail.html',
            restrict: 'EA'
        };
        return directive;
    }];

    angular.module('app').directive('analysisDetail', analysisDetail);
})();