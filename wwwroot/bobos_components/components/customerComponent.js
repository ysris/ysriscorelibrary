angular.module("frontendAngularClientApp")
    .component('customerComponent', {
        templateUrl: 'bobos_components/views/defaultComponent.html',
        bindings: { resolve: '<', close: '&', dismiss: '&' },
        controller: function ($rootScope, customerService, $state, customerService) {
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
                customerService.updateasadmin($ctrl.entity).then(function (resp) {
                    $rootScope.addNotification("Customer edited");
                    $ctrl.close({ $value: $ctrl.entity });
                }, $rootScope.raiseErrorDelegate);
            };
        }
    })