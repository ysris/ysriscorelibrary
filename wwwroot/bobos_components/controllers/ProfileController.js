"use strict";
angular.module("frontendAngularClientApp")
    .controller("ProfileController", function ($scope, $state, $rootScope, customerService, $stateParams) {
        $rootScope.PageHeaderTitle = null;
        $rootScope.PageHeaderSubTitle = null;
        $scope.userEntity = null;
        var refresh = function () {
            if ($stateParams.id == "")
                $stateParams.id = null;
            customerService.get($stateParams.id).then(function (resp) {
                $scope.userEntity = resp.data;
            }, $rootScope.raiseErrorDelegate);
        };
        (function () {
            refresh();            
        })();
    }); 