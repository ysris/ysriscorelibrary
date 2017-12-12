angular.module("frontendAngularClientApp").factory("clientService", function ($http, Upload) {
    return {
        List: function () { return $http.get("/api/client"); }
        , GetEmptyEntity: function () { return $http.get("/api/client/empty"); }
        , Post: function (entity) {
            if (entity.id == null || entity.id == 0)
                return $http.post("/api/client", entity);
            else
                return $http.put("/api/client/" + entity.id, entity);
        }
        , Get: function (id) { return $http.get("/api/client/" + id); }
    };
});