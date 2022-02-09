using TruePeople.SharePreview.Services;

namespace TruePeople.SharePreview.Factories
{
    /// <summary>
    /// Umbraco 7 does not have any built-in DI (unlike v8 and above) and we don't want to assume/plan for
    /// any particular DI configuration for the projects consuming this plugin. We supply services
    /// in this plugin are from this factory so that there are no external DI considerations.
    /// </summary>
    public static class SharePreviewSettingsServiceFactory
    {
        private static SharePreviewSettingsService _settingsService;

        public static SharePreviewSettingsService GetSettingsService()
        {
            if (_settingsService == null)
            {
                _settingsService = new SharePreviewSettingsService();
            }
            return _settingsService;
        }
    }
}
