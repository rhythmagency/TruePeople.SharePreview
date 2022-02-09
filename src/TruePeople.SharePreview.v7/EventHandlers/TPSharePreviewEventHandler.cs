using System.Web.Mvc;
using System.Web.Routing;
using TruePeople.SharePreview.EventHandlers.Handlers;
using TruePeople.SharePreview.RequestFilters;
using Umbraco.Core;
using Umbraco.Web;

namespace TruePeople.SharePreview.EventHandlers
{
    public class TPSharePreviewEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // Add the preview badge filter
            GlobalFilters.Filters.Add(new RemoveSharePreviewBadge());

            // Map the plugin route
            RouteTable.Routes.MapUmbracoRoute(
                name: "TPSharePreview",
                url: "umbraco/sharepreview/{action}/{pageId}",
                defaults: new { controller = "Default" },
                new TPPreviewShareRouteHandler()
            );

            // Boilerplate
            base.ApplicationStarted(umbracoApplication, applicationContext);
        }
    }
}
