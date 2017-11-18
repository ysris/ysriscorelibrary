/**
 * @ngdoc directive
 * @name frontendAngularClientApp.directive:masterHeader
 * @description
 * # masterHeader
 */
angular.module('frontendAngularClientApp')
    .directive('masterNavigation', function () {
        return {
            templateUrl: '/bobos_components/views/templates/master-navigation.html'            
        };
    });
