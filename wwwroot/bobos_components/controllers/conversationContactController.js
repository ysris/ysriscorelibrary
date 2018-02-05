﻿angular.module("frontendAngularClientApp")
    .factory("conversationContactService", function ($http, Upload, $rootScope) {
        var querystring = "";
        return {
            list: function (customerDestId) { return $http.get("/api/conversationcontact/" + querystring); },
            //sendMessage: function (entity) { return $http.post("/api/conversationmessage", entity); }
        };
    })
    .controller("conversationContactsController", function ($scope, $state, $stateParams, $rootScope, conversationContactService) {
        $scope.entitylist = null;

        $scope.refresh = function () {
            $rootScope.IsBusy = true;
            conversationContactService.list().then(function (result) {
                $scope.entitylist = result.data;
                $rootScope.SetPageTitle("Contacts", "");
                $rootScope.IsBusy = false;
            }, $rootScope.raiseErrorDelegate);
        };

        //$scope.send = function () {
        //    var entity = { message: $scope.entity.message, destId: $scope.destCustomerId };
        //    $rootScope.IsBusy = true;
        //    conversationMessageService.sendMessage(entity).then(function () {
        //        $rootScope.IsBusy = false;
        //        $scope.refresh();
        //    }, $rootScope.raiseErrorDelegate);
        //};

        $scope.refresh();
    })
    ;