using EPiServer.Shell;
using Foundation.Localization.Models;

namespace Foundation.Localization
{
    [UIDescriptorRegistration]
    public class TranslationItemUIDescriptor : UIDescriptor<TranslationItem>
    {
        public TranslationItemUIDescriptor() : base("epi-iconObjectTranslation")
        {

        }
    }
}
