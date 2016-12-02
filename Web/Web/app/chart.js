$(function () {

    $("#querybutton").click(function () {
        var code = $("#querycode").val();
        var chart = Highcharts.charts[0];

        chart.showLoading('请稍等...');
        $.getJSON(root + 'api/MktData/' + code, function (result) {
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

    $.getJSON(root + 'api/MktData/600000', function (result) {
        var data = result.data;
        for (var i = 0; i < data.length; ++i) {
            var date = new Date(data[i][0]);
            data[i][0] = Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes());
        }

        // Add a null value for the end date
        //data = [].concat(data, [[Date.UTC(2011, 9, 14, 19, 59), null, null, null, null]]);

        // create the chart
        Highcharts.stockChart('container', {
            chart: {
                type: 'candlestick',
                zoomType: 'x',
                backgroundColor: "#ffffff"
            },

            //navigator: {
            //    adaptToUpdatedData: false,
            //    series: {
            //        data: data
            //    }
            //},

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
                    //color: '#54ffff',
                    color: '#00cc00',
                    upColor: '#ff3232'
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
                }
                //minRange: 3600 * 1000
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