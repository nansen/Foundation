﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Localization.XmlResources;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Foundation.Localization.Models;

namespace Foundation.Localization
{
    /// <summary>
    ///     An <see cref="XmlLocalizationProvider"/> which extracts translation XML from the page tree
    /// </summary>
    public class ContentXmlLocalizationProvider : XmlLocalizationProvider
    {
        /// <summary>
        ///     The backup xml, to be returned when something goes wrong.
        /// </summary>
        private const string BackupXml = @"<?xml version='1.0' encoding='utf-8'?><languages></languages>";

        private static readonly ILogger Logger = LogManager.GetLogger(typeof(ContentXmlLocalizationProvider));
        private static readonly IContentRepository ContentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
        private static readonly ILocalizationSettingsRepository _settingsRepository = ServiceLocator.Current.GetInstance<ILocalizationSettingsRepository>();

        private ContentReference _translationContainer;
        private ContentReference TranslationContainer
        {
            get
            {
                if (_translationContainer == null)
                {
                    var settingsPage = _settingsRepository.GetDefaultSiteSettings();
                    _translationContainer = settingsPage != null && settingsPage.TranslationsRoot != null ? settingsPage.TranslationsRoot : null;
                }
                return _translationContainer;
            }
        }

        private IEnumerable<CultureInfo> _availableLanguages;
        public override IEnumerable<CultureInfo> AvailableLanguages
        {
            get
            {
                if (_availableLanguages == null)
                {
                    IEnumerable<CultureInfo> languages;
                    try
                    {
                        languages = ContentRepository.GetLanguageBranches<PageData>(TranslationContainer)
                            .Select(pageData => pageData.Language)
                            .ToList();
                    }
                    catch
                    {
                        //TranslationContainer might not have a value. If this is the case, IContentRepository throws an exception. if this happens, fall back to the base implementation of this property
                        languages = base.AvailableLanguages;
                    }

                    if (languages.IsNullOrEmpty() || languages.Any(l => String.IsNullOrEmpty(l.NativeName) || String.IsNullOrEmpty(l.Name)))
                    {
                        languages = new List<CultureInfo> { LanguageLoaderOption.MasterLanguage().Language };
                    }

                    _availableLanguages = languages;
                }
                return _availableLanguages;
            }
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">
        /// The friendly name of the provider.
        /// </param>
        /// <param name="config">
        /// A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.
        /// </param>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
            LoadTranslations();
        }

        /// <summary>
        ///     Load the translations.
        /// </summary>
        private void LoadTranslations()
        {
            try
            {
                var translationsXml = GetTranslationXmlString();
                var byteArray = Encoding.UTF8.GetBytes(translationsXml);

                using (var stream = new MemoryStream(byteArray))
                {
                    Load(stream);
                }
            }
            catch (ArgumentNullException argumentNullException)
            {
                Logger.Error("No xml generated by the TranslationFactory while handling Reload event.", argumentNullException);
            }
            catch (EncoderFallbackException encoderFallbackException)
            {
                Logger.Error("Encoder exception while handling Reload event.", encoderFallbackException);
            }
            catch (XmlException xmlException)
            {
                Logger.Error("Invalid xml generated by the TranslationFactory while handling Reload event.", xmlException);
            }
        }

        /// <summary>
        ///     Generate the language xml from the translation containers and items.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private string GetTranslationXmlString()
        {
            XElement returnXml = null;
            MemoryStream ms = null;

            try
            {
                ms = new MemoryStream();

                var xw = XmlWriter.Create(ms, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = false, Encoding = Encoding.UTF8 });

                xw.WriteStartDocument();
                xw.WriteStartElement("languages");

                foreach (var cultureInfo in AvailableLanguages)
                {
                    xw.WriteStartElement("language");
                    xw.WriteAttributeString("name", cultureInfo.EnglishName);
                    xw.WriteAttributeString("id", cultureInfo.Name);

                    AddContainerElement(xw, TranslationContainer, cultureInfo);

                    xw.WriteFullEndElement();
                }

                xw.WriteFullEndElement();
                xw.WriteEndDocument();
                xw.Flush();

                ms.Position = 0;

                var xr = XmlReader.Create(ms);

                returnXml = XElement.Load(xr);

                xw.Close();
            }
            catch (Exception e)
            {
                if (ms != null)
                {
                    ms.Dispose();
                }

                Logger.Error("Error occurred while attempting to create XDocument for translations. Falling back to backup xml", e);
            }

            return returnXml != null ? returnXml.ToString() : XDocument.Parse(BackupXml).ToString();
        }

        /// <summary>
        /// Add a category element.
        /// </summary>
        /// <param name="xmlWriter">
        /// The xmlWriter.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture info.
        /// </param>
        private static void AddCategoryElement(XmlWriter xmlWriter, ContentReference container, CultureInfo cultureInfo)
        {
            var translationItemReferences = ContentRepository.GetDescendents(container).ToList();
            var translationItems = ContentRepository.GetItems(translationItemReferences, new LoaderOptions { LanguageLoaderOption.Fallback(cultureInfo) })
                                                    .OfType<TranslationItem>()
                                                    .ToList();

            foreach (var translationItem in translationItems)
            {
                xmlWriter.WriteStartElement("category");
                xmlWriter.WriteAttributeString("name", translationItem.OriginalText);
                xmlWriter.WriteElementString("description", translationItem.Translation);
                xmlWriter.WriteFullEndElement();
            }
        }

        /// <summary>
        /// The add element.
        /// </summary>
        /// <param name="xw">
        /// The xmlWriter.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture info.
        /// </param>
        private static void AddContainerElement(XmlWriter xw, ContentReference container, CultureInfo cultureInfo)
        {
            var languageOptions = new LanguageLoaderOption
            {
                Language = cultureInfo,
                FallbackBehaviour = LanguageBehaviour.Fallback
            };

            var containerChildren = ContentRepository.GetChildren<PageData>(container, new LoaderOptions { languageOptions }).ToList();

            foreach (var child in containerChildren)
            {
                var translationContainer = child as TranslationContainer;

                if (translationContainer != null)
                {
                    string key = Regex.Replace(translationContainer.OriginalText.ToLowerInvariant(), @"[^A-Za-z0-9]+", String.Empty);

                    xw.WriteStartElement(key);

                    AddContainerElement(xw, child.PageLink, cultureInfo);

                    xw.WriteFullEndElement();
                }

                var translationItem = child as TranslationItem;

                if (translationItem != null)
                {
                    string key = Regex.Replace(translationItem.OriginalText.ToLowerInvariant(), @"[^A-Za-z0-9]+", String.Empty);
                    xw.WriteElementString(key, translationItem.Translation);
                }
            }
        }
    }
}