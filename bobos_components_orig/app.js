"use strict";

angular
    .module("frontendAngularClientApp", [
        "ui.router"
        //"ngSanitize",
        //, "config"
        //,"ngStorage"
        , "ngFileUpload"
        //"angular-chartist",
        //"pascalprecht.translate",
        //"SignalR",
        //, "checklist-model"
        , "ngMessages"
        , "oitozero.ngSweetAlert"
        , "angularProgressbar"
        , "cn.offCanvas"
    ])
    .factory('myOffCanvas', function (cnOffCanvas) {
        return cnOffCanvas({
            controller: 'MyOffCanvasCtrl',
            controllerAs: 'offCanvas',
            templateUrl: 'Views/templates/my-off-canvas.html'
        });
    })
    // typically you'll inject the offCanvas service into its own
    // controller so that the nav can toggle itself
    .controller('MyOffCanvasCtrl', function (myOffCanvas, $rootScope, $scope) {
        this.toggle = myOffCanvas.toggle;
        this.hello=function() {alert("Hello!");};      

        
    // Menu utilities //

    this.showPageFor = function (condition) {


        switch (condition) {
            case "NotConnectedUser":
                return $scope.ConnectedUserId == null || $scope.ConnectedUserId == 'null';
            default:

                if ($rootScope.customer == null)
                    return false;

                //for condition Locataire => check  if there is Locataire in the roles... and so on
                var conditions = condition.split(",");
                for (var i in conditions) {
                    if ($rootScope.customer.roles.indexOf(conditions[i].trim()) !== -1)
                        return true;
                }
                return false;
        }
    };

    })
    .run(["$rootScope", "$location", "$state", "$http", "$window", "userService", function ($rootScope, $location, $state, $http, $window, userService) {
        $rootScope.updateLocalSession = function () {
            // User id
            $rootScope.ConnectedUserId = null;
            if (window.localStorage.id != null)
                $rootScope.ConnectedUserId = window.localStorage.id;
            $rootScope.customerType = null;
            if (window.localStorage.customerType != null)
                $rootScope.customerType = window.localStorage.customerType;
            $rootScope.customer = null;
            if (window.localStorage.customer != null)
                $rootScope.customer = JSON.parse(window.localStorage.customer);

            // Notifications
            $rootScope.ConnectedUserNotifications = [];
            if (window.localStorage.ConnectedUserNotifications != null)
                $rootScope.ConnectedUserNotifications = JSON.parse(window.localStorage.ConnectedUserNotifications);

            userService.get().then(
                function (resp) {

                }, function (e) {
                    //error : session lost => gotosignin
                    window.localStorage.clear();
                    $state.go("signin", { suppl: null });
                });

            $rootScope.avatarUri = null; $rootScope.avatarUri = "/api/customer/avatar?=" + + new Date();

        };
        $rootScope.updateLocalSession();

    }])
    .config(["$urlRouterProvider", "$stateProvider", "$httpProvider", "$locationProvider", function ($urlRouterProvider, $stateProvider, $httpProvider, $locationProvider) {
        $urlRouterProvider.otherwise("/home");
        $stateProvider
            .state("signin", { url: "/signin/:suppl", templateUrl: "Views/signin.html", controller: "SignInController" })
            .state("signin2", { url: "/signin", templateUrl: "Views/signin.html", controller: "SignInController" })
            .state("passwordrecover", { url: "/passwordrecover", templateUrl: "/views/passwordrecover.html", controller: "PasswordRecoverController" })
            .state("passwordrecover2", { url: "/passwordrecover2/:email/:activationcode", templateUrl: "/views/passwordrecover2.html", controller: "PasswordRecover2Controller" })
            .state("profile", { url: "/profile/:id", templateUrl: "views/profile.html", controller: "ProfileController" });
        $stateProvider
            //.state("home", { url: "/home", templateUrl: "Views/home.html", controller: "HomeController" })
            .state("inscription", { url: "/inscription", templateUrl: "Views/inscription.html", controller: "InscriptionController" })
            .state("inscriptioncoach", { url: "/inscriptioncoach", templateUrl: "Views/inscriptioncoach.html", controller: "InscriptionController" })
            .state("dashboard",
            {
                url: "/dashboard",
                controllerProvider: function () {
                    switch (window.localStorage.customerType) {
                        case "User": return "DashboardUserController";
                        case "Coach": return "DashboardCoachController";
                    }
                },
                templateUrl: function () {
                    switch (window.localStorage.customerType) {
                        case "User": return "Views/dashboarduser.html";
                        case "Coach": return "Views/dashboardcoach.html";
                    }
                }
            })
            .state("coachs", { url: "/coachs", templateUrl: "Views/coachs.html", controller: "CoachsController" })
            .state("programs", { url: "/programs", templateUrl: "Views/programs.html", controller: "ProgramsController" })
            .state("profileedit", { url: "/profileedit", templateUrl: "bobos_components/views/profileedit.html", controller: "ProfileEditController" })

            .state("sportprogramedit", { url: "/sportprogramedit/:id", templateUrl: "Views/sportprogramedit.html", controller: "SportProgramEditController" })
            .state("sportprogram", { url: "/sportprogram/:id", templateUrl: "Views/sportprogram.html", controller: "SportProgramController" })
            .state("sportprogramrun", { url: "/sportprogramrun/:id", templateUrl: "Views/sportprogramrun.html", controller: "SportProgramRunController" })

            ;
    }])
    ;