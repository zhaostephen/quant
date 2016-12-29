(function () {
    'use strict';

    var ctrl = ['$scope', '$http', function ($scope, $http) {
        $scope.code = 'sh';
        $scope.type = 'D';

        $scope.search = function () {
            $scope.type = "D";
        };

        $scope.select = function (code) {
            if (!code || code == "")
                return;
            $scope.code = code;
            $scope.type = "D";
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
    }];

    angular.module('app').controller('searchCtrl', ctrl)
})();