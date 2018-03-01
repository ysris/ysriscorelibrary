angular.module("frontendAngularClientApp")
    .factory("customerActivationService", function ($http, Upload) {
        return {
            inviteCustomer: function (entity) { return $http.post("/api/customer/activateinvitation", entity); }
        };
    })
    .controller("ActivateCustomerInvitationController", function ($scope, $state, $rootScope, $stateParams, customerActivationService) {
        $scope.entity = { email: $state.params.email, activationcode: $state.params.activationcode };

        $scope.submit = function () {
            customerActivationService.inviteCustomer($scope.entity).then(function (resp) {
                $rootScope.addNotification('Activated');
                $state.go("signin2");
            });
        }
    })
    ;