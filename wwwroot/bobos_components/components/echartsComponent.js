angular.module('frontendAngularClientApp').component('eCharts', {
    template: '',
    bindings: { options: "<", style: "@" },
    controller: function ($rootScope, $state, $element, $attrs, $window, $interval, $timeout) {
        var $ctrl = this;
        this.$onInit = function () {
        };



        this.$onChanges = function (changesObj) {
            var guid = function guid() {
                function s4() {
                    return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
                }
                return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
            };

            id = guid();
            $element.html('<div id="' + id + '" style="height:400px; ' + this.style + ' "></div>');

            //passer un parameter pour dire si on doit refresh every x seconds

            var xxx = echarts.init(document.getElementById(id));
            $ctrl.myChart = xxx;
            if (this.options != null)
                $ctrl.myChart.setOption(this.options);


            $rootScope.$on('onChartRefresh', function () {
                $timeout(function () {
                    $ctrl.myChart.resize();
                }, 1);
            });
            //angular.element($window).bind('resize', function () {
            //});

        };
    }
});