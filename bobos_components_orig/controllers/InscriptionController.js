"use strict";

angular.module("frontendAngularClientApp").controller("InscriptionController", ["$scope", "$state", "$rootScope", "userService", function ($scope, $state, $rootScope, userService) {
    $scope.CompanyName = "Sekoya";
    $scope.Entity = null;
    $scope.UserType = null;

    (function () {
        switch ($state.current.name) {
            case "inscription":
                $scope.UserType = "User";
                break;   
        }
    })();

    $scope.save = function () {
        $rootScope.IsBusy = true;
        $scope.Entity.UserType = $scope.UserType;
        userService.createAccount($scope.Entity).then(
            function (resp) {
                $rootScope.IsBusy = false;
                $rootScope.addNotification("Compte créé, merci de vérifier votre email");
                $state.go("home");
            },
            $rootScope.raiseErrorDelegate
        );
    };
}]);