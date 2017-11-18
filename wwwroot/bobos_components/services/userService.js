﻿'use strict';

angular.module("frontendAngularClientApp").factory("userService", function ($http, Upload) {
    return {
        createAccount: function (entity) { return $http.post("/api/Customer", entity); }
        , logIn: function (username, password) { return $http.post("/api/Customer/Login", { username: username, password: password }); }
        , logOut: function () { return $http.post("/api/Customer/Logout"); }
        , recover: function (email) { return $http.post("/api/Customer/recover", { email: email }); }
        , recover2: function (email, activationcode, password) { return $http.post("/api/Customer/recover2", { email: email, activationCode: activationcode, password: password }, { 'Content-Type': 'application/json' }); }
        , get: function (id) {
            if (id == null)
                return $http.get("/api/Customer/me");
            return $http.get("/api/Customer/" + id);
        }
        , update: function (entity) { return $http.post("/api/Customer/update", entity); }
        , uploadAvatar: function (file) { return Upload.upload({ url: "/api/customer/avatar/", data: { 'file': file } }) }
        , List: function () { return $http.get("/api/customer"); }
        , GetProjection: function () { return $http.get("/api/customer/projection"); }
        , delete: function () { return $http.delete("/api/customer"); }
    };
});