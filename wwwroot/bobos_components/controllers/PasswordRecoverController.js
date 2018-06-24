angular.module("frontendAngularClientApp")
    .controller("PasswordRecoverController", function ($scope, $state, $rootScope, customerService, $translate) {
        $scope.email = null;
        //var init = function () {
        //};
        //init();

        $scope.recover = function () {
            $rootScope.IsBusy = true;
            customerService.recover($scope.email).then(function (response) {
                $rootScope.addNotification($translate.instant('PASSWORD_RECOVERY_SENT'));
                $state.go("signin2");
                $rootScope.IsBusy = false;
            }, $rootScope.raiseErrorDelegate);
        };
    })
    .controller("PasswordRecover2Controller", function ($scope, $state, $rootScope, customerService, $stateParams, $translate) {
        $scope.email = $stateParams.email;
        $scope.activationCode = $stateParams.activationcode;
        $scope.password = null;
        $scope.passwordConfirm = null;

        $scope.recover = function () {
            $rootScope.IsBusy = true;
            customerService.recover2($scope.email, $scope.activationCode, $scope.password).then(function (response) {
                $rootScope.addNotification($translate.instant('PASSWORD_RECOVERY_DONE'));
                $state.go("signin2");
                $rootScope.IsBusy = false;
            }, $rootScope.raiseErrorDelegate);
        };
    })
    ;