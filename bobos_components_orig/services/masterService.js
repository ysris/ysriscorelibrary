'use strict';

angular.module("frontendAngularClientApp").factory("masterService", function ($http, Upload, $rootScope) {
    return {
        updateLocalSession: function () {
            // User id
            $rootScope.ConnectedUserId = null;
            if (window.localStorage.id != null)
                $rootScope.ConnectedUserId = window.localStorage.id;
            $rootScope.customerType = null;
            if (window.localStorage.customerType != null)
                $rootScope.customerType = window.localStorage.customerType;
            $rootScope.customer = null;
            if (window.localStorage.customer != null) {
                $rootScope.customer = JSON.parse(window.localStorage.customer);
                $rootScope.avatarUri = "/api/customer/avatar";
            }

            // Notifications
            $rootScope.ConnectedUserNotifications = [];
            if (window.localStorage.ConnectedUserNotifications != null)
                $rootScope.ConnectedUserNotifications = JSON.parse(window.localStorage.ConnectedUserNotifications);
        }
    };
});