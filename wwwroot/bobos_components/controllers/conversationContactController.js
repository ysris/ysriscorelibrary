angular.module("frontendAngularClientApp")
    .factory("conversationContactService", function ($http, Upload, $rootScope) {
        var querystring = "";
        return { list: function (customerDestId) { return $http.get("/api/conversationcontact/" + querystring); }, };
    })
    .controller("conversationContactsController", function ($scope, $state, $stateParams, $rootScope, conversationContactService, $http) {
        $scope.entitylist = null;
        $scope.selectedContact = null;

        /** Pusher event received */
        var eventHook = $rootScope.$on('onPusherMessage', function (event, data) { $scope.refresh(); });
        $scope.$on('$destroy', function () { eventHook(); });

        $scope.refresh = function () {
            $rootScope.IsBusy = true;
            conversationContactService.list().then(function (result) {
                $scope.entitylist = result.data;

                if ($scope.entitylist.length == 1)
                    $scope.selectedContact = $scope.entitylist[0];

                /* Full notifications count */
                $rootScope.refreshFullNotificationCount = function () {
                    $http.get("/api/realtime/fullnotificationcount").then(function (resp) {
                        $rootScope.fullNotificationsCount = resp.data.fullNotificationsCount;
                    });
                };
                $rootScope.refreshFullNotificationCount();
                $rootScope.IsBusy = false;
            }, $rootScope.raiseErrorDelegate);
        };

        $scope.setSelected = function (item) {
            $scope.selectedContact = item;
            $scope.refresh();
        };

        $scope.refresh();
    })
    ;