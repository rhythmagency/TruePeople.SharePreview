using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Http;
using TruePeople.SharePreview.Factories;
using TruePeople.SharePreview.Helpers;
using TruePeople.SharePreview.Models;
using TruePeople.SharePreview.Services;
using Umbraco.Core.Services;
using Umbraco.Web.WebApi;

namespace TruePeople.SharePreview.Controllers.ApiControllers
{
    public class SharePreviewApiController : UmbracoAuthorizedApiController
    {
        private readonly IContentService _contentService;
        private readonly SharePreviewSettingsService _sharePreviewSettingsService;

        public SharePreviewApiController()
        {
            _contentService = ContentServiceFactory.GetContentService();
            _sharePreviewSettingsService = SharePreviewSettingsServiceFactory.GetSettingsService();
        }

        [HttpGet]
        public bool HasShareableLink(int nodeId)
        {
            if(nodeId == -1)
            {
                return false;
            }
            var content = _contentService.GetById(nodeId);
            var latestContentVersion = _contentService.GetVersions(nodeId).FirstOrDefault();
            var wasEdited = VersionHelper.WasNodeEdited(content, latestContentVersion);
            return wasEdited && !string.IsNullOrEmpty(content.Template.Alias) && !content.Trashed;
        }

        [HttpGet]
        public string GetShareableLink(int nodeId)
        {
            try
            {
                return GenerateShareLink(nodeId);
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(SharePreviewApiController), "Error occured whilst trying to create a shareable link", ex);
                return null;
            }
        }

        private string GenerateShareLink(int nodeId)
        {
            var privateEncryptionKey = _sharePreviewSettingsService.GetSettings().PrivateKey;

            if(privateEncryptionKey == null)
            {
                return null;
            }

            var latestNodeVersion = _contentService.GetVersions(nodeId).FirstOrDefault();

            var objToEncrypt = new SharePreviewContext()
            {
                NodeId = nodeId,
                NewestVersionId = latestNodeVersion.Version
            };

            var encrypted = TPEncryptHelper.EncryptString(JsonConvert.SerializeObject(objToEncrypt), privateEncryptionKey);

            return string.Format("/umbraco/sharepreview/index/{0}", encrypted);
        }
    }
}
