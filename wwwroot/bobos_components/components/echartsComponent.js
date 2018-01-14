angular.module('frontendAngularClientApp').component('eCharts', {
    template: '',
    bindings: { options: "<", style: "@" },
    controller: function ($rootScope, $state, $element, $attrs, $window, $timeout) {
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
            $element.html('<div id="' + id + '" style="height:400px; width:100%; ' + this.style + ' "></div>');

            var myChart = echarts.init(document.getElementById(id));
            if (this.options != null)
                myChart.setOption(this.options);

            alert(1);
            $timeout(function () {
                myChart.resize();
                alert(2);
            }, 1000)

        };
    }
});