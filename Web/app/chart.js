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
        var setUtc = function (f) {
            for (var i = 0; i < result[f].length; ++i) {
                result[f][i][0] = utc(result[f][i][0]);
            }
        }

        setUtc("data");
        setUtc("volume");
        setUtc("macd");
        setUtc("dif");
        setUtc("dea");
        setUtc("macdvol");
        setUtc("difvol");
        setUtc("deavol");

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
            keyhighdates: keyhighdates,
            keylowdates: keylowdates
        });
    });
}
function range(result) {
    var count = result.data.length - Math.min(result.data.length, 120);
    Highcharts.charts[0].xAxis[0].setExtremes(result.data[count][0]);
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
                    text: 'M-成交量'
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
                        shared: true
                    },
                }, {
                    type: 'column',
                    name: '成交量',
                    dataGrouping: {
                        enabled: false
                    },
                    data: result.volume, tooltip: { valueDecimals: 0 }, yAxis: 1
                }, {
                    type: 'column',
                    name: 'MACD',
                    //colors: ['#00cc00', '#ff3232'],
                    maxPointWidth: 1,
                    shadow: false,
                    dataGrouping: {
                        enabled: false
                    },
                    data: result.macd, tooltip: { valueDecimals: 2 }, yAxis: 2
                }, {
                    type: 'line',
                    name: 'DIF',
                    color: "blue",
                    lineWidth: 0.5,
                    shadow: false,
                    dataGrouping: {
                        enabled: false
                    },
                    data: result.dif, tooltip: { valueDecimals: 2 }, yAxis: 2
                }, {
                    type: 'line',
                    name: 'DEA',
                    lineWidth: 0.5,
                    color: "red",
                    shadow: false,
                    dataGrouping: {
                        enabled: false
                    },
                    data: result.dea, tooltip: { valueDecimals: 2 }, yAxis: 2
                }, {
                    type: 'column',
                    name: 'MACD',
                    //colors: ['#00cc00', '#ff3232'],
                    maxPointWidth: 1,
                    shadow: false,
                    dataGrouping: {
                        enabled: false
                    },
                    data: result.macdvol, tooltip: { valueDecimals: 2 }, yAxis: 3
                }, {
                    type: 'line',
                    name: 'DIF',
                    color: "blue",
                    lineWidth: 0.5,
                    shadow: false,
                    dataGrouping: {
                        enabled: false
                    },
                    data: result.difvol, tooltip: { valueDecimals: 2 }, yAxis: 3
                }, {
                    type: 'line',
                    name: 'DEA',
                    color: "red",
                    lineWidth: 0.5,
                    shadow: false,
                    dataGrouping: {
                        enabled: false
                    },
                    data: result.deavol, tooltip: { valueDecimals: 2 }, yAxis: 3
                }
            ]
        });
        range(result);
    });
}
function candlestick(code, period) {
    var chart = Highcharts.charts[0];
    if (!chart) return;

    chart.showLoading('请稍等...');
    getData(code, period, function (result) {
        chart.series[0].setData(result.data);
        chart.series[1].setData(result.volume);
        chart.series[2].setData(result.macd);
        chart.series[3].setData(result.dif);
        chart.series[4].setData(result.dea);
        chart.series[5].setData(result.macdvol);
        chart.series[6].setData(result.difvol);
        chart.series[7].setData(result.deavol);
        chart.setTitle({
            text: result.name
        });
        range(result);
        chart.hideLoading();
    });
}