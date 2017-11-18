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
            templateUrl: '/bobos_components/views/templates/master-footer.html',
            restrict: 'E'
        };
    });
