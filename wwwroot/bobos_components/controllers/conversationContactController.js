angular.module("frontendAngularClientApp")
    .factory("conversationContactService", function ($http, Upload, $rootScope) {
        return {
            list: function (customerDestId) { return $http.get("/api/conversationcontact/"); },
            markasread: function (id) {

                if (id == null || id == undefined)
                    return $http.get("/api/conversationmessage/markasread");
                return $http.get("/api/conversationmessage/markasread/" + id);
            }
        };
    })
    .controller("conversationContactsController", function ($scope, $state, $stateParams, $rootScope, conversationContactService, $http) {
        $scope.entitylist = null;
        $scope.selectedContact = null;

        /** Pusher event received */
        var eventHook = $rootScope.$on('onPusherMessage', function (event, data) { $scope.refresh(); });
        $scope.$on('$destroy', function () { eventHook(); });

        $scope.refresh = function () {

            $rootScope.IsBusy = true;

            $rootScope.refreshFullNotificationCount();
            //TODO : Remove double imbricated ajax call

            conversationContactService.list().then(function (result) {
                $scope.entitylist = result.data;
                if ($scope.entitylist.length == 1)
                    $scope.selectedContact = $scope.entitylist[0];

                if ($scope.entitylist.length == 1) {
                    conversationContactService.markasread().then(function (result) {
                        $rootScope.refreshFullNotificationCount();
                        $rootScope.IsBusy = false;
                    }, $rootScope.raiseErrorDelegate);
                }

                $rootScope.IsBusy = false;
            }, $rootScope.raiseErrorDelegate);
        };

        $scope.setSelected = function (item) {

            $rootScope.IsBusy = true;
            conversationContactService.markasread(item.id).then(function (result) {
                $scope.selectedContact = item;
                $scope.refresh();
                $rootScope.refreshFullNotificationCount();
                $rootScope.IsBusy = false;
            });


        };




        $scope.refresh();

    });