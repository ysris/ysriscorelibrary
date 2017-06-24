'use strict';

/**
 * @ngdoc directive
 * @name frontendAngularClientApp.directive:masterHeader
 * @description
 * # masterHeader
 */
angular.module('frontendAngularClientApp')
    .directive('masterFooter', function () {
        return {
            templateUrl: '/views/templates/master-footer.html',
            restrict: 'E'
        };
    });
