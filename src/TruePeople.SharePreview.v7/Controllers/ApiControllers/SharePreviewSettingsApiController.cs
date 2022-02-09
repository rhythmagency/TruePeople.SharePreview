using System.Web.Http;
using TruePeople.SharePreview.Factories;
using TruePeople.SharePreview.Models;
using TruePeople.SharePreview.Services;
using Umbraco.Web.WebApi;

namespace TruePeople.SharePreview.Controllers.ApiControllers
{
    public class ShareablePreviewSettingsApiController : UmbracoAuthorizedApiController
    {
        private readonly SharePreviewSettingsService _shareablePreviewSettingsService;

        public ShareablePreviewSettingsApiController()
        {
            _shareablePreviewSettingsService = SharePreviewSettingsServiceFactory.GetSettingsService();
        }

        [HttpGet]
        public ShareSettings GetSettings()
        {
            return _shareablePreviewSettingsService.GetSettings();
        }

        [HttpPost]
        public bool SaveSettings(ShareSettings settings)
        {
            return _shareablePreviewSettingsService.UpdateSettings(settings);
        }
    }
}
