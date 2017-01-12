(function () {
    'use strict';

    angular.module('app').controller('sectorstocksCtrl', ['$scope', '$http', '$compile', '$filter', function ($scope, $http, $compile, $filter) {
        function N(digits, chg) {
            return function (value, row, index) {
                var s = $filter('number')(value, digits);
                if (chg) {
                    var cls = value >= 0 ? "red" : "green";
                    return "<span class='" + cls + "'>" + s + "</span>"
                }

                return s;
            };
        }
        function D(f) {
            return function (value) {
                return $filter('date')(value, f || "yyyy-MM-dd");
            }
        }
        function detailFormatter(index, row) {
            $scope.code = "'" + row.code + "'";
            return $compile("<analysis code=\"'" + row.code + "'\"></div>")($scope);
        }

        $scope.$watch('sector', function (newValue) {
            $scope.sector = newValue;
            $scope.refresh();
        });

        $scope.refresh = function () {
            $scope.stocks = {
                options: {
                    height: $(window).height()-70,
                    pagination: true,
                    pageSize: 200,
                    pageList: [200, 400, 'ALL'],
                    showColumns: false,
                    showRefresh: false,
                    minimumCountColumns: 2,
                    showToggle: false,
                    sidePagination: "server",
                    showPaginationSwitch: false,
                    detailView: true,
                    detailFormatter: detailFormatter,
                    url: root + 'api/selections/sectorstocks?sector=' + $scope.sector,
                    columns:
                    [
                        {
                            field: 'code', title: '代码', align: 'center', valign: 'middle', sortable: true,
                            formatter: function (value) {
                                return "<a target='_blank' href='" + root + "analysis?code=" + value + "'>" + value + "</a>"
                            }
                        },
                        { field: 'name', title: '名称', align: 'center', valign: 'middle', sortable: true },
                        { field: 'chg', title: '涨跌%', align: 'center', valign: 'middle', sortable: true, formatter: N(2, true) },
                        { field: 'date', title: '日期', align: 'center', valign: 'middle', sortable: true, formatter: D() },
                        { field: 'high', title: '最高', align: 'center', valign: 'middle' },
                        { field: 'low', title: '最低', align: 'center', valign: 'middle' },
                        { field: 'open', title: '开盘', align: 'center', valign: 'middle' },
                        { field: 'close', title: '收盘', align: 'center', valign: 'middle' },
                        { field: 'volume', title: '成交量', align: 'center', valign: 'middle', sortable: true },
                        { field: 'pb', title: 'pb', align: 'center', valign: 'middle', sortable: true },
                        { field: 'pe', title: 'pe', align: 'center', valign: 'middle', sortable: true },
                        { field: 'mktcap', title: '市值', align: 'center', valign: 'middle', sortable: true }
                    ]
                }
            };
        };

        $scope.refresh();
    }]);

    angular.module('app').directive('sectorstocks', ["$http", function ($http) {
        var directive = {
            replace: true,
            scope: {
                sector: "="
            },
            controller: 'sectorstocksCtrl',
            templateUrl: root + 'app/directives/sectorstocks.html',
            restrict: 'EA'
        };
        return directive;
    }]);
})();