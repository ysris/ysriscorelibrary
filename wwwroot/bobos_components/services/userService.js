angular.module("frontendAngularClientApp").factory("userService", function ($http, Upload) {
    return {
        createAccount: function (entity) { return $http.post("/api/customer", entity); }
        , logIn: function (username, password) { return $http.post("/api/customer/Login", { username: username, password: password }); }
        , logOut: function () { return $http.post("/api/customer/Logout"); }
        , recover: function (email) { return $http.post("/api/customer/recover", { email: email }); }
        , recover2: function (email, activationcode, password) { return $http.post("/api/customer/recover2", { email: email, activationCode: activationcode, password: password }, { 'Content-Type': 'application/json' }); }
        , get: function (id) {
            if (id == null)
                return $http.get("/api/customer/me");
            return $http.get("/api/customer/" + id);
        }
        , update: function (entity) { return $http.post("/api/customer/update", entity); }
        , uploadAvatar: function (file) { return Upload.upload({ url: "/api/customer/avatar/", data: { 'file': file } }) }
        , List: function () { return $http.get("/api/customer"); }
        , GetProjection: function () { return $http.get("/api/customer/projection"); }
        , delete: function () { return $http.delete("/api/customer"); }

        , inviteCustomer: function (entity) { return $http.post("/api/customer/invite", entity); }
        
    };
});