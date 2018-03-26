angular.module("frontendAngularClientApp")
    .factory("conversationMessageService", function ($http, Upload, $rootScope) {
        var querystring = "";
        return {
            listForDestCustomer: function (customerDestId) { return $http.get("/api/conversationmessage/getforcustomer/" + customerDestId + querystring); },
            sendMessage: function (entity) { return $http.post("/api/conversationmessage", entity); }
        };
    })
    .directive('enterSubmit', function () {
        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {

                elem.bind('keydown', function (event) {
                    var code = event.keyCode;

                    if (code === 13) {
                        if (event.ctrlKey) {
                            event.preventDefault();
                            scope.$apply(attrs.enterSubmit);
                        }
                    }
                });
            }
        }
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
        controller: function ($rootScope, $scope, $state, $element, $attrs, $window, $interval, conversationMessageService) {
            var $ctrl = this;

            //$ctrl.customer FROM BINDINGS
            $ctrl.entitylist = null;
            $ctrl.entity = null;



            //$ctrl.$onInit = function () { };
            $ctrl.$onChanges = function (changesObj) {
                $ctrl.refresh();
            };


            /**
             * Pusher event received
             */
            var eventHook = $rootScope.$on('onPusherMessage', function (event, data) {
                switch (data.type) {
                    case "ConversationMessageSent":
                        if (data.destId == $rootScope.ConnectedUserId && data.authorId == $ctrl.customer.id) {
                            //alert("We should update conversation with id=" + data.authorId);
                            $ctrl.refresh();

                        }
                        break;

                    default:
                }
            });
            $scope.$on('$destroy', function () { eventHook(); });


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
                    $ctrl.entity.message = null;
                    //        //$rootScope.IsBusy = false;
                    $ctrl.refresh();
                }, $rootScope.raiseErrorDelegate);
            };

            $ctrl.refresh();

        }
    })
    ;