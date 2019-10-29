using EPiServer.Core;

namespace Foundation.Localization.Models
{
    public interface ILocalizationSettings
    {
        ContentReference TranslationsRoot { get; }
    }
}