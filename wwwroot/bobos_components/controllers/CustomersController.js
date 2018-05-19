angular.module("frontendAngularClientApp")
    .factory("customerActivationService", function ($http, Upload) {
        return {
            activate: function (entity) { return $http.post("/api/customer/activateinvitation", entity); }
        };
    })
    .controller("ActivateCustomerInvitationController", function ($scope, $state, $rootScope, $stateParams, customerActivationService) {
        $scope.entity = { email: $state.params.email, activationcode: $state.params.activationcode };

        $scope.submit = function () {
            customerActivationService.activate($scope.entity).then(function (resp) {
                $rootScope.addNotification('Activated');
                $state.go("signin2");
            });
        }
    })
    .factory("customerService", function ($http, Upload) {
        return {
            activate: function (entity) { return $http.post("/api/customer/activateasadmin", entity); }
            , disable: function (entity) { return $http.post("/api/customer/disableasadmin", entity); }
            , GetEmptyEntity: function () { return $http.get("/api/customer/empty"); }
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
            , inviteCustomer: function (entity) { return $http.post("/api/customer/invite", entity); }
            , grantAdminRole: function (entity) { return $http.post("/api/customer/grant", { entity: entity, role: "Administrator" }); }
            , revokeAdminRole: function (entity) { return $http.post("/api/customer/revoke", { entity: entity, role: "Administrator" }); }
        };
    })
    .component('customerComponent', {
        templateUrl: 'bobos_components/views/defaultComponent.html',
        bindings: { resolve: '<', close: '&', dismiss: '&' },
        controller: function ($rootScope, customerService, $state, customerService) {
            var $ctrl = this;
            $ctrl.entity = null;

            $ctrl.modalTitle = "Customer";

            $ctrl.$onInit = function () {
                $ctrl.entity = $ctrl.resolve.entity;

                if ($ctrl.entity == null)
                    customerService.GetEmptyEntity().then(function (resp) {
                        $ctrl.entity = resp.data;
                    })
            };

            $ctrl.cancel = function () {
                $ctrl.dismiss({ $value: 'cancel' });
            };

            $ctrl.submit = function () {
                customerService.updateasadmin($ctrl.entity).then(function (resp) {
                    $rootScope.addNotification("Customer edited");
                    $ctrl.close({ $value: $ctrl.entity });
                }, $rootScope.raiseErrorDelegate);
            };
        }
    })
    .controller("CustomersController", function ($scope, $state, $rootScope, customerService, $uibModal) {
        $scope.entitylist = null;

        /**
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
         * 
         * @param {any} obj
         */
        $scope.activateAcccount = function (obj) {
            $rootScope.askConfirm({
                title: "Activate this account ?",
                callBack: function (isConfirm) {
                    if (isConfirm) {
                        customerService.activate(obj).then(function (resp) {
                            $scope.refresh();
                            $rootScope.addNotification("Account activated");
                        }, $rootScope.raiseErrorDelegate);
                    }
                }
            });
        };

        /**
         * 
         * @param {any} obj
         */
        $scope.disableAccount = function (obj) {
            $rootScope.askConfirm({
                title: "Disable this account ?",
                callBack: function (isConfirm) {
                    if (isConfirm) {
                        customerService.disable(obj).then(function (resp) {
                            $scope.refresh();
                            $rootScope.addNotification("Account disabled");
                        }, $rootScope.raiseErrorDelegate);
                    }
                }
            });
        };

        /**
         * 
         * @param {any} obj
         */
        $scope.deleteAccount = function (obj) {
            $rootScope.askConfirm({
                title: "Delete this account ?",
                callBack: function (isConfirm) {
                    if (isConfirm) {
                        customerService.delete(obj.id).then(function (resp) {
                            $scope.refresh();
                            $rootScope.addNotification("Account deleted");
                        }, $rootScope.raiseErrorDelegate);
                    }
                }
            });
        };

        /**
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
         * 
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
         * 
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