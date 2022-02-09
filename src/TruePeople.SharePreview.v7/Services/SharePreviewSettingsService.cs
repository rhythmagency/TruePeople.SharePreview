using log4net;
using System;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;
using TruePeople.SharePreview.Models;


namespace TruePeople.SharePreview.Services
{
    public class SharePreviewSettingsService
    {
        private readonly string _settingsCacheKey = "ShareablePreviewSettings";
        private readonly string _configPath = HttpRuntime.AppDomainAppPath + "\\config\\sharePreviewSettings.config";
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ShareSettings GetSettings()
        {
            if (!(HttpRuntime.Cache.Get(_settingsCacheKey) is ShareSettings settings))
            {
                settings = ReadSettings();
                HttpRuntime.Cache.Insert(_settingsCacheKey, settings, null, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration);
            }
            return settings;
        }


        public bool UpdateSettings(ShareSettings newSettings)
        {
            if (SetSettings(newSettings))
            {
                HttpRuntime.Cache.Insert(_settingsCacheKey, newSettings, null, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration);
                return true;
            }
            else
            {
                return false;
            }
        }

        private ShareSettings ReadSettings()
        {
            try
            {
                if (System.IO.File.Exists(_configPath))
                {
                    using (var stream = System.IO.File.OpenRead(_configPath))
                    {
                        var serializer = new XmlSerializer(typeof(ShareSettings));
                        return serializer.Deserialize(stream) as ShareSettings ?? new ShareSettings();
                    }
                }
                else
                {
                    //This probably means that this is the first time they start the application with this package installed.
                    //So we generate a random private key so it works out of the box.
                    var randomSettings = new ShareSettings
                    {
                        PrivateKey = Guid.NewGuid().ToString(),
                        NotValidUrl = "/"
                    };

                    UpdateSettings(randomSettings);

                    return randomSettings;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error occured whilst reading out the settings.", ex);
                return null;
            }
        }

        private bool SetSettings(ShareSettings newSettings)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(_configPath))
                {
                    var serializer = new XmlSerializer(typeof(ShareSettings));
                    serializer.Serialize(writer, newSettings);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Error occured whilst saving the settings.", ex);
                return false;
            }
        }
    }
}
