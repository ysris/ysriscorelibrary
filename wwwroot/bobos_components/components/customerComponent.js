angular.module('frontendAngularClientApp').component('customerComponent', {
  templateUrl: '/bobos_components/views/defaultComponent.html',
  bindings: { resolve: '<', close: '&', dismiss: '&' },
  controller: function ($rootScope, userService, $state, customerService, clientService) {
    var $ctrl = this;
    $ctrl.entity = null;

    $ctrl.modalTitle = "Customer";

    $ctrl.$onInit = function () {
      $ctrl.entity = $ctrl.resolve.entity;

      if ($ctrl.entity == null)
        customerService.GetEmptyEntity().then(function (resp) {
          $ctrl.entity = resp.data;
        })
    };

    $ctrl.cancel = function () {
      $ctrl.dismiss({ $value: 'cancel' });
    };

    $ctrl.submit = function () {
      console.log("entity", $ctrl.entity);
      userService.createAccount($ctrl.entity).then(function (resp) {
        $rootScope.addNotification("Customer added");
        $ctrl.close({ $value: $ctrl.entity });
      }, $rootScope.raiseErrorDelegate);
    };
  }
});