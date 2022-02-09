(function () {
    "use strict";

    function sharePreviewResource($http, umbRequestHelper) {

        var sharePreviewBaseUrl = "/umbraco/backoffice/api/SharePreviewApi/";

        var resource = {
            getShareableLink: getShareableLink,
            hasShareableLink: hasShareableLink
        };

        return resource;

        function getShareableLink(id) {
            return umbRequestHelper.resourcePromise(
                $http.get(sharePreviewBaseUrl + "GetShareableLink", {
                    params: {
                        nodeId: id
                    }
                }));
        };

        function hasShareableLink(id) {
            return umbRequestHelper.resourcePromise(
                $http.get(sharePreviewBaseUrl + "HasShareableLink", {
                    params: {
                        nodeId: id
                    }
                }));
        }
    }

    angular.module("umbraco.resources").factory("sharePreviewResource", sharePreviewResource);

})();