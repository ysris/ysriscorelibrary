'use strict';

/**
 * @ngdoc directive
 * @name frontendAngularClientApp.directive:masterHeader
 * @description
 * # masterHeader
 */
angular.module('frontendAngularClientApp')
    .directive('busyIndicator', function () {
        return {
            templateUrl: '/bobos_components/views/templates/busy-indicator.html',
            restrict: 'E'
        };
    });
