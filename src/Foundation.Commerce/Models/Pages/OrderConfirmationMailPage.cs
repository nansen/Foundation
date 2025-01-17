﻿using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using Foundation.Cms.Pages;

namespace Foundation.Commerce.Models.Pages
{
    [ContentType(DisplayName = "Order Confirmation Mail Page", 
        GUID = "f13b7a68-0702-4023-92b3-15064d338c0c",
        Description = "The reset passord template mail page.",
        GroupName = CommerceGroupNames.Commerce,
        AvailableInEditMode =false)]
    [ImageUrl("~/assets/icons/cms/pages/CMS-icon-page-26.png")]
    public class OrderConfirmationMailPage : MailBasePage
    {
        
    }
}