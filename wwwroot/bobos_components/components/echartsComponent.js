angular.module('frontendAngularClientApp').component('eCharts', {
  template: '',
  bindings: { options: "=" },
  controller: function ($rootScope, $state, $element, $attrs, $window) {
    this.$onInit = function () {



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
      $element.html('<div id="' + id + '" style="height:400px; width:100%;"></div>');

      var myChart = echarts.init(document.getElementById(id));
      myChart.setOption(this.options);
      myChart.resize();

    };
  }
});