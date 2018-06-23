angular.module("frontendAngularClientApp")
    .factory("customerService", function ($http, Upload) {
        return {
            activateinvitation: function (entity) { return $http.post("/api/customer/activateinvitation", entity); }
            , activate: function (entity) { return $http.post("/api/customer/activateasadmin", entity); }
            , disable: function (entity) { return $http.post("/api/customer/disableasadmin", entity); }
            , GetEmptyEntity: function () { return $http.get("/api/customer/empty"); }
            , ping: function () { return $http.get("/api/customer/ping"); }
            , List: function () { return $http.get("/api/customer"); }
            , createAccount: function (entity) { return $http.post("/api/customer", entity); }
            , logIn: function (username, password) { return $http.post("/api/customer/Login", { username: username, password: password }); }
            , logOut: function () { return $http.post("/api/customer/Logout"); }
            , recover: function (email) { return $http.post("/api/customer/recover", { email: email }); }
            , recover2: function (email, activationcode, password) { return $http.post("/api/customer/recover2", { email: email, activationCode: activationcode, password: password }, { 'Content-Type': 'application/json' }); }
            , get: function (id) {
                if (id == null)
                    return $http.get("/api/customer/me"); 
                return $http.get("/api/customer/" + id);
            }
            , update: function (entity) { return $http.post("/api/customer/update", entity); }
            , updateasadmin: function (entity) { return $http.post("/api/customer/updateasadmin", entity); }
            , uploadAvatar: function (file) { return Upload.upload({ url: "/api/customer/avatar/", data: { 'file': file } }) }
            , GetProjection: function () { return $http.get("/api/customer/projection"); }
            , delete: function (id) {
                if (id == undefined)
                    return $http.delete("/api/customer");
                return $http.delete("/api/customer/" + id);
            }
            , getInvite: function (entity) { return $http.get("/api/customer/invite"); }
            , postInvite: function (entity) { return $http.post("/api/customer/invite", entity); }
            , grantAdminRole: function (entity) { return $http.post("/api/customer/grant", { entity: entity, role: "Administrator" }); }
            , revokeAdminRole: function (entity) { return $http.post("/api/customer/revoke", { entity: entity, role: "Administrator" }); }
        };
    })
     
    .controller("ActivateCustomerInvitationController", function ($scope, $state, $rootScope, $stateParams, customerService) {
        $scope.entity = { email: $state.params.email, activationcode: $state.params.activationcode };

        $scope.submit = function () {
            customerService.activateinvitation($scope.entity).then(function (resp) {
                $rootScope.addNotification('Activated');
                $state.go("signin2");
            });
        }
    })
    .controller("CustomersController", function ($scope, $state, $rootScope, customerService, $uibModal) {
        $scope.entitylist = null;

        /**
         * Refresh accounts list
         */
        $scope.refresh = function () {
            $rootScope.SetPageTitle("Customers", "");
            $rootScope.SetPageMocked(false);
            $rootScope.IsBusy = true;
            customerService.List().then(function (resp) {
                $scope.entitylist = resp.data;
                $rootScope.IsBusy = false;
            }, $rootScope.raiseErrorDelegate);
        };

        /**
         * Activate an account
         * @param {any} entity
         */
        $scope.activateAcccount = function (entity) {
            $rootScope.askConfirm({
                title: "Activate this account ?",
                callBack: function (isConfirm) {
                    if (isConfirm) {
                        customerService.activate(entity).then(function (resp) {
                            $scope.refresh();
                            $rootScope.addNotification("Account activated");
                        }, $rootScope.raiseErrorDelegate);
                    }
                }
            });
        };

        /**
         * Disable an account
         * @param {any} obj
         */
        $scope.disableAccount = function (entity) {
            $rootScope.askConfirm({
                title: "Disable this account ?",
                callBack: function (isConfirm) {
                    if (isConfirm) {
                        customerService.disable(entity).then(function (resp) {
                            $scope.refresh();
                            $rootScope.addNotification("Account disabled");
                        }, $rootScope.raiseErrorDelegate);
                    }
                }
            });
        };

        /**
         * Delete an account
         * @param {any} obj
         */
        $scope.deleteAccount = function (entity) {
            $rootScope.askConfirm({
                title: "Delete this account ?",
                callBack: function (isConfirm) {
                    if (isConfirm) {
                        customerService.delete(entity.id).then(function (resp) {
                            $scope.refresh();
                            $rootScope.addNotification("Account deleted");
                        }, $rootScope.raiseErrorDelegate);
                    }
                }
            });
        };

        /**
         * Display the modal for customer creation
         */
        $scope.add = function () {
            var modalInstance = $uibModal.open({
                component: "customerComponent",
                // resolve: {
                // items: function () {return $scope.items;}
                // }
            });
            modalInstance.result.then(function (selectedItem) {
                $scope.refresh();
            }, function () {
                // alert(2);
            });

        };

        /**
         * Display the modal for customer edition
         * @param {any} entity
         */
        $scope.edit = function (entity) {
            var modalInstance = $uibModal.open({
                component: "customerComponent",
                resolve: { entity: function () { return entity; } }
            });
            modalInstance.result.then(function (selectedItem) {
                $scope.refresh();
            }, function () {
                // alert(2);
            });

        };

        /**
         * Invite a customer
         */
        $scope.invite = function () {
            var modalInstance = $uibModal.open({
                component: "inviteCustomerComponent",
                // resolve: { items: function () {return $scope.items;} }
            });
            modalInstance.result.then(function (selectedItem) {
                $scope.refresh();
            }, function () {
                // error fallback
            });

        };

        /**
         * Grant admin role to a customer
         * @param {any} obj
         * @return void
         */
        $scope.grantAdminRole = function (entity) {
            if (window.confirm("Put this account as admin ?")) {
                customerService.grantAdminRole(entity).then(function (resp) {
                    $scope.refresh();
                    $rootScope.addNotification("Account granted as admin");
                }, $rootScope.raiseErrorDelegate);
            }
        };

        /**
         * Revoke admin role from a customer
         * @param {any} obj
         * @return void
         */
        $scope.revokeAdminRole = function (entity) {
            if (window.confirm("Revoke this account as admin ?")) {
                customerService.revokeAdminRole(entity).then(function (resp) {
                    $scope.refresh();
                    $rootScope.addNotification("Account revoked from admin");
                }, $rootScope.raiseErrorDelegate);
            }
        };


        /**
         * Returns if the user is an admin of the app
         * @param {any} entity
         * @return bool
         */
        $scope.hasAdminRole = function (entity) {
            return entity.roles.indexOf("Administrator") != -1;
        }


        $scope.refresh();
    })
    ;