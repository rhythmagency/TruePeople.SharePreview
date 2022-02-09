angular.module("umbraco").run(function ($rootScope, $routeParams, eventsService, localizationService, sharePreviewResource, $compile) {
    var buttonLabel = "Share preview URL";
    localizationService.localize("shareablepreview_buttonlabel").then(function (value) {
        buttonLabel = value;
    });

    $rootScope.$on('$routeChangeSuccess', function (event, next) {
        if ($routeParams.id === undefined || $routeParams.id === -1 || $routeParams.section !== "content") {
            return;
        }
        setSharePreviewButton($routeParams.id);
    });

    eventsService.on("content.loaded", function (name, args) {
        if (args.content.id === parseInt($routeParams.id)) {
            contentReload(args.content.id);
        }
    });

    eventsService.on("content.saved", function (name, args) {
        if (args.content.id === parseInt($routeParams.id)) {
            contentReload(args.content.id);
        }
    });

    function contentReload(id) {
        if (id === undefined) {
            return;
        }
        initializeButtonLoader(id);
    }

    function setSharePreviewButton(nodeId) {
        sharePreviewResource.hasShareableLink(nodeId).then(function (res) {
            sharePreviewResource.getShareableLink(nodeId).then(function (data) {
                if (data == null || data == undefined) {
                    return;
                }

                data = JSON.parse(data);

                var buttonElement = angular.element("<single-share-link enabled='" + res +"' shareUrl='" + data + "' buttonLabel='" + buttonLabel + "' />");
                var linkFN = $compile(buttonElement);
                var el = linkFN($rootScope);

                if (document.querySelector(".umb-editor-drawer-content__right-side") === null) {
                    return;
                }

                if (document.querySelector("#shareLink") === null) {
                    document.querySelector(".umb-editor-drawer-content__right-side").prepend(el[0]);
                } else {
                    document.querySelector("#shareLink").replaceWith(el[0]);
                }
            });
        });
    }

    function initializeButtonLoader(contentId) {
        setTimeout(function () {
            setSharePreviewButton(contentId);
        }, 10);
    }
});
