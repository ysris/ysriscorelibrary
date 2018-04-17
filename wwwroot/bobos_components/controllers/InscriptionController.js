angular.module("frontendAngularClientApp").controller("InscriptionController", function ($scope, $state, $rootScope, customerService) {
    $scope.Entity = null;
    $scope.customerType = null;

    (function () {
        switch ($state.current.name) {
            case "inscription":
                $scope.customerType = "User";
                break;
        }
    })();

    $scope.save = function () {
        $rootScope.IsBusy = true;
        $scope.Entity.customerType = $scope.customerType;
        customerService.createAccount($scope.Entity).then(
            function (resp) {
                $rootScope.IsBusy = false;
                $rootScope.addNotification("Account created, please check your email");
                $state.go("signin2");
            },
            $rootScope.raiseErrorDelegate
        );
    };
});