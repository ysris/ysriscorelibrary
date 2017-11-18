'use strict';

/**
 * @ngdoc directive
 * @name frontendAngularClientApp.directive:masterHeader
 * @description
 * # masterHeader
 */
angular.module('frontendAngularClientApp')
    .directive('masterHeader', function () {
        return {
            templateUrl: '/views/master-header.html',
            restrict: 'E'
        };
    });
