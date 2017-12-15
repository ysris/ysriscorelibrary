"use strict";

angular.module("frontendAngularClientApp")
    .controller("SignInController", ["$scope", "$state", "$rootScope", "$stateParams", "userService", function ($scope, $state, $rootScope, $stateParams, userService) {
        var suppl = $stateParams.suppl;
        $rootScope.IsBusy = false;
        $scope.login = "";
        $scope.password = "";

        (function () {
            $rootScope.SetPageTitle("", "");

            if (suppl == "activationsucceeded") {
                //Show activationsucceeded popup
                new PNotify({ title: "Account activation succeeded", text: "You can now signin", type: "success", styling: "bootstrap3", delay: 2000 });
            }
        })();

        $scope.signIn = function () {
            $rootScope.IsBusy = true;
            userService.logIn($scope.login, $scope.password).then(function (response) {
                $rootScope.setConnectedUser(response.data);
                $rootScope.avatarUri = "/api/customer/avatar";
                $state.go("dashboard");
                $rootScope.IsBusy = false;
            }, function errorCallback(e) {
                new PNotify({ title: "Login error", text: e.data.error, type: "error", styling: "bootstrap3", delay: 2000 });
                $state.go("home");
                $rootScope.IsBusy = false;
            });
        };
    }]);