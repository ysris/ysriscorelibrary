angular.module("frontendAngularClientApp").controller("CustomersController", function ($scope, $state, $rootScope, userService, customerService, $uibModal) {
    $scope.entitylist = null;

    var refresh = function () {
        $rootScope.SetPageMocked(false);
        $rootScope.IsBusy = true;
        userService.List().then(function (resp) {
            $scope.entitylist = resp.data;
            $rootScope.IsBusy = false;
        }, $rootScope.raiseErrorDelegate);
    };

    (function () {
        refresh();
    })();

    $scope.activateAcccount = function (obj) {
        $rootScope.askConfirm({
            title: "Activate this account ?",
            callBack: function (isConfirm) {
                if (isConfirm) {
                    customerService.activate(obj).then(function (resp) {
                        refresh();
                        $rootScope.addNotification("Account activated");
                    }, $rootScope.raiseErrorDelegate);
                }
            }
        });
    };
    $scope.disableAccount = function (obj) {
        $rootScope.askConfirm({
            title: "Disable this account ?",
            callBack: function (isConfirm) {
                if (isConfirm) {
                    customerService.disable(obj).then(function (resp) {
                        refresh();
                        $rootScope.addNotification("Account disabled");
                    }, $rootScope.raiseErrorDelegate);
                }
            }
        });
    };

    $scope.add = function () {
        var modalInstance = $uibModal.open({
            component: "customerComponent",
            // resolve: {
            // items: function () {return $scope.items;}
            // }
        });
        modalInstance.result.then(function (selectedItem) {            
            refresh();
        }, function () {
            // alert(2);
        });

    };
});