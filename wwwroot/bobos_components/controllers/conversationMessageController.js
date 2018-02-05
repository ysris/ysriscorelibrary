angular.module("frontendAngularClientApp")
    .factory("conversationMessageService", function ($http, Upload, $rootScope) {
        var querystring = "";
        return {
            listForDestCustomer: function (customerDestId) { return $http.get("/api/conversationmessage/getforcustomer/" + customerDestId + querystring); },
            sendMessage: function (entity) { return $http.post("/api/conversationmessage", entity); }
        };
    })
    .controller("conversationMessagesController", function ($scope, $state, $stateParams, $rootScope, conversationMessageService) {
        $scope.destCustomerId = $stateParams.destCustomerid;

        $scope.entitylist = null;
        $scope.entity = null;

        $scope.refresh = function () {
            $rootScope.IsBusy = true;
            conversationMessageService.listForDestCustomer($scope.destCustomerId).then(function (result) {
                $scope.entitylist = result.data;
                $rootScope.SetPageTitle("Conversations", "");
                $rootScope.IsBusy = false;
            }, $rootScope.raiseErrorDelegate);
        };

        $scope.send = function () {
            var entity = { message: $scope.entity.message, destId: $scope.destCustomerId };
            $rootScope.IsBusy = true;
            conversationMessageService.sendMessage(entity).then(function () {
                $rootScope.IsBusy = false;
                $scope.refresh();
            }, $rootScope.raiseErrorDelegate);
        };

        $scope.refresh();
    })
    //.controller("conversationMessageController", function ($scope, $state, $rootScope, $stateParams, conversationMessageService) {
    //    var refresh = function () {
    //        $rootScope.IsBusy = true;
    //        conversationMessageService.Get($stateParams.id).then(function (resp) {
    //            $scope.entity = resp.data.entity;
    //            $scope.trends = resp.data.trends;
    //            $scope.echartOptions5 =
    //                {
    //                    grid: [
    //                        { left: 50, right: 50, height: '33%' }
    //                        , { left: 50, right: 50, top: '47%', height: '10%' }
    //                        , { left: 50, right: 50, top: '62%', height: '10%' }
    //                        , { left: 50, right: 50, top: '75%', height: '10%' }
    //                    ],
    //                    // title: { text: $scope.entity.prettyName, left: 0 },
    //                    tooltip: { trigger: 'axis', axisPointer: { type: 'cross' } },
    //                    legend: {
    //                        data: [$scope.entity.prettyName, 'SMA50', 'SMA200', 'RSI14', 'macd1Hist', 'macd1MACD', 'macd1Signal', 'macd2Hist', 'macd2MACD', 'macd2Signal']
    //                    },
    //                    xAxis: [
    //                        {
    //                            gridIndex: 0,
    //                            type: 'category',
    //                            data: $scope.entity.chartTimeSerie.categoryData,
    //                            scale: true, boundaryGap: false, axisLine: { onZero: false }, splitLine: { show: false }, splitNumber: 20,
    //                            min: 'dataMin', max: 'dataMax'
    //                        }
    //                        , {
    //                            gridIndex: 1,
    //                            type: 'category',
    //                            data: $scope.entity.chartTimeSerie.categoryData,
    //                            scale: true, boundaryGap: false, axisLine: { onZero: false }, splitLine: { show: false }, splitNumber: 20,
    //                            min: 'dataMin', max: 'dataMax'
    //                        }
    //                        , {
    //                            gridIndex: 2,
    //                            type: 'category',
    //                            data: $scope.entity.chartTimeSerie.categoryData,
    //                            scale: true, boundaryGap: false, axisLine: { onZero: false }, splitLine: { show: false }, splitNumber: 20,
    //                            min: 'dataMin', max: 'dataMax'
    //                        }
    //                        , {
    //                            gridIndex: 3,
    //                            type: 'category',
    //                            data: $scope.entity.chartTimeSerie.categoryData,
    //                            scale: true, boundaryGap: false, axisLine: { onZero: false }, splitLine: { show: false }, splitNumber: 20,
    //                            min: 'dataMin', max: 'dataMax'
    //                        }
    //                    ],
    //                    yAxis: [
    //                        { gridIndex: 0, scale: true, splitArea: { show: true }, min: 'dataMin', max: 'dataMax' }
    //                        , { gridIndex: 1, scale: true, splitArea: { show: true }, min: 'dataMin', max: 'dataMax' }
    //                        , { gridIndex: 2, scale: true, splitArea: { show: true }, min: 'dataMin', max: 'dataMax' }
    //                        , { gridIndex: 3, scale: true, splitArea: { show: true }, min: 'dataMin', max: 'dataMax' }
    //                    ],
    //                    dataZoom: [
    //                        { xAxisIndex: [0, 1, 2, 3], type: 'inside', start: 50, end: 100 },
    //                        { xAxisIndex: [0, 1, 2, 3], show: true, type: 'slider', y: '90%', start: 50, end: 100 }
    //                    ],
    //                    series: [
    //                        {
    //                            xAxisIndex: 0, yAxisIndex: 0, name: $scope.entity.prettyName, type: 'candlestick', data: $scope.entity.chartTimeSerie.values,
    //                            itemStyle: {
    //                                normal: {
    //                                    color: upColor,
    //                                    color0: downColor,
    //                                    borderColor: null,
    //                                    borderColor0: null
    //                                }
    //                            },
    //                        }
    //                        , { xAxisIndex: 0, yAxisIndex: 0, name: 'keySr', type: 'line', data: $scope.entity.indicators.keySr, smooth: true }
    //                        , { xAxisIndex: 0, yAxisIndex: 0, name: 'TypicalPrice', type: 'line', data: $scope.entity.indicators.typicalPrice, smooth: true }
    //                        , { xAxisIndex: 0, yAxisIndex: 0, name: 'SMA50', type: 'line', data: $scope.entity.indicators.SMA50, smooth: true }
    //                        , { xAxisIndex: 0, yAxisIndex: 0, name: 'SMA200', type: 'line', data: $scope.entity.indicators.SMA200, smooth: true }
    //                        //, { xAxisIndex: 1, yAxisIndex: 1, name: 'macd1Hist', type: 'line', data: $scope.entity.indicators.macd1Hist, smooth: true }
    //                        , { xAxisIndex: 1, yAxisIndex: 1, name: 'macd1MACD', type: 'line', data: $scope.entity.indicators.macd1MACD, smooth: true }
    //                        , { xAxisIndex: 1, yAxisIndex: 1, name: 'macd1Signal', type: 'line', data: $scope.entity.indicators.macd1Signal, smooth: true }
    //                        //, { xAxisIndex: 2, yAxisIndex: 2, name: 'macd2Hist', type: 'line', data: $scope.entity.indicators.macd2Hist, smooth: true }
    //                        , { xAxisIndex: 2, yAxisIndex: 2, name: 'macd2MACD', type: 'line', data: $scope.entity.indicators.macd2MACD, smooth: true }
    //                        , { xAxisIndex: 2, yAxisIndex: 2, name: 'macd2Signal', type: 'line', data: $scope.entity.indicators.macd2Signal, smooth: true }
    //                        , { xAxisIndex: 3, yAxisIndex: 3, name: 'RSI14', type: 'line', data: $scope.entity.indicators.RSI14, smooth: true }
    //                    ]
    //                };
    //            $rootScope.SetPageTitle("Instrument", $scope.entity.prettyName);
    //            $rootScope.IsBusy = false;
    //        }, $rootScope.raiseErrorDelegate);
    //    };
    //    $scope.favorite = function (obj) { instrumentService.addFavorite(obj).then(function (resp) { refresh(); $rootScope.addNotification("Added favorite"); }); };
    //    $scope.unfavorite = function (obj) { instrumentService.unFavorite(obj).then(function (resp) { refresh(); $rootScope.addNotification("Removed favorite"); }); };
    //    (function () { refresh(); })();
    //})    
    ;