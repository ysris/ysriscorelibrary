angular.module("frontendAngularClientApp").controller("PasswordRecoverController", function ($scope, $state, $rootScope, customerService) {
    $scope.email = null;
    //var init = function () {
    //};
    //init();

    $scope.recover = function () {
        $rootScope.IsBusy = true;
        customerService.recover($scope.email).then(function (response) {
            $rootScope.addNotification("Password recovery sent, please check your email");
            $state.go("home");
            $rootScope.IsBusy = false;
        }, function errorCallback(e) {
            $rootScope.addNotification("Recover error");
            $state.go("home");
            $rootScope.IsBusy = false;
        });
    };
});