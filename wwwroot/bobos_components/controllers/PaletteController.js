angular.module("frontendAngularClientApp").controller("PaletteController", function ($scope, $state, $rootScope, $stateParams, dashboardService, $http) {




    $scope.echartDoughnutOptions = {
        tooltip: {
            trigger: 'item',
            formatter: "{a} <br/>{b}: {c} ({d}%)"
        },
        legend: {
            orient: 'vertical',
            x: 'left',
            data: ['直接访问', '邮件营销', '联盟广告', '视频广告', '搜索引擎']
        },
        series: [
            {
                name: '访问来源',
                type: 'pie',
                radius: ['50%', '70%'],
                avoidLabelOverlap: false,
                label: {
                    normal: {
                        show: false,
                        position: 'center'
                    },
                    emphasis: {
                        show: true,
                        textStyle: {
                            fontSize: '30',
                            fontWeight: 'bold'
                        }
                    }
                },
                labelLine: {
                    normal: {
                        show: false
                    }
                },
                data: [
                    { value: 335, name: '直接访问' },
                    { value: 310, name: '邮件营销' },
                    { value: 234, name: '联盟广告' },
                    { value: 135, name: '视频广告' },
                    { value: 1548, name: '搜索引擎' }
                ]
            }
        ]
    };



    function genData(count) {
        var nameList = [
            '赵', '钱', '孙', '李', '周', '吴', '郑', '王', '冯', '陈', '褚', '卫', '蒋', '沈', '韩', '杨', '朱', '秦', '尤', '许', '何', '吕', '施', '张', '孔', '曹', '严', '华', '金', '魏', '陶', '姜', '戚', '谢', '邹', '喻', '柏', '水', '窦', '章', '云', '苏', '潘', '葛', '奚', '范', '彭', '郎', '鲁', '韦', '昌', '马', '苗', '凤', '花', '方', '俞', '任', '袁', '柳', '酆', '鲍', '史', '唐', '费', '廉', '岑', '薛', '雷', '贺', '倪', '汤', '滕', '殷', '罗', '毕', '郝', '邬', '安', '常', '乐', '于', '时', '傅', '皮', '卞', '齐', '康', '伍', '余', '元', '卜', '顾', '孟', '平', '黄', '和', '穆', '萧', '尹', '姚', '邵', '湛', '汪', '祁', '毛', '禹', '狄', '米', '贝', '明', '臧', '计', '伏', '成', '戴', '谈', '宋', '茅', '庞', '熊', '纪', '舒', '屈', '项', '祝', '董', '梁', '杜', '阮', '蓝', '闵', '席', '季', '麻', '强', '贾', '路', '娄', '危'
        ];
        var legendData = [];
        var seriesData = [];
        var selected = {};
        for (var i = 0; i < 5; i++) {
            name = Math.random() > 0.65
                ? makeWord(4, 1) + '·' + makeWord(3, 0)
                : makeWord(2, 1);
            legendData.push(name);
            seriesData.push({
                name: name,
                value: Math.round(Math.random() * 100000)
            });
            selected[name] = i < 6;
        }

        return {
            legendData: legendData,
            seriesData: seriesData,
            selected: selected
        };

        function makeWord(max, min) {
            var nameLen = Math.ceil(Math.random() * max + min);
            var name = [];
            for (var i = 0; i < nameLen; i++) {
                name.push(nameList[Math.round(Math.random() * nameList.length - 1)]);
            }
            return name.join('');
        }
    }


    var data = genData(50);
    $scope.echartPieChartOptions = {
        title: {
            text: '同名数量统计',
            subtext: '纯属虚构',
            x: 'center'
        },
        tooltip: {
            trigger: 'item',
            formatter: "{a} <br/>{b} : {c} ({d}%)"
        },
        legend: {
            type: 'scroll',
            orient: 'vertical',
            right: 10,
            top: 20,
            bottom: 20,
            data: data.legendData,

            selected: data.selected
        },
        series: [
            {
                name: '姓名',
                type: 'pie',
                radius: '55%',
                center: ['40%', '50%'],
                data: data.seriesData,
                itemStyle: {
                    emphasis: {
                        shadowBlur: 10,
                        shadowOffsetX: 0,
                        shadowColor: 'rgba(0, 0, 0, 0.5)'
                    }
                }
            }
        ]
    };



    $scope.echartWaterFallOptions = {
        title: {
            text: '深圳月最低生活费组成（单位:元）',
            subtext: 'From ExcelHome',
            sublink: 'http://e.weibo.com/1341556070/AjQH99che'
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {            // 坐标轴指示器，坐标轴触发有效
                type: 'shadow'        // 默认为直线，可选为：'line' | 'shadow'
            },
            formatter: function (params) {
                var tar = params[1];
                return tar.name + '<br/>' + tar.seriesName + ' : ' + tar.value;
            }
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        xAxis: {
            type: 'category',
            splitLine: { show: false },
            data: ['总费用', '房租', '水电费', '交通费', '伙食费', '日用品数']
        },
        yAxis: {
            type: 'value'
        },
        series: [
            {
                name: '辅助',
                type: 'bar',
                stack: '总量',
                itemStyle: {
                    normal: {
                        barBorderColor: 'rgba(0,0,0,0)',
                        color: 'rgba(0,0,0,0)'
                    },
                    emphasis: {
                        barBorderColor: 'rgba(0,0,0,0)',
                        color: 'rgba(0,0,0,0)'
                    }
                },
                data: [0, 1700, 1400, 1200, 300, 0]
            },
            {
                name: '生活费',
                type: 'bar',
                stack: '总量',
                label: {
                    normal: {
                        show: true,
                        position: 'inside'
                    }
                },
                data: [2900, 1200, 300, 200, 900, 300]
            }
        ]
    };



    $scope.echartCategoryBarsOptions = {
        color: ['#003366', '#006699', '#4cabce', '#e5323e'],
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        legend: {
            data: ['Forest', 'Steppe', 'Desert', 'Wetland']
        },
        toolbox: {
            show: true,
            orient: 'vertical',
            left: 'right',
            top: 'center',
            feature: {
                mark: { show: true },
                dataView: { show: true, readOnly: false },
                magicType: { show: true, type: ['line', 'bar', 'stack', 'tiled'] },
                restore: { show: true },
                saveAsImage: { show: true }
            }
        },
        calculable: true,
        xAxis: [
            {
                type: 'category',
                axisTick: { show: false },
                data: ['2012', '2013', '2014', '2015', '2016']
            }
        ],
        yAxis: [
            {
                type: 'value'
            }
        ],
        series: [
            {
                name: 'Forest',
                type: 'bar',
                barGap: 0,
                data: [320, 332, 301, 334, 390]
            },
            {
                name: 'Steppe',
                type: 'bar',
                data: [220, 182, 191, 234, 290]
            },
            {
                name: 'Desert',
                type: 'bar',
                data: [150, 232, 201, 154, 190]
            },
            {
                name: 'Wetland',
                type: 'bar',
                data: [98, 77, 101, 99, 40]
            }
        ]
    };


    // Echarts
    $scope.echartOptions = {
        title: {
            text: 'ECharts entry example'
        },
        tooltip: {
            show: true
        },
        legend: {
            data: ['Sales']
        },
        xAxis: {
            data: ["shirt", "cardign", "chiffon shirt", "pants", "heels", "socks"]
        },
        yAxis: {},
        series: [{
            name: 'Sales',
            type: 'bar',
            data: [5, 20, 36, 10, 10, 20]
        }]
    };


    $scope.echartOptions2 = {
        title: {
            text: '未来一周气温变化(5秒后自动轮询)',
            subtext: '纯属虚构'
        },
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            data: ['最高气温', '最低气温']
        },
        toolbox: {
            show: true,
            feature: {
                mark: { show: true },
                dataView: { show: true, readOnly: false },
                magicType: { show: true, type: ['line', 'bar'] },
                restore: { show: true },
                saveAsImage: { show: true }
            }
        },
        calculable: true,
        xAxis: [
            {
                type: 'category',
                boundaryGap: false,
                data: ['周一', '周二', '周三', '周四', '周五', '周六', '周日']
            }
        ],
        yAxis: [
            {
                type: 'value',
                axisLabel: {
                    formatter: '{value} °C'
                }
            }
        ],
        series: [
            {
                name: '最高气温',
                type: 'line',
                data: [11, 11, 15, 13, 12, 13, 10],
                markPoint: {
                    data: [
                        { type: 'max', name: '最大值' },
                        { type: 'min', name: '最小值' }
                    ]
                },
                markLine: {
                    data: [
                        { type: 'average', name: '平均值' }
                    ]
                }
            },
            {
                name: '最低气温',
                type: 'line',
                data: [1, -2, 2, 5, 3, 2, 0],
                markPoint: {
                    data: [
                        { name: '周最低', value: -2, xAxis: 1, yAxis: -1.5 }
                    ]
                },
                markLine: {
                    data: [
                        { type: 'average', name: '平均值' }
                    ]
                }
            }
        ]
    };

    $scope.echartOptions3 = {
        grid: [{
            left: 50,
            right: 50,
            height: '35%'
        }, {
            left: 50,
            right: 50,
            top: '55%',
            height: '35%'
        }],
        xAxis: [
            {
                type: 'category',
                boundaryGap: false,
                data: ['周一', '周二', '周三', '周四', '周五', '周六', '周日']
            },
            {
                gridIndex: 1,
                type: 'category',
                boundaryGap: false,
                data: ['周一', '周二', '周三', '周四', '周五', '周六', '周日']
            }
        ],
        yAxis: [
            {
                type: 'value',
                axisLabel: {
                    formatter: '{value} °C'
                }
            },
            {
                gridIndex: 1,
                type: 'value',
                axisLabel: {
                    formatter: '{value} AAA'
                }
            }

        ],
        series: [
            {

                name: '最低气温',
                type: 'line',
                data: [1, -2, 2, 5, 3, 2, 0],
            },
            {
                xAxisIndex: 1,
                yAxisIndex: 1,
                name: '最低气温',
                type: 'line',
                data: [1, -2, 2, 5, 3, 2, 0],
            }
        ]
    };

    $scope.echartOptions4 = {
        xAxis: [
            {
                type: 'category',
                boundaryGap: false,
                data: ['周一', '周二', '周三', '周四', '周五', '周六', '周日']
            }
        ],
        yAxis: [
            {
                type: 'value',
                axisLabel: {
                    formatter: '{value} °C'
                }
            }
        ],
        series: [
            {

                name: '最低气温',
                type: 'line',
                data: [1, -2, 2, 5, 3, 2, 0],
            }
        ]
    };

    /**/

    var upColor = '#ec0000';
    var upBorderColor = '#8A0000';
    var downColor = '#00da3c';
    var downBorderColor = '#008F28';


    // 数据意义：开盘(open)，收盘(close)，最低(lowest)，最高(highest)
    var data0 = splitData([
        ['2013/1/24', 2320.26, 2320.26, 2287.3, 2362.94],
        ['2013/1/25', 2300, 2291.3, 2288.26, 2308.38],
        ['2013/1/28', 2295.35, 2346.5, 2295.35, 2346.92],
        ['2013/1/29', 2347.22, 2358.98, 2337.35, 2363.8],
        ['2013/1/30', 2360.75, 2382.48, 2347.89, 2383.76],
        ['2013/1/31', 2383.43, 2385.42, 2371.23, 2391.82],
        ['2013/2/1', 2377.41, 2419.02, 2369.57, 2421.15],
        ['2013/2/4', 2425.92, 2428.15, 2417.58, 2440.38],
        ['2013/2/5', 2411, 2433.13, 2403.3, 2437.42],
        ['2013/2/6', 2432.68, 2434.48, 2427.7, 2441.73],
        ['2013/2/7', 2430.69, 2418.53, 2394.22, 2433.89],
        ['2013/2/8', 2416.62, 2432.4, 2414.4, 2443.03],
        ['2013/2/18', 2441.91, 2421.56, 2415.43, 2444.8],
        ['2013/2/19', 2420.26, 2382.91, 2373.53, 2427.07],
        ['2013/2/20', 2383.49, 2397.18, 2370.61, 2397.94],
        ['2013/2/21', 2378.82, 2325.95, 2309.17, 2378.82],
        ['2013/2/22', 2322.94, 2314.16, 2308.76, 2330.88],
        ['2013/2/25', 2320.62, 2325.82, 2315.01, 2338.78],
        ['2013/2/26', 2313.74, 2293.34, 2289.89, 2340.71],
        ['2013/2/27', 2297.77, 2313.22, 2292.03, 2324.63],
        ['2013/2/28', 2322.32, 2365.59, 2308.92, 2366.16],
        ['2013/3/1', 2364.54, 2359.51, 2330.86, 2369.65],
        ['2013/3/4', 2332.08, 2273.4, 2259.25, 2333.54],
        ['2013/3/5', 2274.81, 2326.31, 2270.1, 2328.14],
        ['2013/3/6', 2333.61, 2347.18, 2321.6, 2351.44],
        ['2013/3/7', 2340.44, 2324.29, 2304.27, 2352.02],
        ['2013/3/8', 2326.42, 2318.61, 2314.59, 2333.67],
        ['2013/3/11', 2314.68, 2310.59, 2296.58, 2320.96],
        ['2013/3/12', 2309.16, 2286.6, 2264.83, 2333.29],
        ['2013/3/13', 2282.17, 2263.97, 2253.25, 2286.33],
        ['2013/3/14', 2255.77, 2270.28, 2253.31, 2276.22],
        ['2013/3/15', 2269.31, 2278.4, 2250, 2312.08],
        ['2013/3/18', 2267.29, 2240.02, 2239.21, 2276.05],
        ['2013/3/19', 2244.26, 2257.43, 2232.02, 2261.31],
        ['2013/3/20', 2257.74, 2317.37, 2257.42, 2317.86],
        ['2013/3/21', 2318.21, 2324.24, 2311.6, 2330.81],
        ['2013/3/22', 2321.4, 2328.28, 2314.97, 2332],
        ['2013/3/25', 2334.74, 2326.72, 2319.91, 2344.89],
        ['2013/3/26', 2318.58, 2297.67, 2281.12, 2319.99],
        ['2013/3/27', 2299.38, 2301.26, 2289, 2323.48],
        ['2013/3/28', 2273.55, 2236.3, 2232.91, 2273.55],
        ['2013/3/29', 2238.49, 2236.62, 2228.81, 2246.87],
        ['2013/4/1', 2229.46, 2234.4, 2227.31, 2243.95],
        ['2013/4/2', 2234.9, 2227.74, 2220.44, 2253.42],
        ['2013/4/3', 2232.69, 2225.29, 2217.25, 2241.34],
        ['2013/4/8', 2196.24, 2211.59, 2180.67, 2212.59],
        ['2013/4/9', 2215.47, 2225.77, 2215.47, 2234.73],
        ['2013/4/10', 2224.93, 2226.13, 2212.56, 2233.04],
        ['2013/4/11', 2236.98, 2219.55, 2217.26, 2242.48],
        ['2013/4/12', 2218.09, 2206.78, 2204.44, 2226.26],
        ['2013/4/15', 2199.91, 2181.94, 2177.39, 2204.99],
        ['2013/4/16', 2169.63, 2194.85, 2165.78, 2196.43],
        ['2013/4/17', 2195.03, 2193.8, 2178.47, 2197.51],
        ['2013/4/18', 2181.82, 2197.6, 2175.44, 2206.03],
        ['2013/4/19', 2201.12, 2244.64, 2200.58, 2250.11],
        ['2013/4/22', 2236.4, 2242.17, 2232.26, 2245.12],
        ['2013/4/23', 2242.62, 2184.54, 2182.81, 2242.62],
        ['2013/4/24', 2187.35, 2218.32, 2184.11, 2226.12],
        ['2013/4/25', 2213.19, 2199.31, 2191.85, 2224.63],
        ['2013/4/26', 2203.89, 2177.91, 2173.86, 2210.58],
        ['2013/5/2', 2170.78, 2174.12, 2161.14, 2179.65],
        ['2013/5/3', 2179.05, 2205.5, 2179.05, 2222.81],
        ['2013/5/6', 2212.5, 2231.17, 2212.5, 2236.07],
        ['2013/5/7', 2227.86, 2235.57, 2219.44, 2240.26],
        ['2013/5/8', 2242.39, 2246.3, 2235.42, 2255.21],
        ['2013/5/9', 2246.96, 2232.97, 2221.38, 2247.86],
        ['2013/5/10', 2228.82, 2246.83, 2225.81, 2247.67],
        ['2013/5/13', 2247.68, 2241.92, 2231.36, 2250.85],
        ['2013/5/14', 2238.9, 2217.01, 2205.87, 2239.93],
        ['2013/5/15', 2217.09, 2224.8, 2213.58, 2225.19],
        ['2013/5/16', 2221.34, 2251.81, 2210.77, 2252.87],
        ['2013/5/17', 2249.81, 2282.87, 2248.41, 2288.09],
        ['2013/5/20', 2286.33, 2299.99, 2281.9, 2309.39],
        ['2013/5/21', 2297.11, 2305.11, 2290.12, 2305.3],
        ['2013/5/22', 2303.75, 2302.4, 2292.43, 2314.18],
        ['2013/5/23', 2293.81, 2275.67, 2274.1, 2304.95],
        ['2013/5/24', 2281.45, 2288.53, 2270.25, 2292.59],
        ['2013/5/27', 2286.66, 2293.08, 2283.94, 2301.7],
        ['2013/5/28', 2293.4, 2321.32, 2281.47, 2322.1],
        ['2013/5/29', 2323.54, 2324.02, 2321.17, 2334.33],
        ['2013/5/30', 2316.25, 2317.75, 2310.49, 2325.72],
        ['2013/5/31', 2320.74, 2300.59, 2299.37, 2325.53],
        ['2013/6/3', 2300.21, 2299.25, 2294.11, 2313.43],
        ['2013/6/4', 2297.1, 2272.42, 2264.76, 2297.1],
        ['2013/6/5', 2270.71, 2270.93, 2260.87, 2276.86],
        ['2013/6/6', 2264.43, 2242.11, 2240.07, 2266.69],
        ['2013/6/7', 2242.26, 2210.9, 2205.07, 2250.63],
        ['2013/6/13', 2190.1, 2148.35, 2126.22, 2190.1]
    ]);


    function splitData(rawData) {
        var categoryData = [];
        var values = []
        for (var i = 0; i < rawData.length; i++) {
            categoryData.push(rawData[i].splice(0, 1)[0]);
            values.push(rawData[i])
        }
        return {
            categoryData: categoryData,
            values: values
        };
    }

    function calculateMA(dayCount) {
        var result = [];
        for (var i = 0, len = data0.values.length; i < len; i++) {
            if (i < dayCount) {
                result.push('-');
                continue;
            }
            var sum = 0;
            for (var j = 0; j < dayCount; j++) {
                sum += data0.values[i - j][1];
            }
            result.push(sum / dayCount);
        }
        return result;
    }



    $scope.echartOptions5 = {
        grid: [{
            left: 50,
            right: 50,
            height: '35%'
        }, {
            left: 50,
            right: 50,
            top: '55%',
            height: '35%'
        }],
        title: {
            text: '上证指数',
            left: 0
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'cross'
            }
        },
        legend: {
            data: ['日K', 'MA5', 'MA10', 'MA20', 'MA30']
        },

        xAxis: [{
            type: 'category',
            data: data0.categoryData,
            scale: true,
            boundaryGap: false,
            axisLine: { onZero: false },
            splitLine: { show: false },
            splitNumber: 20,
            min: 'dataMin',
            max: 'dataMax'
        }, {
            gridIndex: 1,
            type: 'category',
            data: data0.categoryData,
            scale: true,
            boundaryGap: false,
            axisLine: { onZero: false },
            splitLine: { show: false },
            splitNumber: 20,
            min: 'dataMin',
            max: 'dataMax'
        }],
        yAxis: [{
            scale: true,
            splitArea: {
                show: true
            }
        }, {
            gridIndex: 1,
            scale: true,
            splitArea: {
                show: true
            }
        }],
        dataZoom: [
            {
                type: 'inside',
                start: 50,
                end: 100
            },
            {
                show: true,
                type: 'slider',
                y: '90%',
                start: 50,
                end: 100
            }
        ],
        series: [
            {
                name: '日K',
                type: 'candlestick',
                data: data0.values,
                itemStyle: {
                    normal: {
                        color: upColor,
                        color0: downColor,
                        borderColor: upBorderColor,
                        borderColor0: downBorderColor
                    }
                },
                markPoint: {
                    label: {
                        normal: {
                            formatter: function (param) {
                                return param != null ? Math.round(param.value) : '';
                            }
                        }
                    },
                    data: [
                        {
                            name: 'XX标点',
                            coord: ['2013/5/31', 2300],
                            value: 2300,
                            itemStyle: {
                                normal: { color: 'rgb(41,60,85)' }
                            }
                        },
                        {
                            name: 'highest value',
                            type: 'max',
                            valueDim: 'highest'
                        },
                        {
                            name: 'lowest value',
                            type: 'min',
                            valueDim: 'lowest'
                        },
                        {
                            name: 'average value on close',
                            type: 'average',
                            valueDim: 'close'
                        }
                    ],
                    tooltip: {
                        formatter: function (param) {
                            return param.name + '<br>' + (param.data.coord || '');
                        }
                    }
                },
                markLine: {
                    symbol: ['none', 'none'],
                    data: [
                        [
                            {
                                name: 'from lowest to highest',
                                type: 'min',
                                valueDim: 'lowest',
                                symbol: 'circle',
                                symbolSize: 10,
                                label: {
                                    normal: { show: false },
                                    emphasis: { show: false }
                                }
                            },
                            {
                                type: 'max',
                                valueDim: 'highest',
                                symbol: 'circle',
                                symbolSize: 10,
                                label: {
                                    normal: { show: false },
                                    emphasis: { show: false }
                                }
                            }
                        ],
                        {
                            name: 'min line on close',
                            type: 'min',
                            valueDim: 'close'
                        },
                        {
                            name: 'max line on close',
                            type: 'max',
                            valueDim: 'close'
                        }
                    ]
                }
            },
            {
                name: 'MA5',
                type: 'line',
                data: calculateMA(5),
                smooth: true,
                lineStyle: {
                    normal: { opacity: 0.5 }
                }
            },
            {
                name: 'MA10',
                type: 'line',
                data: calculateMA(10),
                smooth: true,
                lineStyle: {
                    normal: { opacity: 0.5 }
                }
            },
            {
                name: 'MA20',
                type: 'line',
                data: calculateMA(20),
                smooth: true,
                lineStyle: {
                    normal: { opacity: 0.5 }
                }
            },
            {
                xAxisIndex: 1,
                yAxisIndex: 1,
                name: 'MA30',
                type: 'line',
                data: calculateMA(30),
                smooth: true,
                lineStyle: {
                    normal: { opacity: 0.5 }
                }
            },

        ]
    };


    $scope.echartOptions6 =
        {
            title: {
                text: '上证指数',
                left: 0
            },
            tooltip: {
                trigger: 'axis',
                axisPointer: {
                    type: 'cross'
                }
            },
            legend: {
                data: ['日K', 'MA5', 'MA10', 'MA20', 'MA30']
            },
            grid: {
                left: '10%',
                right: '10%',
                bottom: '15%'
            },
            xAxis: {
                type: 'category',
                data: data0.categoryData,
                scale: true,
                boundaryGap: false,
                axisLine: { onZero: false },
                splitLine: { show: false },
                splitNumber: 20,
                min: 'dataMin',
                max: 'dataMax'
            },
            yAxis: {
                scale: true,
                splitArea: {
                    show: true
                }
            },
            dataZoom: [
                {
                    type: 'inside',
                    start: 50,
                    end: 100
                },
                {
                    show: true,
                    type: 'slider',
                    y: '90%',
                    start: 50,
                    end: 100
                }
            ],
            series: [
                {
                    name: '日K',
                    type: 'candlestick',
                    data: data0.values,
                    itemStyle: {
                        normal: {
                            color: upColor,
                            color0: downColor,
                            borderColor: upBorderColor,
                            borderColor0: downBorderColor
                        }
                    },
                    markPoint: {
                        label: {
                            normal: {
                                formatter: function (param) {
                                    return param != null ? Math.round(param.value) : '';
                                }
                            }
                        },
                        data: [
                            {
                                name: 'XX标点',
                                coord: ['2013/5/31', 2300],
                                value: 2300,
                                itemStyle: {
                                    normal: { color: 'rgb(41,60,85)' }
                                }
                            },
                            {
                                name: 'highest value',
                                type: 'max',
                                valueDim: 'highest'
                            },
                            {
                                name: 'lowest value',
                                type: 'min',
                                valueDim: 'lowest'
                            },
                            {
                                name: 'average value on close',
                                type: 'average',
                                valueDim: 'close'
                            }
                        ],
                        tooltip: {
                            formatter: function (param) {
                                return param.name + '<br>' + (param.data.coord || '');
                            }
                        }
                    },
                    markLine: {
                        symbol: ['none', 'none'],
                        data: [
                            [
                                {
                                    name: 'from lowest to highest',
                                    type: 'min',
                                    valueDim: 'lowest',
                                    symbol: 'circle',
                                    symbolSize: 10,
                                    label: {
                                        normal: { show: false },
                                        emphasis: { show: false }
                                    }
                                },
                                {
                                    type: 'max',
                                    valueDim: 'highest',
                                    symbol: 'circle',
                                    symbolSize: 10,
                                    label: {
                                        normal: { show: false },
                                        emphasis: { show: false }
                                    }
                                }
                            ],
                            {
                                name: 'min line on close',
                                type: 'min',
                                valueDim: 'close'
                            },
                            {
                                name: 'max line on close',
                                type: 'max',
                                valueDim: 'close'
                            }
                        ]
                    }
                },
                {
                    name: 'MA5',
                    type: 'line',
                    data: calculateMA(5),
                    smooth: true,
                    lineStyle: {
                        normal: { opacity: 0.5 }
                    }
                },
                {
                    name: 'MA10',
                    type: 'line',
                    data: calculateMA(10),
                    smooth: true,
                    lineStyle: {
                        normal: { opacity: 0.5 }
                    }
                },
                {
                    name: 'MA20',
                    type: 'line',
                    data: calculateMA(20),
                    smooth: true,
                    lineStyle: {
                        normal: { opacity: 0.5 }
                    }
                },
                {
                    name: 'MA30',
                    type: 'line',
                    data: calculateMA(30),
                    smooth: true,
                    lineStyle: {
                        normal: { opacity: 0.5 }
                    }
                },

            ]
        };


    $scope.echartOptions7 = {
        title: {
            text: '折线图堆叠'
        },
        tooltip: {
            trigger: 'axis'
        },
        legend: {
            data: ['邮件营销', '联盟广告', '视频广告', '直接访问', '搜索引擎']
        },
        grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
        },
        toolbox: {
            feature: {
                saveAsImage: {}
            }
        },
        xAxis: {
            type: 'category',
            boundaryGap: false,
            data: ['周一', '周二', '周三', '周四', '周五', '周六', '周日']
        },
        yAxis: {
            type: 'value'
        },
        series: [
            {
                name: '邮件营销',
                type: 'line',
                stack: '总量',
                data: [120, 132, 101, 134, 90, 230, 210]
            },
            {
                name: '联盟广告',
                type: 'line',
                stack: '总量',
                data: [220, 182, 191, 234, 290, 330, 310]
            },
            {
                name: '视频广告',
                type: 'line',
                stack: '总量',
                data: [150, 232, 201, 154, 190, 330, 410]
            },
            {
                name: '直接访问',
                type: 'line',
                stack: '总量',
                data: [320, 332, 301, 334, 390, 330, 320]
            },
            {
                name: '搜索引擎',
                type: 'line',
                stack: '总量',
                data: [820, 932, 901, 934, 1290, 1330, 1320]
            }
        ]
    };

    /**/







    $scope.rowCollection = [
        { firstName: 'Laurent', lastName: 'Renard', birthDate: new Date('1987-05-21'), balance: 102, email: 'whatever@gmail.com' },
        { firstName: 'Blandine', lastName: 'Faivre', birthDate: new Date('1987-04-25'), balance: -2323.22, email: 'oufblandou@gmail.com' },
        { firstName: 'Francoise', lastName: 'Frere', birthDate: new Date('1955-08-27'), balance: 42343, email: 'raymondef@gmail.com' }
    ];


    $scope.standardItems = [
        { sizeX: 2, sizeY: 1, row: 0, col: 0, componentName: "foo" },
        { sizeX: 2, sizeY: 2, row: 0, col: 2, componentName: "bar" },
        { sizeX: 1, sizeY: 1, row: 0, col: 4, componentName: "qux" },
        { sizeX: 1, sizeY: 1, row: 0, col: 5 },
        { sizeX: 2, sizeY: 1, row: 1, col: 0 },
        { sizeX: 1, sizeY: 1, row: 1, col: 4 },
        { sizeX: 1, sizeY: 2, row: 1, col: 5 },
        { sizeX: 1, sizeY: 1, row: 2, col: 0 },
        { sizeX: 2, sizeY: 1, row: 2, col: 1 },
        { sizeX: 1, sizeY: 1, row: 2, col: 3 },
        { sizeX: 1, sizeY: 1, row: 2, col: 4 }
    ];


    $scope.add = function () {
        var modalInstance = $uibModal.open({
            component: "clientComponent",
            // resolve: {
            // items: function () {return $scope.items;}
            // }
        });
        modalInstance.result.then(function (selectedItem) {
            refresh();
        }, function () {
            // alert(2);
        });

    };






    (function () {
        // $rootScope.IsBusy = true;
        // dashboardService.GetProjection().then(function (resp) {
        //   $scope.projection = resp.data;
        //   $rootScope.IsBusy = false;
        // }, $rootScope.raiseErrorDelegate);
    })();
});