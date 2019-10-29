﻿using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Filters;
using System.ComponentModel.DataAnnotations;

namespace Foundation.Localization.Models
{
    /// <summary>
    ///     A container to hold translations.
    /// </summary>
    /// <remarks>
    ///     To reference this use the name without spaces and special characters, all lower case.
    ///     So if the Name of the container is e.g. Jeroen Stemerdink, the key in the xml will be jeroenstemerdink.
    /// </remarks>
    /// <example>
    ///     <![CDATA[
    ///         <EPiServer:Translate runat="server" Text="/jeroenstemerdink/textone" />
    ///     ]]>
    /// </example>
    [ContentType(GUID = "40393E0A-81EF-4B9A-B0AC-F883C036359D", AvailableInEditMode = true, Description = "Container to hold translations", DisplayName = "Translation container", GroupName = "Localization")]
    [AvailableContentTypes(Include = new[] { typeof(TranslationItem), typeof(TranslationContainer) })]
    public class TranslationContainer : BaseLocalizationPage
    {
        [Display(GroupName = SystemTabNames.Content, Description = "The text to translate.", Name = "XML node name", Order = 10)]
        [Required(AllowEmptyStrings = false)]
        public override string OriginalText { get; set; }

        /// <summary>
        /// Sets the default property values on the page data.
        /// </summary>
        /// <param name="contentType">The type of content.</param>
        /// <example>
        ///   <code source="../CodeSamples/EPiServer/Core/PageDataSamples.aspx.cs" region="DefaultValues" />
        /// </example>
        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            this[MetaDataProperties.PageChildOrderRule] = FilterSortOrder.Alphabetical;
            VisibleInMenu = false;
            OriginalText = Name;
        }

        public override void SetPageData(System.Xml.XmlNode node)
        {
            Name = node.Name;
        }
    }
}
