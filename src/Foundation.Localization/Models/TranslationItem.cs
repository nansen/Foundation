using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Foundation.Localization.Models
{
    /// <summary>
    ///     The translation PageType.
    /// </summary>
    [ContentType(GUID = "A691F851-6C6E-4C06-B62E-8FBC5A038A68", AvailableInEditMode = true,
        Description = "Translation", DisplayName = "Translation", GroupName = "Localization")]
    [AvailableContentTypes(Exclude = new[] { typeof(PageData) })]
    public class TranslationItem : BaseLocalizationPage
    {
        private const string NAME_ATTRIBUTE = "name";
        private const string DESC_ATTRIBUTE = "description";

        #region Public Properties

        [Display(GroupName = SystemTabNames.Content, Description = "The text to translate.", Name = "Original text", Order = 10)]
        [Required(AllowEmptyStrings = false)]
        public override string OriginalText { get; set; }

        /// <summary>
        ///     Gets or sets the translation.
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Description = "The translation of the original text.",
            Name = "Translation", Order = 20)]
        [CultureSpecific]
        [Required(AllowEmptyStrings = true)]
        public virtual string Translation { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Sets the default property values on the page data.
        /// </summary>
        /// <param name="contentType">
        /// The type of content.
        /// </param>
        /// <example>
        /// <code source="../CodeSamples/EPiServer/Core/PageDataSamples.aspx.cs" region="DefaultValues">
        /// </code>
        /// </example>
        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            this.VisibleInMenu = false;
        }

        public override void SetPageData(System.Xml.XmlNode node)
        {
            string name = string.Empty;
            if (node.Attributes[NAME_ATTRIBUTE] != null && !string.IsNullOrEmpty(node.Attributes[NAME_ATTRIBUTE].InnerText))
                name = node.Attributes[NAME_ATTRIBUTE].InnerText;
            else
                name = node.Name;

            this.Name = name;
            this.OriginalText = name;

            string translation = string.Empty;
            if (!string.IsNullOrEmpty(node.InnerText))
                translation = node.InnerText;
            else if (node.Attributes[DESC_ATTRIBUTE] != null && !string.IsNullOrEmpty(node.Attributes["description"].InnerText))
                translation = node.Attributes[DESC_ATTRIBUTE].InnerText;

            this.Translation = translation;
        }

        #endregion
    }
}
