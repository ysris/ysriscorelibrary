angular.module("frontendAngularClientApp")
    .factory("customerService", function ($http, Upload) {
        return {
            activate: function (entity) { return $http.post("/api/customer/activateasadmin", entity); }
            , disable: function (entity) { return $http.post("/api/customer/disableasadmin", entity); }
            , GetEmptyEntity: function () { return $http.get("/api/customer/empty"); }
        };
    })
    .factory("userCustomerService", function ($http, Upload) {
        return {
            saveOnboarding: function (entity) { return $http.post("/api/usercustomer", entity); }
            , getMe: function () { return $http.get("/api/usercustomer/me"); }
            , uploadFile: function(file) { return Upload.upload({ url: "/api/usercustomer/file/", data: { 'file': file } }); }
        };
    })
    .factory("businessCustomerService", function ($http, Upload) {
        return {
            saveOnboarding: function (entity) { return $http.post("/api/businesscustomer", entity); }
            , getMe: function () { return $http.get("/api/businesscustomer/me"); }
            // , uploadFileSet: function (fileSet) { return Upload.upload({ url: "/api/businesscustomer/files/", data: { 'fileSet': fileSet } }) }
            //, logIn: function (username, password) { return $http.post("/api/AccountLogin/Login", { username: username, password: password }); }
            //, logOut: function () { return $http.post("/api/AccountLogin/Logout"); }
            //, recover: function (email) { return $http.post("/api/AccountLogin/recover", { email: email }); }
            //, recover2: function (email, activationcode, password) { return $http.post("/api/AccountLogin/recover2", { email: email, activationCode: activationcode, password: password }, { 'Content-Type': 'application/json' }); }
            //, UploadAvatar: function (file) {
            //    return Upload.upload({
            //        url: URL.root + URL.api + "customer/avatar/",
            //        data: { 'file': file }
            //    });
            //}
            //,
            //List: function () { return $http.get("/api/customer"); }
        };
    })
    ;