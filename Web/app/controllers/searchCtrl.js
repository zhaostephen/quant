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

        $scope.search = function () {
            candlestick($scope.code, "D");
            $scope.getAnalytic($scope.code, "D");
        };

        $scope.select = function (code) {
            if (!code || code == "")
                return;
            $scope.code = code;
            candlestick($scope.code, "D");
            $scope.getAnalytic($scope.code, "D");
        };

        $scope.basicnames = new kendo.data.DataSource({
            serverFiltering: true,
            transport: {
                read: function (options) {
                    $http
                        .get(root + 'api/basics/names/' + $("#querycode").val())
                        .then(function (res) {
                            options.success(res.data);
                        }, function (res) {
                            options.error(res);
                        });
                }
            }
        });

        $scope.code = 'sh';
        setupcharts('sh', "D", function (code, period) {
            $scope.getAnalytic(code, period);
        });
        $scope.getAnalytic('sh', "D");
    }];

    angular.module('app').controller('searchCtrl', ctrl)
})();