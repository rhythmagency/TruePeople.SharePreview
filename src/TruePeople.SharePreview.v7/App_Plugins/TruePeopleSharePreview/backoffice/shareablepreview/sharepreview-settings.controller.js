angular.module("umbraco").controller("TP.SharePreview.Settings.Controller",
    function ($scope, $http, notificationsService) {
        $scope.submitButtonState = "init";
        $scope.settings = {};
        $scope.pageName = "Shareable Preview Settings";

        $http.get("/umbraco/backoffice/api/ShareablePreviewSettingsApi/GetSettings").then(function (res) {
            $scope.settings.privateKey = res.data.PrivateKey;
            $scope.settings.notValidUrl = res.data.NotValidUrl;
        });

        $scope.saveSettings = function () {
            $scope.submitButtonState = "busy";
            var data = {
                privateKey: $scope.settings.privateKey,
                notValidUrl: $scope.settings.notValidUrl
            };
            $http.post("/umbraco/backoffice/api/ShareablePreviewSettingsApi/SaveSettings", JSON.stringify(data)).then(function (res) {
                let result = JSON.parse(res.data);
                if (result === true) {
                    notificationsService.success("Shareable preview", "Successfully saved the settings.");
                    $scope.submitButtonState = "success";
                } else {
                    notificationsService.error("Shareable preview", "Something went wrong while trying to save the settings.");
                    $scope.submitButtonState = "error";
                }
            });
        }
    });