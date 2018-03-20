angular.module("frontendAngularClientApp")
    .controller("PasswordRecover2Controller", ["$scope", "$state", "$rootScope", "userService", "$stateParams", function ($scope, $state, $rootScope, customerService, $stateParams) {
        $scope.email = $stateParams.email;
        $scope.activationCode = $stateParams.activationcode;
        $scope.password = null;
        $scope.passwordConfirm = null;

        //(function () {
        //})();

        $scope.recover = function () {
            $rootScope.IsBusy = true;
            customerService.recover2($scope.email, $scope.activationCode, $scope.password).then(function (response) {
                $rootScope.addNotification("Password recovery done");
                $state.go("signin2");
                $rootScope.IsBusy = false;
            }, function errorCallback(e) {
                $rootScope.addNotification("Recover error");
                $state.go("home");
                $rootScope.IsBusy = false;
            });
        };
    }]);