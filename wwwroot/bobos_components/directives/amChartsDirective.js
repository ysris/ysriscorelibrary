angular.module("frontendAngularClientApp").directive('amChart', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            config: '='
        },
        template: '<div style="margin: 0 auto"></div>',
        link: function (scope, element, attrs) {
            var getIdForUseInAmCharts = function () {
                var id = scope.id;// try to use existing outer id to create new id

                if (!id) {//generate a UUID
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
                }
                return id;
            };

            var id = getIdForUseInAmCharts();
            element.attr('id', id);

            var chart = false;
            var initChart = function () {
                if (chart) chart.destroy();
                chart = AmCharts.makeChart(id, scope.config);
            };
            initChart();
        }//end watch           
    }
});