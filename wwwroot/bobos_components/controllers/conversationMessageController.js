angular.module("frontendAngularClientApp")
    .factory("conversationMessageService", function ($http, Upload, $rootScope) {
        var querystring = "";
        return {
            listForDestCustomer: function (customerDestId) { return $http.get("/api/conversationmessage/getforcustomer/" + customerDestId + querystring); },
            sendMessage: function (entity) { return $http.post("/api/conversationmessage", entity); }
        };
    })
    .controller("conversationMessagesController", function ($scope, $state, $stateParams, $rootScope, conversationMessageService) {
        $scope.destCustomerId = $stateParams.destCustomerid;

        $scope.entitylist = null;
        $scope.entity = null;

        $scope.refresh = function () {
            $rootScope.IsBusy = true;
            conversationMessageService.listForDestCustomer($scope.destCustomerId).then(function (result) {
                $scope.entitylist = result.data;
                $rootScope.SetPageTitle("Conversations", "");
                $rootScope.IsBusy = false;
            }, $rootScope.raiseErrorDelegate);
        };

        $scope.send = function () {
            var entity = { message: $scope.entity.message, destId: $scope.destCustomerId };
            $rootScope.IsBusy = true;
            conversationMessageService.sendMessage(entity).then(function () {
                $rootScope.IsBusy = false;
                $scope.refresh();
            }, $rootScope.raiseErrorDelegate);
        };

        $scope.refresh();
    })
    .component('conversationBox', {
        //template: '<pre>{{$ctrl.entitylist|json}}</pre>',
        templateUrl: '/bobos_components/views/conversationComponentView.html',
        bindings: { customer: "<" },
        controller: function ($rootScope, $state, $element, $attrs, $window, $interval, conversationMessageService) {
            var $ctrl = this;

            //$ctrl.$onInit = function () { };
            $ctrl.$onChanges = function (changesObj) {
                $ctrl.refresh();
            };

            //$ctrl.customer FROM BINDINGS
            $ctrl.entitylist = null;
            $ctrl.entity = null;


            $ctrl.refresh = function () {
                if ($ctrl.customer == null)
                    return;

                //alert("refresh");
                //    //$rootScope.IsBusy = true;
                conversationMessageService.listForDestCustomer($ctrl.customer.id).then(function (result) {
                    $ctrl.entitylist = result.data;
                    //$rootScope.IsBusy = false;
                }, $rootScope.raiseErrorDelegate);
            };

            $ctrl.send = function () {
                var entity = { message: $ctrl.entity.message, destId: $ctrl.customer.id };
                //    //$rootScope.IsBusy = true;
                conversationMessageService.sendMessage(entity).then(function () {
                    //        //$rootScope.IsBusy = false;
                    $ctrl.refresh();
                }, $rootScope.raiseErrorDelegate);
            };

            $ctrl.refresh();

        }
    })
    ;