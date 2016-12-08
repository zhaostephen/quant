(function () {
    'use strict';

    var ctrl = ['$scope', '$http', function ($scope, $http) {
        $scope.search = function () {
            candlestick($scope.code, "D");
        };

        $scope.select = function (code) {
            if (!code || code == "")
                return;
            $scope.code = code;
            candlestick($scope.code, "D");
        };

        $scope.basicnames = new kendo.data.DataSource({
            serverFiltering: true,
            transport: {
                read: function (options) {
                    $http
                        .get(root + 'api/basics/' + $("#querycode").val())
                        .then(function (res) {
                            options.success(res.data);
                        }, function (res) {
                            options.error(res);
                        });
                }
            }
        });
    }];

    angular.module('app').controller('searchCtrl', ctrl)
})();