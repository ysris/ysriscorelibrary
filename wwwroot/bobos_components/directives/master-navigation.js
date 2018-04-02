/**
 * @ngdoc directive
 * @name frontendAngularClientApp.directive:masterHeader
 * @description
 * # masterHeader
 */
angular.module('frontendAngularClientApp')
    .directive('masterNavigation', function () {
        return {
            templateUrl: '/views/master-navigation.html'            
        };
    });
