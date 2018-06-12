angular.module("frontendAngularClientApp")
    .controller("SignInController", function ($scope, $state, $rootScope, $stateParams, customerService) {
        $scope.login = "";
        $scope.password = "";

        $scope.refresh = function () {
            $rootScope.SetPageTitle("", "");
            if ($stateParams.suppl == "activationsucceeded")
                new PNotify({ title: "Account activation succeeded", text: "You can now signin", type: "success", styling: "bootstrap3", delay: 2000 });
        };

        /**
        * Override this method from an extend of this controller to control the way the app should react after a successful login
        **/
        $scope.redirectBackDelegate = function (response) {
            //to be overriden
            $state.go("dashboard");
        };

        /**
         * Signin
         * @param redirectBackDelegate : method to execute at successfull login
        **/
        $scope.signIn = function () {
            $rootScope.IsBusy = true;
            customerService.logIn($scope.login, $scope.password).then(function (response) {
                if (response.data.error != null) {
                    $rootScope.IsBusy = false;
                    $rootScope.raiseErrorDelegate(response);
                    return;
                }

                $rootScope.setConnectedUser(response.data);
                $rootScope.avatarUri = "/api/customer/avatar";

                $scope.redirectBackDelegate(response);

                $rootScope.IsBusy = false;
            }, function errorCallback(e) {
                new PNotify({ title: "Login error", text: e.data.error, type: "error", styling: "bootstrap3", delay: 2000 });
                $rootScope.IsBusy = false;
            });
        };

        $scope.refresh();
    });