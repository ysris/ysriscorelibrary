"use strict";

angular.module("frontendAngularClientApp")
    .controller("PasswordRecoverController", ["$scope", "$state", "$rootScope", "userService", function ($scope, $state, $rootScope, userService) {
        $scope.email = null;
        //var init = function () {
        //};
        //init();

        $scope.recover = function () {
            $rootScope.IsBusy = true;
            userService.recover($scope.email).then(function (response) {
                $rootScope.addNotification("Password recovery sent, please check your email");
                $state.go("home");
                $rootScope.IsBusy = false;
            }, function errorCallback(e) {
                $rootScope.addNotification("Recover error");
                $state.go("home");
                $rootScope.IsBusy = false;
            });
        };
    }]);