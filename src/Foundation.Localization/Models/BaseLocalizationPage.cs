using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Foundation.Localization.Models
{
    public abstract class BaseLocalizationPage : PageData
    {
        protected const string REGEX_REPLACE = @"[^A-Za-z0-9]+";
        protected const string CATEGORIES_NODE_NAME = "categories";

        private IContentRepository _contentRepo;
        protected virtual IContentRepository ContentRepo
        {
            get
            {
                if (_contentRepo == null)
                    _contentRepo = ServiceLocator.Current.GetInstance<IContentRepository>();
                return _contentRepo;
            }
        }

        public virtual string NodeName
        {
            get
            {
                var key = this.OriginalText ?? this.Name;
                return Regex.Replace(key.ToLowerInvariant(), REGEX_REPLACE, string.Empty);
            }
        }

        public abstract string OriginalText { get; set; }

        /// <summary>
        ///     Gets the lookup key this item.
        /// </summary>
        /// <remarks>
        /// Displays the path to use in e.g. the translate control.
        ///     <![CDATA[
        ///         <EPiServer:Translate runat="server" Text="/jeroenstemerdink/textone" />
        ///     ]]> 
        /// </remarks>
        public string LookupKey
        {
            get
            {
                List<string> keyParts = new List<string>();

                PageData masterLanguagePage;
                IEnumerable<IContent> ancestors;
                if (this.ContentLink.ID != 0) //page has not been published yet
                {
                    // Use the masterlanguage branch, that one is always available.
                    masterLanguagePage = ContentRepo.Get<PageData>(PageLink, new LoaderOptions {LanguageLoaderOption.Specific(MasterLanguage)});
                    ancestors = ContentRepo.GetAncestors(masterLanguagePage.PageLink).Reverse();
                }
                else
                {
                    // Use the masterlanguage branch, that one is always available.
                    masterLanguagePage = ContentRepo.Get<PageData>(ParentLink, new LoaderOptions {LanguageLoaderOption.Specific(MasterLanguage)});
                    ancestors = ContentRepo.GetAncestors(masterLanguagePage.PageLink)
                        .Reverse()
                        .Concat(new[] {masterLanguagePage});
                }

                // Get all translation containers, skip the main one.
                keyParts.AddRange(
                    ancestors.OfType<TranslationContainer>()
                        .Select(ancestor => ancestor.NodeName)
                        .Skip(1));

                // Add this file
                keyParts.Add(this.NodeName);

                return string.Format(CultureInfo.InvariantCulture, "/{0}", string.Join("/", keyParts));
            }
        }

        public abstract void SetPageData(System.Xml.XmlNode node);
    }
}
