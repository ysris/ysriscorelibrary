"use strict";

angular.module("frontendAngularClientApp")
    .controller("CustomersController", ["$scope", "$state", "$rootScope", "userService", function ($scope, $state, $rootScope, userService) {
        $scope.entitylist = null;
        (function () {
            $rootScope.IsBusy = true;
            userService.List().then(function (resp) {
                $scope.entitylist = resp.data;
                $rootScope.IsBusy = false;
            }, $rootScope.raiseErrorDelegate);
        })();
    }]);