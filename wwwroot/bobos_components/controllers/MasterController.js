angular.module("frontendAngularClientApp")
    .controller("MasterController", function ($scope, $rootScope, $state, customerService, $location, SweetAlert) {
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

        $rootScope.updateConnectedUser =
            function () {
                customerService.get().then(function (resp) {
                    $rootScope.setConnectedUser(resp.data);
                    $rootScope.updateLocalSession();
                }, $rootScope.raiseErrorDelegate);
            };

        /**
         * Logout
         * @returns {} 
         */
        $rootScope.logout = function () {
            $rootScope.IsBusy = true;
            customerService.logOut().then(function () {
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

        //TODO : Refactor this : should be abstracted
        var remove = function (item, collection) { collection.splice(collection.indexOf(item), 1); }
        $scope.addEntity = function (entity, collection) { collection.push(angular.copy(entity)); };
        $scope.deleteEntity = function (entity, collection) { remove(entity, collection); };

        /**
         * 
         * @param {any} condition
         */
        $scope.showPageFor = function (condition) {
            if (angular.isArray(condition)) {
                for (var i = 0; i < condition.length; i++)
                    if ($scope.showPageFor(condition[i]))
                        return true;
                return false;
            }
            else {
                switch (condition) {
                    case "NotConnectedUser":
                        return $scope.ConnectedUserId == null || $scope.ConnectedUserId == 'null';
                    case "ConnectedUser":
                        return $scope.ConnectedUserId != null && $scope.ConnectedUserId != 'null';
                    case "ConnectedActivatedUser":
                        return ($scope.ConnectedUserId != null && $scope.ConnectedUserId != 'null') && $rootScope.customer.accountStatus == 'Activated';
                    default:
                        if ($rootScope.customer == null)
                            return false;
                        var conditions = condition.split(",");
                        for (var i in conditions)
                            if ($rootScope.customer.roles.indexOf(conditions[i].trim()) !== -1)
                                return true;
                        return false;
                }
            }
        };

        /**
         * 
         * @param {any} notificationMsg
         */
        $rootScope.addNotification = function (notificationMsg, notifcationType) {
            if (notifcationType == null || notifcationType == undefined)
                notifcationType = 'success';
            var n = new PNotify({ title: notificationMsg, type: notifcationType, styling: 'bootstrap3', delay: 2000, buttons: { closer: false, sticker: false } });
            n.get().click(function () {
                n.remove();
            });

            $rootScope.ConnectedUserNotifications.push(notificationMsg);

        };

        /**
         * 
         */
        $rootScope.removeNotifications = function () {
            $rootScope.ConnectedUserNotifications = null;
            window.localStorage.ConnectedUserNotifications = null;
        }

        /**
         * When a module has something to complain about, it comes here
         */
        $rootScope.raiseErrorDelegate = function (e) {
            console.log("ERROR", e);
            if (e.data.error != null) {
                $rootScope.addNotification(e.data.error, 'error');
            }
            if (e.statusText != null) {
                $rootScope.addNotification(e.statusText, 'error');
            }

            if (e.statusText == "Unauthorized")
                $rootScope.killSessionLoca();

            $rootScope.IsBusy = false;
        };

        $rootScope.killSessionLoca = function () {

            window.localStorage.id = null;
            $rootScope.ConnectedUserId = window.localStorage.id;
            $rootScope.customerType = null;
            window.localStorage.customerType = null;
            $rootScope.customerType = window.localStorage.customerType;
            $rootScope.customer = null;
            window.localStorage.customer = null;


            $state.go("signin2");

        }

        /**
         * 
         * @param {any} params
         */
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

        /**
         * 
         * @param {any} title
         * @param {any} subtitle
         */
        $rootScope.SetPageTitle = function (title, subtitle) {
            $rootScope.PageHeaderTitle = title;
            $rootScope.PageHeaderSubTitle = subtitle;
        };

        /**
         * 
         * @param {any} choice
         */
        $rootScope.SetPageMocked = function (choice) {
            $rootScope.IsMockedPage = choice;

        };
    });