﻿(function () {
    'use strict';

    function singleShareLinkDirective() {

        var directive = {
            scope: {
                buttonlabel: "@",
                shareurl: "@",
                enabled: "@"
            },
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/TPSharePreview/components/single-share-link.html?umb_rnd=' + Umbraco.Sys.ServerVariables.application.cacheBuster
        };

        return directive;

    }

    angular.module('umbraco.directives').directive('singleShareLink', singleShareLinkDirective);

})();