angular.module("frontendAngularClientApp")
    .controller("PasswordRecoverController", function ($scope, $state, $rootScope, customerService) {
        $scope.email = null;
        //var init = function () {
        //};
        //init();

        $scope.recover = function () {
            $rootScope.IsBusy = true;
            customerService.recover($scope.email).then(function (response) {
                $rootScope.addNotification("Password recovery sent, please check your email");
                $state.go("signin2");
                $rootScope.IsBusy = false;
            }, function errorCallback(e) {
                $rootScope.addNotification("Recover error");
                $rootScope.IsBusy = false;
            });
        };
    })
    .controller("PasswordRecover2Controller", function ($scope, $state, $rootScope, customerService, $stateParams) {
        $scope.email = $stateParams.email;
        $scope.activationCode = $stateParams.activationcode;
        $scope.password = null;
        $scope.passwordConfirm = null;

        $scope.recover = function () {
            $rootScope.IsBusy = true;
            customerService.recover2($scope.email, $scope.activationCode, $scope.password).then(function (response) {
                $rootScope.addNotification("Password recovery done");
                $state.go("signin2");
                $rootScope.IsBusy = false;
            }, function errorCallback(e) {
                $rootScope.addNotification("Recover error");
                $rootScope.IsBusy = false;
            });
        };
    })
    ;