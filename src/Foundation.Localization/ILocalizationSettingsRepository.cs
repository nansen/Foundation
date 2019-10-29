using Foundation.Localization.Models;

namespace Foundation.Localization
{
    public interface ILocalizationSettingsRepository
    {
        ILocalizationSettings GetDefaultSiteSettings();
        ILocalizationSettings GetSettingsById(string id);
    }
}