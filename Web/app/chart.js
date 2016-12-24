var keyhighdates = [];
var keylowdates = [];
var currentcode = "";
var currentperiod = "";
var chartssetup = false;

function kperiod(period) {
    switch(period)
    {
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
            return Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours()-8, date.getMinutes());
        };

        for (var i = 0; i < result.data.length; ++i) {
            result.data[i][0] = utc(result.data[i][0]);
        }
        for (var i = 0; i < result.volume.length; ++i) {
            result.volume[i][0] = utc(result.volume[i][0]);
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
            name: result.name + " - " + kperiod(period),
            data: result.data,
            volume:result.volume,
            keyhighdates: keyhighdates,
            keylowdates: keylowdates
        });
    });
}
function setupcharts(code, period, callback) {
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
                enabled: true,
                selected: 6,
                inputEnabled: false,
                buttons: [{
                    text: '日K',
                    value: 'D'
                }, {
                    text: '周K',
                    value:'W'
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
                    color: '#00cc00',
                    upColor: '#ff3232',
                    dataLabels: {
                        enabled: true,
                        useHTML: true,
                        formatter: function () {
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
                height: '70%'
            }, {
                title: {
                    text: '成交量'
                },
                top: '70%',
                height: '30%',
                offset: 0
            }],
            series: [
                {
                    data: result.data,
                    name: "价格",
                    tooltip: {
                        headerFormat:"",
                        pointFormatter: function () {
                            var s = Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x);
                            s += '<br />开盘:<b>'
                            + this.open
                            + '</b><br />最高:<b>'
                            + this.high
                            + '</b><br />最低:<b>'
                            + this.low
                            + '</b><br />收盘:<b>'
                            + this.close
                            + '</b>';
                            var cls = this.close >= this.open ? "red" : "green";
                            return "<span class='" + cls + "'>" + s + "</span><br/>";
                        },
                        shared:true
                    },
                },
                { type: 'column', name: '成交量', data: result.volume, tooltip: { valueDecimals: 0}, yAxis: 1 }
            ]
        });
    });
}
function candlestick(code, period) {
    var chart = Highcharts.charts[0];
    if (!chart) return;

    chart.showLoading('请稍等...');
    getData(code, period, function (result) {
        chart.series[0].setData(result.data);
        chart.series[1].setData(result.volume);
        chart.setTitle({
            text: result.name
        });
        chart.hideLoading();
    });
}