using Umbraco.Core;
using Umbraco.Core.Services;

namespace TruePeople.SharePreview.Factories
{
    /// <summary>
    /// There's probably a better way of supplying the ContentService (service locator anti-pattern, I know), 
    /// but it's centralised here for easy replacement in the future if desired.
    /// </summary>
    public static class ContentServiceFactory
    {
        private static IContentService _contentService;

        public static IContentService GetContentService()
        {
            if (_contentService == null)
            {
                _contentService = ApplicationContext.Current.Services.ContentService;
            }
            return _contentService;
        }
    }
}
