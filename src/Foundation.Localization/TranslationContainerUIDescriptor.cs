using EPiServer.Shell;
using Foundation.Localization.Models;

namespace Foundation.Localization
{
    [UIDescriptorRegistration]
    public class TranslationContainerUIDescriptor : UIDescriptor<TranslationContainer>
    {
        public TranslationContainerUIDescriptor() : base("epi-iconObjectFolderAllSites")
        {

        }
    }
}
