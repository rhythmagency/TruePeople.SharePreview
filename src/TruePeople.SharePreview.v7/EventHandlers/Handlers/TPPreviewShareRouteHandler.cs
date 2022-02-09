using System;
using System.Linq;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;
using TruePeople.SharePreview.Helpers;
using System.Web;
using Newtonsoft.Json;
using TruePeople.SharePreview.Models;
using System.Security.Cryptography;
using Umbraco.Core.Models;
using log4net;
using System.Reflection;
using System.Globalization;
using System.Threading;
using TruePeople.SharePreview.Factories;

namespace TruePeople.SharePreview.EventHandlers.Handlers
{
    internal class TPPreviewShareRouteHandler : UmbracoVirtualNodeRouteHandler
    {
        public const string AcceptPreviewMode = "UMB-WEBSITE-PREVIEW-ACCEPT";
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly CultureInfo DefaultCulture = new CultureInfo("en-US");

        protected override IPublishedContent FindContent(RequestContext requestContext, UmbracoContext umbracoContext)
        {
            var contentService = ContentServiceFactory.GetContentService();
            var sharePreviewSettingsService = SharePreviewSettingsServiceFactory.GetSettingsService();
            var settings = sharePreviewSettingsService.GetSettings();

            //Decode first layer of the base64 string;
            //Then decode second layer and decrypt it with private key.
            try
            {
                var decrypted = TPEncryptHelper.DecryptString(requestContext.RouteData.Values["pageId"].ToString(), settings.PrivateKey);
                var sharePreviewContext = JsonConvert.DeserializeObject<SharePreviewContext>(decrypted);

                if (sharePreviewContext.NodeId == default || sharePreviewContext.NewestVersionId == default)
                {
                    RedirectToInvalidUrl(settings.NotValidUrl);
                }

                var content = contentService.GetById(sharePreviewContext.NodeId);
                var latestNodeVersion = contentService.GetVersions(sharePreviewContext.NodeId).FirstOrDefault();
                var wasEdited = VersionHelper.WasNodeEdited(content, latestNodeVersion);

                if (latestNodeVersion != null && latestNodeVersion.Version == sharePreviewContext.NewestVersionId && wasEdited)
                {
                    var page = umbracoContext.ContentCache.GetById(true, sharePreviewContext.NodeId);

                    //Since we don't use the base.PreparePublishedRequest, the culture isn't being set correctly.
                    //Set it like umbraco does for previews.
                    Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = DefaultCulture;

                    return page;
                }

                RedirectToInvalidUrl(settings.NotValidUrl);
            }
            catch (CryptographicException)
            {
                //Probably means someone changed their key and still have a url that was encrypted with the old key.
                RedirectToInvalidUrl(settings.NotValidUrl);
            }
            catch (Exception ex)
            {
                Logger.Error("Error occured when rendering shareable preview content.", ex);
                RedirectToInvalidUrl(settings.NotValidUrl);
            }

            //When it gets here something went wrong...
            return null;
        }

        protected override void PreparePublishedContentRequest(PublishedContentRequest request)
        {
            var def = new RouteDefinition
            {
                ActionName = request.RoutingContext.UmbracoContext.HttpContext.Request.RequestContext.RouteData.GetRequiredString("action"),
                ControllerName = request.RoutingContext.UmbracoContext.HttpContext.Request.RequestContext.RouteData.GetRequiredString("controller"),
                PublishedContentRequest = request
            };

            // Set the special data token to the current route definition
            request.RoutingContext.UmbracoContext.HttpContext.Request.RequestContext.RouteData.DataTokens[Constants.Web.UmbracoRouteDefinitionDataToken] = def;
            request.RoutingContext.UmbracoContext.HttpContext.Request.RequestContext.RouteData.Values["action"] = request.PublishedContent.GetTemplateAlias();

            // We set it here again, because it gets overwritten in the pipeline.
            request.Culture = Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = DefaultCulture;

            // Since Umbraco 8.10 they show a "Exit Preview mode" message if you visit the site in preview mode. We don't want this in this package.
            // They check if the message should be shown by using a cookie. If this cookie doesn't exists, we set the cookie with the same expiration.
            if (request.RoutingContext.UmbracoContext.HttpContext != null)
            {
                request.RoutingContext.UmbracoContext.HttpContext.Response.Cookies.Set(new HttpCookie(AcceptPreviewMode, "true") { Expires = DateTime.Now.AddMinutes(5)});
            }
        }

        private void RedirectToInvalidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                HttpContext.Current.Response.Redirect("/");
            }
            else
            {
                //Redirect to configured page.
                HttpContext.Current.Response.Redirect(url);
            }
        }
    }
}
