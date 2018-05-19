angular.module("frontendAngularClientApp")
    .component('inviteCustomerComponent', {
        templateUrl: 'bobos_components/views/inviteCustomerComponent.html',
        bindings: { resolve: '<', close: '&', dismiss: '&' },
        controller: function ($rootScope, customerService, $state, customerService) {
            var $ctrl = this;
            $ctrl.entity = { email: undefined, boolSendEmail: true };

            $ctrl.modalTitle = "Invite a Customer";

            $ctrl.$onInit = function () {
                //$ctrl.entity = $ctrl.resolve.entity;

                //if ($ctrl.entity == null)
                //    customerService.GetEmptyEntity().then(function (resp) {
                //        $ctrl.entity = resp.data;
                //    })
            };

            $ctrl.cancel = function () {
                $ctrl.dismiss({ $value: 'cancel' });
            };

            $ctrl.submit = function () {
                customerService.inviteCustomer({ entity: $ctrl.entity, boolSendEmail: $ctrl.entity.boolSendEmail }).then(function (resp) {
                    $rootScope.addNotification("Customer invited");
                    $ctrl.close({ $value: $ctrl.entity });
                }, $rootScope.raiseErrorDelegate);
            };
        }
    });