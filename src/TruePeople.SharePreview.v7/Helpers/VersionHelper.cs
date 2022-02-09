using Umbraco.Core.Models;

namespace TruePeople.SharePreview.Helpers
{
    internal static class VersionHelper
    {
        /// <summary>
        /// This is a rough replacement for the "Edited" property found in Umbraco 8+.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="latestVersion"></param>
        /// <returns></returns>
        public static bool WasNodeEdited(IContent content, IContent latestVersion)
        {
            // Since Umbraco 7's IContent doesn't contain an Edited property, we will infer whether it has been
            // edited by checking if either:
            // - there is no published version guid, or
            // - the latest version guid is different from the published version guid
            return latestVersion != null && (content.PublishedVersionGuid == default || content.PublishedVersionGuid != latestVersion.Version);
        }
    }
}
