using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using Foundation.Localization.Models;
using System;

namespace Foundation.Localization
{
    /// <summary>
    /// An implementation of <see cref="ILocalizationSettingsRepository"/> which assumes that localization settings are defined on the start page for a given site.
    /// </summary>
    public class StartPageLocalizationSettingsRepository : ILocalizationSettingsRepository
    {
        private readonly IContentLoader _contentLoader;
        private readonly ISiteDefinitionRepository _siteDefinitionRepository;
        public StartPageLocalizationSettingsRepository(IContentLoader contentLoader, ISiteDefinitionRepository siteDefinitionRepository)
        {
            _contentLoader = contentLoader;
            _siteDefinitionRepository = siteDefinitionRepository;
        }
        public ILocalizationSettings GetDefaultSiteSettings()
        {
            return GetSettingsById(SiteDefinition.Current.Id.ToString());
        }

        public ILocalizationSettings GetSettingsById(string id)
        {
            ILocalizationSettings settings;
            var siteDefinitionForId = _siteDefinitionRepository.Get(new Guid(id));
            if (siteDefinitionForId != null)
            {
                settings = _contentLoader.Get<IContent>(siteDefinitionForId.StartPage) as ILocalizationSettings;
            }
            else
            {
                settings = null;
            }

            return settings;
        }
    }
}