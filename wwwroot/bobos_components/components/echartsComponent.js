angular.module('frontendAngularClientApp').component('eCharts', {
    template: '',
    bindings: { options: "<", style: "@" },
    controller: function ($rootScope, $state, $element, $attrs, $window) {
        this.$onInit = function () {




        };

        this.$onChanges = function (changesObj) {

            var guid = function guid() {
                function s4() {
                    return Math.floor((1 + Math.random()) * 0x10000)
                        .toString(16)
                        .substring(1);
                }
                return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
                    s4() + '-' + s4() + s4() + s4();
            };



            id = guid();
            console.log(this.style);
            $element.html('<div id="' + id + '" style="height:400px; width:100%; ' + this.style + ' "></div>');

            var myChart = echarts.init(document.getElementById(id));
            myChart.setOption(this.options);
            myChart.resize();

        };
    }
});