(function () {
    'use strict';

    angular.module('app').controller('analysisCtrl', ['$scope', '$http', '$compile', '$filter', function ($scope, $http, $compile, $filter) {
        var keyhighdates = [];
        var keylowdates = [];
        var textMAElement = null;
        var volumeElement = null;
        var macdElement = null;
        var macdvolElement = null;

        $scope.type = "D";
        $scope.analytic = {};

        function kperiod(period) {
            switch (period) {
                case "D": return "日K";
                case "W": return "周K";
                case "M": return "月K";
                case "5": return "5分钟";
                case "15": return "15分钟";
                case "30": return "30分钟";
                case "60": return "60分钟";
                default: return period;
            }
        }
        function getData(callback) {
            var code = $scope.code;
            var period = $scope.type || "D";
            $http.get(root + 'api/kdata/' + code + "?ktype=" + period)
                .then(function (res) {
                    var result = res.data;
                    var utc = function (d) {
                        var date = new Date(d);
                        return Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours() - 8, date.getMinutes());
                    };
                    var setUtc = function (f) {
                        for (var i = 0; i < result[f].length; ++i) {
                            result[f][i][0] = utc(result[f][i][0]);
                        }
                    }

                    var colors = [];
                    for (var i = 0; i < result.data.length; ++i) {
                        if (result.data[i][5] >= 0)
                            colors.push("red");
                        else
                            colors.push("forestgreen");
                    }

                    setUtc("data");
                    setUtc("volume");
                    setUtc("macd");
                    setUtc("dif");
                    setUtc("dea");
                    setUtc("macdvol");
                    setUtc("difvol");
                    setUtc("deavol");
                    setUtc("ma5");
                    setUtc("ma30");
                    setUtc("ma60");
                    setUtc("ma120");
                    setUtc("bottom");

                    keyhighdates = [];
                    keylowdates = [];
                    var keyprices = result.keyprices;
                    for (var i = 0; i < keyprices.length; ++i) {
                        if (keyprices[i].Flag == "upper") {
                            keyhighdates.push(utc(keyprices[i].Date));
                        }
                        else {
                            keylowdates.push(utc(keyprices[i].Date));
                        }
                    }

                    var bottom = [];
                    for (var i = 0; i < result.bottom.length; ++i) {
                        bottom.push({
                            x: result.bottom[i][0],
                            title: "B",
                            text:"B"
                        });
                    }

                    $scope.result = {
                        name: result.name + " - " + kperiod(period),
                        data: result.data,
                        volume: result.volume,
                        macd: result.macd,
                        dif: result.dif,
                        dea: result.dea,
                        macdvol: result.macdvol,
                        difvol: result.difvol,
                        deavol: result.deavol,
                        ma5: result.ma5,
                        ma30: result.ma30,
                        ma60: result.ma60,
                        ma120: result.ma120,
                        keyhighdates: keyhighdates,
                        keylowdates: keylowdates,
                        colors: colors,
                        bottom: bottom
                    };
                    callback($scope.result);
                });
        }
        function drawtext(element, text, height, color) {
            var chart = Highcharts.charts[0];
            if (element) {
                element.element.remove();
            }
            return chart.renderer
                .text(text, 20, height)
                .add()
                .css({ fontWeight: 'bold', fontSize: "xx-small", color: color });
        }
        function drawindicators(index) {
            var result = $scope.result;
            if (!result || result.length == 0) return;
            if (angular.isUndefined(index))
                index = result.data.length - 1;

            var date = result.data[index][0];
            var open = result.data[index][1];
            var high = result.data[index][2];
            var low = result.data[index][3];
            var close = result.data[index][4];
            var chg = result.data[index][5];
            var volume = result.volume[index][1];
            var ma5 = result.ma5[index][1];
            var ma120 = result.ma120[index][1];
            var macd = result.macd[index][1];
            var dea = result.dea[index][1];
            var dif = result.dif[index][1];
            var macdvol = result.macdvol[index][1];
            var deavol = result.deavol[index][1];
            var difvol = result.difvol[index][1];

            var chart = Highcharts.charts[0];
            var height = chart.options.chart.height - 90 - 54 - 46;

            textMAElement = drawtext(
                textMAElement,
                Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', date).replace(" 00:00:00", "") +
                " 涨跌:" + chg + "%" + " 现价:" + close + " 开盘:" + open + " 最高:" + high + " 最低:" + low + " MA5:" + ma5 +
                " MA120:" + ma120,
                 90,
                 chg >= 0 ? "red" : "green");

            volumeElement = drawtext(
                volumeElement,
                 "成交量:" + volume,
                 height * 0.55 + 90,
                 chg >= 0 ? "red" : "green");

            macdElement = drawtext(
                  macdElement,
                   "DIF:" + dif + " DEA:" + dea + " M:" + macd,
                   height * 0.7 + 90,
                   macd >= 0 ? "red" : "green");

            macdvolElement = drawtext(
                  macdvolElement,
                   "DIF:" + difvol + " DEA:" + deavol + " M:" + macdvol,
                   height * 0.85 + 90,
                   macdvol >= 0 ? "red" : "green");
        }
        function range() {
            var result = $scope.result;
            var count = result.data.length - Math.min(result.data.length, 150);
            Highcharts.charts[0].xAxis[0].setExtremes(result.data[count][0]);
            drawindicators();
        }
        function candlestick() {
            var chart = Highcharts.charts[0];
            if (!chart) return;
            chart.showLoading('请稍等...');
            getData(function (result) {
                chart.series[0].setData(result.data);
                chart.series[1].setData(result.ma5);
                chart.series[2].setData(result.ma120);
                //chart.series[3].setData(result.volume);
                chart.series[3].update({
                    data:result.volume,
                    colors: result.colors,
                    colorByPoint: true
                });
                chart.series[4].setData(result.macd);
                chart.series[5].setData(result.dif);
                chart.series[6].setData(result.dea);
                chart.series[7].setData(result.macdvol);
                chart.series[8].setData(result.difvol);
                chart.series[9].setData(result.deavol);
                chart.series[10].setData(result.bottom);
                chart.setTitle({
                    text: result.name
                });
                range();
                chart.hideLoading();
            });
        }
        function setExtremes(e) {
            if (e.rangeSelectorButton) {
                $scope.type = e.rangeSelectorButton.value;
                candlestick();
            }
        }
        function tooltipFormatter() {
            drawindicators(this.index);
            var result = $scope.result;
            var chg = result.data[this.index][5];

            var s = Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x).replace(" 00:00:00", "") + "<br />";
            s += '涨跌:' + chg + "%"
            + ' 开盘:' + this.open
            + ' 最高:' + this.high
            + ' 最低:' + this.low
            + ' 收盘:' + this.close;
            var cls = chg >= 0 ? "red" : "green";
            return "<span style='color:" + cls + "'>" + s + "</span><br/>";
        }
        function setupcharts(element) {
            Highcharts.setOptions({
                global: {
                    useUTC: true
                },
                lang: {
                    rangeSelectorFrom: "日期:",
                    rangeSelectorTo: "至",
                    rangeSelectorZoom: "",
                    loading: '加载中...'
                },
            });
            Highcharts.stockChart(element, {
                chart: {
                    type: 'candlestick',
                    zoomType: 'x',
                    backgroundColor: "#ffffff",
                    height: Math.max($(window).height() - 290, 500),
                    width: 1000
                },
                credits: { enabled: false },
                navigator: {
                    xAxis: {
                        labels: {
                            formatter: function () { return Highcharts.dateFormat('%m/%d', this.value); }
                        }
                    }
                },
                scrollbar: {
                    liveRedraw: false
                },
                subtitle: {
                    text: ''
                },
                rangeSelector: {
                    buttonSpacing: 5,
                    buttonTheme: {
                        width: 50,
                        fill: 'none',
                        stroke: 'none',
                        'stroke-width': 0,
                        r: 4,
                        style: {
                            color: '#039',
                            fontWeight: 'bold'
                        },
                        states: {
                            hover: {
                            },
                            select: {
                                fill: '#039',
                                style: {
                                    color: 'white'
                                }
                            }
                        }
                    },
                    enabled: true,
                    selected: 6,
                    inputEnabled: false,
                    buttons: [{
                        text: '日K',
                        value: 'D'
                    }, {
                        text: '周K',
                        value: 'W'
                    }, {
                        text: '月K',
                        value: 'M'
                    }, {
                        text: '5分钟',
                        value: '5'
                    }, {
                        text: '15分钟',
                        value: '15'
                    }, {
                        text: '30分钟',
                        value: '30'
                    }, {
                        text: '60分钟',
                        value: '60'
                    }]
                },
                exporting: {
                    enabled: false,
                },
                plotOptions: {
                    candlestick: {
                        color: '#228B22',
                        upColor: '#ff3232',
                        dataLabels: {
                            enabled: true,
                            allowOverlap: true,
                            useHTML: true,
                            formatter: function () {
                                var high = (keyhighdates.indexOf(this.x) != -1);
                                var low = (keylowdates.indexOf(this.x) != -1);

                                var label = high ? this.point.high : (low ? this.point.low : "");
                                if (label != "") {
                                    var css = high ? "small-high-label" : "small-low-label";
                                    var bottom = 0;
                                    if (low)
                                        bottom = -1 * (this.point.shapeArgs.height + 20) + "px";
                                    return "<span class=\"" + css + "\" style=\"bottom:" + bottom + "\">" + label + "</span>";
                                }
                                return label;
                            }
                        }
                    },
                    line: {
                        marker: {
                            states: {
                                hover: {
                                    enabled: false
                                },
                                select: {
                                    enabled: true
                                }
                            }
                        },
                        states: {
                            hover: {
                                enabled: false
                            }
                        }
                    }
                },
                xAxis: {
                    events: {
                        setExtremes: setExtremes
                    },
                    labels: {
                        formatter: function () { return Highcharts.dateFormat('%m/%d', this.value); }
                    }
                },
                yAxis: [{
                    title: {
                        text: '价格'
                    },
                    height: '55%'
                }, {
                    title: {
                        text: '成交量'
                    },
                    top: '55%',
                    height: '15%',
                    offset: 0
                }, {
                    title: {
                        text: 'M-价格'
                    },
                    top: '70%',
                    height: '15%',
                    offset: 0
                }, {
                    title: {
                        text: 'M2-成交量'
                    },
                    top: '85%',
                    height: '15%',
                    offset: 0
                }],
                series: [
                    {
                        id: 'dataseries',
                        name: "价格",
                        dataGrouping: {
                            enabled: false
                        },
                        tooltip: {
                            headerFormat: "",
                            pointFormatter: tooltipFormatter,
                            useHTML: false,
                            shared: true
                        },
                        events: {
                            mouseOut: function () {
                                drawindicators();
                            }
                        }
                    }, {
                        type: 'line',
                        name: 'MA5',
                        lineWidth: 1,
                        shadow: false,
                        dataGrouping: {
                            enabled: false
                        },
                        tooltip: { valueDecimals: 2, pointFormatter: function () { return "MA5:<b>" + this.y + "</b>  "; } }
                    }, {
                        type: 'line',
                        name: 'MA120',
                        lineWidth: 1,
                        shadow: false,
                        dataGrouping: {
                            enabled: false
                        },
                        tooltip: { valueDecimals: 2, pointFormatter: function () { return "MA120:<b>" + this.y + "</b>  "; } }
                    }, {
                        type: 'column',
                        name: '成交量',
                        shadow: false,
                        dataGrouping: {
                            enabled: false
                        },
                        tooltip: { valueDecimals: 2, pointFormatter: function () { return "<br/>成交量(手):<b>" + Highcharts.numberFormat(this.y, 0).replace(" ", "") + "</b>  "; } },
                        yAxis: 1
                    }, {
                        type: 'column',
                        name: 'MACD',
                        color: "red",
                        negativeColor: "forestgreen",
                        maxPointWidth: 1,
                        shadow: false,
                        dataGrouping: {
                            enabled: false
                        },
                        tooltip: { valueDecimals: 2, pointFormatter: function () { return "<br/>M:<b>" + Highcharts.numberFormat(this.y, 2) + "</b>  "; } },
                        yAxis: 2
                    }, {
                        type: 'line',
                        name: 'DIF',
                        color: "#000000",
                        lineWidth: 1,
                        shadow: false,
                        dataGrouping: {
                            enabled: false
                        },
                        tooltip: { valueDecimals: 2, pointFormatter: function () { return "DIF:<b>" + Highcharts.numberFormat(this.y, 2) + "</b>  "; } },
                        yAxis: 2
                    }, {
                        type: 'line',
                        name: 'DEA',
                        lineWidth: 1,
                        color: "#8B4513",
                        shadow: false,
                        dataGrouping: {
                            enabled: false
                        },
                        tooltip: { valueDecimals: 2, pointFormatter: function () { return "DEA:<b>" + Highcharts.numberFormat(this.y, 2) + "</b>  "; } },
                        yAxis: 2
                    }, {
                        type: 'column',
                        name: 'MACD',
                        color: "red",
                        negativeColor: "forestgreen",
                        maxPointWidth: 1,
                        shadow: false,
                        dataGrouping: {
                            enabled: false
                        },
                        tooltip: { valueDecimals: 2, pointFormatter: function () { return "<br/>M2:<b>" + Highcharts.numberFormat(this.y, 2) + "</b>  "; } },
                        yAxis: 3
                    }, {
                        type: 'line',
                        name: 'DIF',
                        color: "#000000",
                        lineWidth: 1,
                        shadow: false,
                        dataGrouping: {
                            enabled: false
                        },
                        tooltip: { valueDecimals: 2, pointFormatter: function () { return "DIF2:<b>" + Highcharts.numberFormat(this.y, 2) + "</b>  "; } },
                        yAxis: 3
                    }, {
                        type: 'line',
                        name: 'DEA',
                        color: "#8B4513",
                        lineWidth: 1,
                        shadow: false,
                        dataGrouping: {
                            enabled: false
                        },
                        tooltip: { valueDecimals: 2, pointFormatter: function () { return "DEA2:<b>" + Highcharts.numberFormat(this.y, 2) + "</b>  "; } },
                        yAxis: 3
                    }, {
                        type: 'flags',
                        onSeries: 'dataseries',
                        shape: 'circlepin',
                        title: "<span style='color:red'>B<span>",
                        color: "red",
                        useHTML:"true",
                        width: 5,
                        y:-30
                    }
                ]
            });
        }

        $scope.$watch('code', function (newValue) {
            $scope.code = newValue;

            candlestick($scope.code, $scope.type);

            $scope.refresh();
        });

        this.setupcharts = function (element) {
            var id = "id" + $scope.code;
            $(element).attr("id", id);
            setupcharts(id, $scope.code, $scope.type);
        }

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

        $scope.refresh = function () {
            $http
                .get(root + 'api/analytic/' + $scope.code + '/' + $scope.type)
                .then(function (res) {
                    $scope.analytic = res.data;

                    $scope.sectorstocks = {
                        options: {
                            height: 500,
                            pagination: true,
                            pageSize: 100,
                            pageList: [100, 200, 'ALL'],
                            showColumns: true,
                            showRefresh: true,
                            minimumCountColumns: 2,
                            showToggle: true,
                            sidePagination: "server",
                            showPaginationSwitch: true,
                            detailView: true,
                            detailFormatter: detailFormatter,
                            url: root + 'api/selections/sectorstocks?sector=' + res.data.code,
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

                }, function (res) { });
        };

        candlestick();
        $scope.refresh();
    }]);

    var analysisDetail = ["$http", function ($http) {
        var directive = {
            link: function (scope, element, attrs, chartCtrl) {
                if (scope.showChart) {
                    chartCtrl.setupcharts($(element).find("div")[1]);
                }
            },
            replace: true,
            scope: {
                code: "=",
                showChart: "&"
            },
            controller: 'analysisCtrl',
            templateUrl: root + 'app/directives/analysis.html',
            restrict: 'EA'
        };
        return directive;
    }];

    angular.module('app').directive('analysis', analysisDetail);
})();