(function () {
    'use strict';

    angular.module('app').controller('analysisCtrl', ['$scope', '$http', function ($scope, $http) {
        var keyhighdates = [];
        var keylowdates = [];
        var currentcode = "";
        var currentperiod = "";
        var chartssetup = false;

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
        function getData(code, period, callback) {
            period = period || "D";
            currentcode = code;
            currentperiod = period;
            $.getJSON(root + 'api/kdata/' + code + "?ktype=" + period, function (result) {
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
                    if (result.data[i][4] >= result.data[i][1])
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
                callback({
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
                    colors: colors
                });
            });
        }
        function range(result) {
            var count = result.data.length - Math.min(result.data.length, 150);
            Highcharts.charts[0].xAxis[0].setExtremes(result.data[count][0]);
        }
        function candlestick(code, period) {
            var chart = Highcharts.charts[0];
            if (!chart) return;

            chart.showLoading('请稍等...');
            getData(code, period, function (result) {
                chart.series[0].setData(result.data);
                chart.series[1].setData(result.ma5);
                chart.series[2].setData(result.ma120);
                chart.series[3].setData(result.volume);
                chart.series[4].setData(result.macd);
                chart.series[5].setData(result.dif);
                chart.series[6].setData(result.dea);
                chart.series[7].setData(result.macdvol);
                chart.series[8].setData(result.difvol);
                chart.series[9].setData(result.deavol);
                chart.setTitle({
                    text: result.name
                });
                chart.series[3].update({
                    colors: result.colors
                });
                range(result);
                chart.hideLoading();
            });
        }
        function setupcharts(element, code, period, callback) {
            if (chartssetup) {
                return;
            }
            chartssetup = true;
            function setExtremes(e) {
                if (e.rangeSelectorButton) {
                    candlestick(currentcode, e.rangeSelectorButton.value);
                    if (callback)
                        callback(currentcode, e.rangeSelectorButton.value);
                }
            }
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
            getData(code, period, function (result) {
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
                    title: {
                        text: result.name
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
                                allowOverlap:true,
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
                            data: result.data,
                            name: "价格",
                            dataGrouping: {
                                enabled: false
                            },
                            tooltip: {
                                headerFormat: "",
                                pointFormatter: function () {
                                    var s = Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x).replace(" 00:00:00", "") + "<br />";
                                    s += '  开盘:<b>' + this.open
                                    + '</b>  最高:<b>' + this.high
                                    + '</b>  最低:<b>' + this.low
                                    + '</b>  收盘:<b>' + this.close
                                    + '</b>';
                                    var cls = this.close >= this.open ? "red" : "green";
                                    return "<span class='" + cls + "'>" + s + "</span><br/>";
                                },
                                useHTML: true,
                                shared: true
                            },
                        }, {
                            type: 'line',
                            name: 'MA5',
                            lineWidth: 1,
                            shadow: false,
                            dataGrouping: {
                                enabled: false
                            },
                            data: result.ma5,
                            tooltip: { valueDecimals: 2, pointFormatter: function () { return "MA5:<b>" + this.y + "</b>  "; } }
                        }, {
                            type: 'line',
                            name: 'MA120',
                            lineWidth: 1,
                            shadow: false,
                            dataGrouping: {
                                enabled: false
                            },
                            data: result.ma120,
                            tooltip: { valueDecimals: 2, pointFormatter: function () { return "MA120:<b>" + this.y + "</b>  "; } }
                        }, {
                            type: 'column',
                            name: '成交量',
                            colors: result.colors,
                            colorByPoint: true,
                            shadow: false,
                            dataGrouping: {
                                enabled: false
                            },
                            data: result.volume,
                            tooltip: { valueDecimals: 2, pointFormatter: function () { return "<br/>成交量:<b>" + Highcharts.numberFormat(this.y / 100 / 10000, 0).replace(" ", "") + "</b>  "; } },
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
                            data: result.macd,
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
                            data: result.dif,
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
                            data: result.dea,
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
                            data: result.macdvol,
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
                            data: result.difvol,
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
                            data: result.deavol,
                            tooltip: { valueDecimals: 2, pointFormatter: function () { return "DEA2:<b>" + Highcharts.numberFormat(this.y, 2) + "</b>  "; } },
                            yAxis: 3
                        }
                    ]
                });
                range(result);
            });
        }

        $scope.type = "D";
        $scope.analytic = {};
        
        $scope.$watch('code', function (newValue) {
            $scope.code = newValue;

            candlestick($scope.code, $scope.type);

            $scope.refresh();
        });

        this.setupcharts = function (element) {
            var id = "id" + $scope.code;
            $(element).attr("id", id);
            setupcharts(id, $scope.code, $scope.type, function (code, period) {
                $scope.code = code;
                $scope.type = period;
            });
        }

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