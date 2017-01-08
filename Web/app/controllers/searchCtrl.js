(function () {
    'use strict';

    var ctrl = ['$scope', '$http', function ($scope, $http) {
        $scope.code = 'sh';
        $scope.searchcode = 'sh';

        $scope.search = function () {
            $scope.code = $scope.searchcode;
        };

        $scope.select = function (code) {
            $scope.code = code;
            $scope.searchcode = code;
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

        $scope.hotsectors = [];
        $http
            .get(root + 'api/basics/hotsectors')
            .then(function (res) {
                $scope.hotsectors = res.data;
            });
    }];

    angular.module('app').controller('searchCtrl', ctrl)
})();