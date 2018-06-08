﻿angular.module("frontendAngularClientApp")
    .controller("ProfileEditController", function ($scope, $state, $rootScope, customerService) {
        $scope.Entity = null;
        $scope.ProfilePicPreview = "/api/customer/avatar";
        $rootScope.ProgressPct = null;

        $scope.subNavigation = "profileedit";
        $scope.subNavigate = function (obj) {
            $scope.subNavigation = obj;
        };

        (function () {
            customerService.get().then(function (resp) {
                $scope.Entity = resp.data;
            });
        })();

        $scope.submitProfileEdit = function () {
            $rootScope.IsBusy = true;
            customerService.update($scope.Entity).then(function (resp) {
                $scope.Entity = resp.data;
                $rootScope.setConnectedUser($scope.Entity);

                $rootScope.addNotification("Profile updated");
                $rootScope.IsBusy = false;


            }, $rootScope.raiseErrorDelegate);
        };


        $scope.setImg = function () {
            if ($scope.newFile != null) {
                $rootScope.IsBusy = true;
                customerService.uploadAvatar($scope.newFile).then(
                    function (resp) {
                        //$scope.Entity = resp.data;
                        //$rootScope.setConnectedUser($scope.Entity);

                        var newUrl = window.URL || window.webkitURL;
                        if (newUrl) {
                            $scope.ProfilePicPreview = newUrl.createObjectURL($scope.newFile);
                            $rootScope.avatarUri = newUrl.createObjectURL($scope.newFile);
                        }

                        $rootScope.addNotification("Profile pic uploaded");
                        $rootScope.IsBusy = false;
                    },
                    $rootScope.raiseErrorDelegate,
                    function (evt) { $rootScope.ProgressPct = parseInt(100.0 * evt.loaded / evt.total); });
            }
        };

        $scope.deleteAccount = function () {
            $rootScope.askConfirm({
                title: "Are you sure ?",
                callBack: function (isConfirm) {
                    if (isConfirm) {
                        customerService.delete().then(
                            function (resp) {
                                $rootScope.logout();
                                $rootScope.addNotification("Account deleted");
                            },
                            $rootScope.raiseErrorDelegate);
                    }
                }
            });
        };
    });