/** THEBOBOS**/
"use strict";
angular.module("frontendAngularClientApp").controller("MasterController", function ($uibModal, $scope, $rootScope, $state, userService, $location, SweetAlert) {
    (function () {
        $rootScope.$on('$locationChangeSuccess',
            function (cur, next) {
                $rootScope.CurrentPage = $location.path().slice(1);
            });
    })();

    /**
     * Set the local connected user
     * @param {} id userId
     */
    $rootScope.setConnectedUser =
        function (userEntity) {
            window.localStorage.id = userEntity.id;
            window.localStorage.customerType = userEntity.customerType;
            window.localStorage.customer = JSON.stringify(userEntity);

            $rootScope.ConnectedUserId = userEntity.id;
            $rootScope.customerType = userEntity.customerType;
            $rootScope.customer = userEntity;
        };

    /**
     * Logout
     * @returns {} 
     */
    $rootScope.logout = function () {
        $rootScope.IsBusy = true;
        userService.logOut().then(function () {
            $rootScope.removeNotifications();
            window.localStorage.id = null;
            window.localStorage.customerType = null;
            window.localStorage.customer = null;

            $rootScope.ConnectedUserId = null;
            $rootScope.customerType = null;
            $rootScope.customer = null;
            $rootScope.IsBusy = false;
            $state.go("signin2");
        });
    }

    $rootScope.toggleDebugDisplay = function () {
        $rootScope.debugDisplay = window.localStorage.debugDisplay == 0 || window.localStorage.debugDisplay == undefined ? 1 : 0;
        window.localStorage.debugDisplay = $rootScope.debugDisplay;
    };


    //TODO : Refactor this : should be abstracted
    var remove = function (item, collection) {
        var index = collection.indexOf(item);
        collection.splice(index, 1);
    }

    $scope.addEntity = function (entity, collection) { collection.push(angular.copy(entity)); };
    $scope.deleteEntity = function (entity, collection) { remove(entity, collection); };

    // Menu utilities //

    $scope.showPageFor = function (condition) {


        switch (condition) {
            case "NotConnectedUser":
                return $scope.ConnectedUserId == null || $scope.ConnectedUserId == 'null';
            default:

                if ($rootScope.customer == null)
                    return false;

                //for condition Locataire => check  if there is Locataire in the roles... and so on
                var conditions = condition.split(",");
                for (var i in conditions) {
                    if ($rootScope.customer.roles.indexOf(conditions[i].trim()) !== -1)
                        return true;
                }
                return false;
        }
    };


    $rootScope.addNotification = function (notificationMsg) {
        //if ($rootScope.ConnectedUserNotifications == null)
        //    $rootScope.ConnectedUserNotifications = [];
        //$rootScope.ConnectedUserNotifications.push({ id: $rootScope.ConnectedUserNotifications.length + 1, msg: notificationMsg });
        //window.localStorage.ConnectedUserNotifications = JSON.stringify($rootScope.ConnectedUserNotifications);
        var n = new PNotify({ title: notificationMsg, text: notificationMsg, type: 'success', styling: 'bootstrap3', delay: 2000, buttons: { closer: false, sticker: false } });
        n.get().click(function () {
            n.remove();
        });
    };

    $rootScope.removeNotifications = function () {
        $rootScope.ConnectedUserNotifications = null;
        window.localStorage.ConnectedUserNotifications = null;
    }

    /**
     * When something has something to complain about, it comes here
     */
    $rootScope.raiseErrorDelegate = function (e) {
        console.log("rootScope.raiseErrorDelegate->e", e);
        $rootScope.addNotification(e.statusText + " " + e.data.ExceptionMessage + " " + e.data.error);
        $rootScope.IsBusy = false;
    };



    $rootScope.updateLocalSession = function () {
        // User id
        $rootScope.ConnectedUserId = null;
        if (window.localStorage.id != null)
            $rootScope.ConnectedUserId = window.localStorage.id;
        $rootScope.customerType = null;
        if (window.localStorage.customerType != null)
            $rootScope.customerType = window.localStorage.customerType;
        $rootScope.customer = null;
        if (window.localStorage.customer != null)
            $rootScope.customer = JSON.parse(window.localStorage.customer);



        // Notifications
        $rootScope.ConnectedUserNotifications = [];
        if (window.localStorage.ConnectedUserNotifications != null)
            $rootScope.ConnectedUserNotifications = JSON.parse(window.localStorage.ConnectedUserNotifications);

        userService.get().then(
            function (resp) {

            }, function (e) {
                //error : session lost => gotosignin
                window.localStorage.clear();
                $state.go("signin", { suppl: null });
            });

    };

    $rootScope.askConfirm = function (params) {
        SweetAlert.swal(
            {
                title: params.title,
                text: params.title,
                type: "warning",
                showCancelButton: true,
                //confirmButtonColor: "#DD6B55",
                confirmButtonText: "Confirm",
                cancelButtonText: "Cancel",
                closeOnConfirm: true,
                closeOnCancel: true
            },
            params.callBack);
    };






    $rootScope.SetPageTitle = function (title, subtitle) {
        $rootScope.PageHeaderTitle = title;
        $rootScope.PageHeaderSubTitle = subtitle;
    };

    $rootScope.SetPageMocked = function (choice) {
        $rootScope.IsMockedPage = choice;

    };


    //
    // Custom
    //

    $rootScope.inscriptionmodal = function (userType) {

        var modalInstance = $uibModal.open({
            component: 'inscriptionModalComponent',
            resolve: {
                userType: function () {
                    return userType;
                }
            }
        });
        modalInstance.result.then(function (selectedItem) {
            // alert(1);
        }, function () {
            // alert(2);
        });

    };

    $rootScope.loginmodal = function () {
        var modalInstance = $uibModal.open({
            component: 'loginModalComponent',
            // resolve: {
            //   items: function () {
            //     return $scope.items;
            //   }
            // }
        });
        modalInstance.result.then(function (selectedItem) {
            // alert(1);
        }, function () {
            // alert(2);
        });
    };





});