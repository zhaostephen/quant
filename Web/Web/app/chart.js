$(function () {
    var keyhighdates = [];
    var keylowdates = [];
    function getData(code, callback) {
        $.getJSON(root + 'api/MktData/' + code, function (result) {
            var data = result.data;
            var utc = function (d) {
                var date = new Date(d);
                return Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes());
            };

            for (var i = 0; i < data.length; ++i) {
                data[i][0] = utc(data[i][0]);
            }

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
                name:name,
                data: data,
                keyhighdates: keyhighdates,
                keylowdates: keylowdates
            });
        });
    }

    $("#querybutton").click(function () {
        var code = $("#querycode").val();
        var chart = Highcharts.charts[0];

        chart.showLoading('请稍等...');
        getData(code, function (result) {
            chart.series[0].setData(result.data);
            chart.setTitle({
                text: result.name
            });
            chart.hideLoading();
        });
    });

    Highcharts.setOptions({
        global: {
            useUTC: true
        },
        lang: {
            rangeSelectorFrom: "日期:",
            rangeSelectorTo: "至",
            rangeSelectorZoom: "",
            loading: '加载中...',
            //months: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
            //shortMonths: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月',
            //        '9月', '10月', '11月', '12月'],
            //weekdays: ['周日', '周一', '周二', '周三', '周四', '周五', '周六']
        },
    });

    /**
     * Load new data depending on the selected min and max
     */
    function afterSetExtremes(e) {
        console.log(e);
        //var chart = Highcharts.charts[0];

        //chart.showLoading('Loading data from server...');
        //$.getJSON('https://www.highcharts.com/samples/data/from-sql.php?start=' + Math.round(e.min) +
        //        '&end=' + Math.round(e.max) + '&callback=?', function (data) {

        //            chart.series[0].setData(data);
        //            chart.hideLoading();
        //        });
    }

    getData("600000", function (result) {
        var data = result.data;

        // Add a null value for the end date
        //data = [].concat(data, [[Date.UTC(2011, 9, 14, 19, 59), null, null, null, null]]);

        // create the chart
        Highcharts.stockChart('container', {
            chart: {
                type: 'candlestick',
                zoomType: 'x',
                backgroundColor: "#ffffff",
                height: 500,
                width:1000
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
                enabled: false,
                selected: 0,
                inputEnabled: false,
                buttons: [{
                    type: 'day',
                    count: 1,
                    text: '日K'
                }, {
                    type: 'day',
                    count: 7,
                    text: '周K'
                }, {
                    type: 'month',
                    count: 1,
                    text: '月K'
                }, {
                    type: 'minute',
                    count: 5,
                    text: '5分钟'
                }, {
                    type: 'minute',
                    count: 15,
                    text: '15分钟'
                }, {
                    type: 'minute',
                    count: 30,
                    text: '30分钟'
                }, {
                    type: 'hour',
                    count: 1,
                    text: '60分钟'
                }]
            },

            exporting: {
                enabled: false,
            },
            //colors: ['#000000', '#0000ff', '#ff00ff', '#f7a35c', '#8085e9'],
            plotOptions: {
                candlestick: {
                    color: '#00cc00',
                    upColor: '#ff3232',
                    dataLabels: {
                        enabled: true,
                        useHTML: true,
                        formatter: function () {
                            console.log(this.point);
                            var high = (keyhighdates.indexOf(this.x) != -1);
                            var low = (keylowdates.indexOf(this.x) != -1);

                            var label = high ? this.point.high : low ? this.point.low : "";
                            if (label != "") {
                                var css = high ? "small-high-label" : "small-low-label";
                                var bottom = 0;
                                if (low)
                                    bottom = -1 * (this.point.shapeArgs.height+20) + "px";
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
                    afterSetExtremes: afterSetExtremes
                },
                labels: {
                    formatter: function () { return Highcharts.dateFormat('%m/%d', this.value); }
                }
            },

            yAxis: {
                floor: 0
            },

            tooltip: {
                formatter: function () {
                    var s = Highcharts.dateFormat('<span> %Y-%m-%d %H:%M:%S</span>', this.x);
                    s += '<br />开盘:<b>'
                    + this.points[0].point.open
                    + '</b><br />最高:<b>'
                    + this.points[0].point.high
                    + '</b><br />最低:<b>'
                    + this.points[0].point.low
                    + '</b><br />收盘:<b>'
                    + this.points[0].point.close
                    + '</b>';
                    return s;
                },
                shared: true,
                useHTML: true,
                valueDecimals: 2, //有多少位数显示在每个系列的y值
                crosshairs: [{
                    color: '#b9b9b0'
                }, {
                    color: '#b9b9b0'
                }]
            },

            series: [{
                data: data,
                dataGrouping: {
                    enabled: false
                },
                tooltip: {
                    valueDecimals: 2
                }
            }]
        });
    });
});