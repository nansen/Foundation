﻿using EPiServer.Commerce.Order;
using Foundation.Commerce.Customer.ViewModels;
using Foundation.Commerce.Models.Pages;
using Foundation.Commerce.Order.ViewModels;
using Mediachase.Commerce.Customers;
using System.Collections.Generic;

namespace Foundation.Commerce.Customer.Services
{
    public interface IAddressBookService
    {
        AddressCollectionViewModel GetAddressBookViewModel(AddressBookPage addressBookPage);
        IList<AddressModel> List();
        bool CanSave(AddressModel addressModel);
        void Save(AddressModel addressModel, FoundationContact contact = null);
        void Delete(string addressId);
        void SetPreferredBillingAddress(string addressId);
        void SetPreferredShippingAddress(string addressId);
        CustomerAddress GetPreferredBillingAddress();
        void LoadAddress(AddressModel addressModel);
        void LoadCountriesAndRegionsForAddress(AddressModel addressModel);
        IEnumerable<string> GetRegionsByCountryCode(string countryCode);
        void MapToAddress(AddressModel addressModel, IOrderAddress orderAddress);
        void MapToAddress(AddressModel addressModel, CustomerAddress customerAddress);
        void MapToModel(CustomerAddress customerAddress, AddressModel addressModel);
        void MapToModel(IOrderAddress orderAddress, AddressModel addressModel);
        IOrderAddress ConvertToAddress(AddressModel addressModel, IOrderGroup orderGroup);
        AddressModel ConvertToModel(IOrderAddress orderAddress);

        IList<AddressModel> MergeAnonymousShippingAddresses(IList<AddressModel> addresses,
            IEnumerable<CartItemViewModel> cartItems);

        bool UseBillingAddressForShipment();
        void UpdateOrganizationAddress(FoundationOrganization organization, B2BAddressViewModel addressModel);
        IEnumerable<CountryViewModel> GetAllCountries();
        string GetCountryNameByCode(string code);
        void DeleteAddress(string organizationId, string addressId);
    }
}