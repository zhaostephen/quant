﻿(function () {
    'use strict';

    var ctrl = ['$scope', '$filter', '$compile', function ($scope, $filter, $compile) {
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

        $scope.universe = {
            options: {
                height: $(window).height() - 70,
                pagination: true,
                pageSize: 50,
                pageList: [50, 100, 'ALL'],
                showColumns: true,
                showRefresh: true,
                minimumCountColumns: 2,
                showToggle: true,
                sidePagination: "server",
                showPaginationSwitch: true,
                detailView: true,
                detailFormatter: detailFormatter,
                url: root + 'api/selections/macd60',
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
    }];

    angular.module('app').controller('selectionCtrl', ctrl)
})();