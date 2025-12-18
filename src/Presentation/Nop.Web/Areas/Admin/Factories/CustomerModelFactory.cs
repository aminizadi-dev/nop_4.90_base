using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
//COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
//Removed: using Nop.Core.Domain.Forums;
//Removed: using Nop.Core.Domain.Gdpr;
//Removed: using Nop.Core.Domain.Tax;
//Removed: using Nop.Services.Affiliates;
//Removed: using Nop.Services.Attributes;
//Removed: using Nop.Services.Authentication.External;
//COMMERCE DOMAIN/SERVICES REMOVED - Phase C
//Removed: using Nop.Core.Domain.Catalog;
//Removed: using Nop.Core.Domain.Orders;
//Removed: using Nop.Services.Catalog;
//Removed: using Nop.Services.Orders;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
//COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
//Removed: using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Stores;
//Removed: using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Customers;
//COMMERCE MODELS REMOVED - Phase C
//Removed: using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the customer model factory implementation
/// </summary>
public partial class CustomerModelFactory : ICustomerModelFactory
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly DateTimeSettings _dateTimeSettings;
    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: protected readonly GdprSettings _gdprSettings;
    //Removed: protected readonly ForumSettings _forumSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    //Removed: protected readonly IAffiliateService _affiliateService;
    //Removed: protected readonly IAttributeFormatter<AddressAttribute, AddressAttributeValue> _addressAttributeFormatter;
    //Removed: protected readonly IAttributeParser<CustomerAttribute, CustomerAttributeValue> _customerAttributeParser;
    //Removed: protected readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    //Removed: protected readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    //Removed: protected readonly ICustomWishlistService _customWishlistService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: protected readonly IExternalAuthenticationService _externalAuthenticationService;
    //Removed: protected readonly IGdprService _gdprService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IGeoLookupService _geoLookupService;
    protected readonly ILocalizationService _localizationService;
    //COMMERCE SERVICES REMOVED - Phase C
    //Removed: protected readonly IOrderService _orderService;
    protected readonly IPictureService _pictureService;
    //Removed: protected readonly IPriceFormatter _priceFormatter;
    //Removed: protected readonly IProductAttributeFormatter _productAttributeFormatter;
    //Removed: protected readonly IProductService _productService;
    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: protected readonly IRewardPointService _rewardPointService;
    //Removed: protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: protected readonly ITaxService _taxService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: protected readonly RewardPointsSettings _rewardPointsSettings;
    //Removed: protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public CustomerModelFactory(AddressSettings addressSettings,
        CustomerSettings customerSettings,
        DateTimeSettings dateTimeSettings,
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: GdprSettings gdprSettings,
        //Removed: ForumSettings forumSettings,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        //Removed: IAffiliateService affiliateService,
        //Removed: IAttributeFormatter<AddressAttribute, AddressAttributeValue> addressAttributeFormatter,
        //Removed: IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
        //Removed:         //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
        //Removed: IAuthenticationPluginManager authenticationPluginManager,
        //Removed: IBackInStockSubscriptionService backInStockSubscriptionService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICountryService countryService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: ICustomWishlistService customWishlistService,
        IDateTimeHelper dateTimeHelper,
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: IExternalAuthenticationService externalAuthenticationService,
        //Removed: IGdprService gdprService,
        IGenericAttributeService genericAttributeService,
        IGeoLookupService geoLookupService,
        ILocalizationService localizationService,
        //COMMERCE SERVICES REMOVED - Phase C
        //Removed: IOrderService orderService,
        IPictureService pictureService,
        //Removed: IPriceFormatter priceFormatter,
        //Removed: IProductAttributeFormatter productAttributeFormatter,
        //Removed: IProductService productService,
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: IRewardPointService rewardPointService,
        //Removed: IShoppingCartService shoppingCartService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStoreService storeService,
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: ITaxService taxService,
        IWorkContext workContext,
        MediaSettings mediaSettings)
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: RewardPointsSettings rewardPointsSettings)
        //Removed: TaxSettings taxSettings)
    {
        _addressSettings = addressSettings;
        _customerSettings = customerSettings;
        _dateTimeSettings = dateTimeSettings;
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: _gdprSettings = gdprSettings;
        //Removed: _forumSettings = forumSettings;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        //Removed: _affiliateService = affiliateService;
        //Removed: _addressAttributeFormatter = addressAttributeFormatter;
        //Removed: _customerAttributeParser = customerAttributeParser;
        //Removed: _customerAttributeService = customerAttributeService;
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: _authenticationPluginManager = authenticationPluginManager;
        //Removed: _backInStockSubscriptionService = backInStockSubscriptionService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _countryService = countryService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: _customWishlistService = customWishlistService;
        _dateTimeHelper = dateTimeHelper;
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: _externalAuthenticationService = externalAuthenticationService;
        //Removed: _gdprService = gdprService;
        _genericAttributeService = genericAttributeService;
        _geoLookupService = geoLookupService;
        _localizationService = localizationService;
        //COMMERCE SERVICES REMOVED - Phase C
        //Removed: _orderService = orderService;
        _pictureService = pictureService;
        //Removed: _priceFormatter = priceFormatter;
        //Removed: _productAttributeFormatter = productAttributeFormatter;
        //Removed: _productService = productService;
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: _rewardPointService = rewardPointService;
        //Removed: _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _storeService = storeService;
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: _taxService = taxService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: _rewardPointsSettings = rewardPointsSettings;
        //Removed: _taxSettings = taxSettings;
    }

    #endregion

    #region Utilities

    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: PrepareAddRewardPointsToCustomerModelAsync (reward points feature)

    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: PrepareAssociatedExternalAuthModelsAsync (external authentication feature)
    //Removed: PrepareCustomerAttributeModelsAsync (customer attributes feature)

    /// <summary>
    /// Prepare HTML string address
    /// </summary>
    /// <param name="model">Address model</param>
    /// <param name="address">Address</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareModelAddressHtmlAsync(AddressModel model, Address address)
    {
        ArgumentNullException.ThrowIfNull(model);

        var separator = "<br />";
        var addressHtmlSb = new StringBuilder("<div>");
        var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
        var (addressLine, _) = await _addressService.FormatAddressAsync(address, languageId, separator, true);
        addressHtmlSb.Append(addressLine);

        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: Address custom attributes formatting

        addressHtmlSb.Append("</div>");

        model.AddressHtml = addressHtmlSb.ToString();
    }

    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: PrepareRewardPointsSearchModel (reward points feature)

    /// <summary>
    /// Prepare customer address search model
    /// </summary>
    /// <param name="searchModel">Customer address search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>Customer address search model</returns>
    protected virtual CustomerAddressSearchModel PrepareCustomerAddressSearchModel(CustomerAddressSearchModel searchModel, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        searchModel.CustomerId = customer.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareCustomerOrderSearchModel (commerce feature)
    //Removed: PrepareCustomerShoppingCartSearchModelAsync (commerce feature)

    /// <summary>
    /// Prepare customer activity log search model
    /// </summary>
    /// <param name="searchModel">Customer activity log search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>Customer activity log search model</returns>
    protected virtual CustomerActivityLogSearchModel PrepareCustomerActivityLogSearchModel(CustomerActivityLogSearchModel searchModel, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        searchModel.CustomerId = customer.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: PrepareCustomerBackInStockSubscriptionSearchModel (commerce feature)
    //Removed: PrepareCustomerAssociatedExternalAuthRecordsSearchModelAsync (external authentication feature)

    #endregion

    #region Methods

    /// <summary>
    /// Prepare customer search model
    /// </summary>
    /// <param name="searchModel">Customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer search model
    /// </returns>
    public virtual async Task<CustomerSearchModel> PrepareCustomerSearchModelAsync(CustomerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.UsernamesEnabled = _customerSettings.UsernamesEnabled;
        searchModel.AvatarEnabled = _customerSettings.AllowCustomersToUploadAvatars;
        searchModel.FirstNameEnabled = _customerSettings.FirstNameEnabled;
        searchModel.LastNameEnabled = _customerSettings.LastNameEnabled;
        searchModel.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        searchModel.CompanyEnabled = _customerSettings.CompanyEnabled;
        searchModel.PhoneEnabled = _customerSettings.PhoneEnabled;
        searchModel.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;

        //search registered customers by default
        var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
        if (registeredRole != null)
            searchModel.SelectedCustomerRoleIds.Add(registeredRole.Id);

        searchModel.AvailableActiveValues = new List<SelectListItem> {
            new(await _localizationService.GetResourceAsync("Admin.Common.All"), string.Empty),
            new(await _localizationService.GetResourceAsync("Admin.Common.Yes"), true.ToString(), true),
            new(await _localizationService.GetResourceAsync("Admin.Common.No"), false.ToString())
        };

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged customer list model
    /// </summary>
    /// <param name="searchModel">Customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer list model
    /// </returns>
    public virtual async Task<CustomerListModel> PrepareCustomerListModelAsync(CustomerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter customers
        _ = int.TryParse(searchModel.SearchDayOfBirth, out var dayOfBirth);
        _ = int.TryParse(searchModel.SearchMonthOfBirth, out var monthOfBirth);
        var createdFromUtc = !searchModel.SearchRegistrationDateFrom.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchRegistrationDateFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var createdToUtc = !searchModel.SearchRegistrationDateTo.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchRegistrationDateTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
        var lastActivityFromUtc = !searchModel.SearchLastActivityFrom.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
        var lastActivityToUtc = !searchModel.SearchLastActivityTo.HasValue ? null
            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchLastActivityTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

        //exclude guests from the result when filter "by registration date" is used
        if (createdFromUtc.HasValue || createdToUtc.HasValue)
        {
            if (!searchModel.SelectedCustomerRoleIds.Any())
            {
                var customerRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
                searchModel.SelectedCustomerRoleIds = customerRoles
                    .Where(cr => cr.SystemName != NopCustomerDefaults.GuestsRoleName).Select(cr => cr.Id).ToList();
            }
            else
            {
                var guestRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.GuestsRoleName);
                if (guestRole != null)
                    searchModel.SelectedCustomerRoleIds.Remove(guestRole.Id);
            }
        }

        //get customers
        var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: searchModel.SelectedCustomerRoleIds.ToArray(),
            email: searchModel.SearchEmail,
            username: searchModel.SearchUsername,
            firstName: searchModel.SearchFirstName,
            lastName: searchModel.SearchLastName,
            dayOfBirth: dayOfBirth,
            monthOfBirth: monthOfBirth,
            company: searchModel.SearchCompany,
            createdFromUtc: createdFromUtc,
            createdToUtc: createdToUtc,
            lastActivityFromUtc: lastActivityFromUtc,
            lastActivityToUtc: lastActivityToUtc,
            phone: searchModel.SearchPhone,
            zipPostalCode: searchModel.SearchZipPostalCode,
            ipAddress: searchModel.SearchIpAddress,
            isActive: searchModel.SearchIsActive,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new CustomerListModel().PrepareToGridAsync(searchModel, customers, () =>
        {
            return customers.SelectAwait(async customer =>
            {
                //fill in model values from the entity
                var customerModel = customer.ToModel<CustomerModel>();

                //convert dates to the user time
                customerModel.Email = (await _customerService.IsRegisteredAsync(customer))
                    ? customer.Email
                    : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                customerModel.FullName = await _customerService.GetCustomerFullNameAsync(customer);
                customerModel.Company = customer.Company;
                customerModel.Phone = customer.Phone;
                customerModel.ZipPostalCode = customer.ZipPostalCode;

                customerModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc);
                customerModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                customerModel.CustomerRoleNames = string.Join(", ",
                    (await _customerService.GetCustomerRolesAsync(customer)).Select(role => role.Name));
                if (_customerSettings.AllowCustomersToUploadAvatars)
                {
                    var avatarPictureId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute);
                    customerModel.AvatarUrl = await _pictureService
                        .GetPictureUrlAsync(avatarPictureId, _mediaSettings.AvatarPictureSize, _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                }

                return customerModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare customer model
    /// </summary>
    /// <param name="model">Customer model</param>
    /// <param name="customer">Customer</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer model
    /// </returns>
    public virtual async Task<CustomerModel> PrepareCustomerModelAsync(CustomerModel model, Customer customer, bool excludeProperties = false)
    {
        if (customer != null)
        {
            //fill in model values from the entity
            model ??= new CustomerModel();

            model.Id = customer.Id;
            //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
            //Removed: model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //Removed: model.AllowSendingOfPrivateMessage = await _customerService.IsRegisteredAsync(customer) && _forumSettings.AllowPrivateMessages;
            model.AllowSendingOfWelcomeMessage = await _customerService.IsRegisteredAsync(customer) &&
                                                 _customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval;
            model.AllowReSendingOfActivationMessage = await _customerService.IsRegisteredAsync(customer) && !customer.Active &&
                                                      _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
            //Removed: model.GdprEnabled = _gdprSettings.GdprEnabled;

            model.MultiFactorAuthenticationProvider = await _genericAttributeService
                .GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);

            //whether to fill in some of properties
            if (!excludeProperties)
            {
                model.Email = customer.Email;
                model.Username = customer.Username;
                //COMMERCE FEATURES REMOVED - Phase C
                //Removed: model.VendorId = customer.VendorId;
                model.AdminComment = customer.AdminComment;
                //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
                //Removed: model.IsTaxExempt = customer.IsTaxExempt;
                model.Active = customer.Active;
                model.FirstName = customer.FirstName;
                model.LastName = customer.LastName;
                model.Gender = customer.Gender;
                model.DateOfBirth = customer.DateOfBirth;
                model.Company = customer.Company;
                model.StreetAddress = customer.StreetAddress;
                model.StreetAddress2 = customer.StreetAddress2;
                model.ZipPostalCode = customer.ZipPostalCode;
                model.City = customer.City;
                model.County = customer.County;
                model.CountryId = customer.CountryId;
                model.StateProvinceId = customer.StateProvinceId;
                model.Phone = customer.Phone;
                model.Fax = customer.Fax;
                model.TimeZoneId = customer.TimeZoneId;
                //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
                //Removed: model.VatNumber = customer.VatNumber;
                //Removed: model.VatNumberStatusNote = await _localizationService.GetLocalizedEnumAsync(customer.VatNumberStatus);
                model.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);
                model.LastIpAddress = customer.LastIpAddress;
                model.LastVisitedPage = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastVisitedPageAttribute);
                model.SelectedCustomerRoleIds = (await _customerService.GetCustomerRoleIdsAsync(customer)).ToList();
                model.RegisteredInStore = (await _storeService.GetAllStoresAsync())
                    .FirstOrDefault(store => store.Id == customer.RegisteredInStoreId)?.Name ?? string.Empty;
                model.DisplayRegisteredInStore = model.Id > 0 && !string.IsNullOrEmpty(model.RegisteredInStore) &&
                                                 (await _storeService.GetAllStoresAsync()).Select(x => x.Id).Count() > 1;
                model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc);

                model.MustChangePassword = customer.MustChangePassword;

                //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
                //Removed: Affiliate model preparation
            }
            //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
            //Removed: Reward points model preparation
            //Removed: PrepareRewardPointsSearchModel(model.CustomerRewardPointsSearchModel, customer);

            //prepare nested search models
            PrepareCustomerAddressSearchModel(model.CustomerAddressSearchModel, customer);
            //COMMERCE FEATURES REMOVED - Phase C
            //Removed: PrepareCustomerOrderSearchModel(model.CustomerOrderSearchModel, customer);
            //Removed: await PrepareCustomerShoppingCartSearchModelAsync(model.CustomerShoppingCartSearchModel, customer);
            PrepareCustomerActivityLogSearchModel(model.CustomerActivityLogSearchModel, customer);
            //Removed: PrepareCustomerBackInStockSubscriptionSearchModel(model.CustomerBackInStockSubscriptionSearchModel, customer);
            //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
            //Removed: await PrepareCustomerAssociatedExternalAuthRecordsSearchModelAsync(model.CustomerAssociatedExternalAuthRecordsSearchModel, customer);
        }
        else
        {
            //whether to fill in some of properties
            if (!excludeProperties)
            {
                //precheck Registered Role as a default role while creating a new customer through admin
                var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
                if (registeredRole != null)
                    model.SelectedCustomerRoleIds.Add(registeredRole.Id);
            }
        }

        model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
        model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
        model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
        model.LastNameEnabled = _customerSettings.LastNameEnabled;
        model.GenderEnabled = _customerSettings.GenderEnabled;
        model.NeutralGenderEnabled = _customerSettings.NeutralGenderEnabled;
        model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        model.CompanyEnabled = _customerSettings.CompanyEnabled;
        model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
        model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
        model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
        model.CityEnabled = _customerSettings.CityEnabled;
        model.CountyEnabled = _customerSettings.CountyEnabled;
        model.CountryEnabled = _customerSettings.CountryEnabled;
        model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
        model.PhoneEnabled = _customerSettings.PhoneEnabled;
        model.FaxEnabled = _customerSettings.FaxEnabled;

        //set default values for the new model
        if (customer == null)
        {
            model.Active = true;
            //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
            //Removed: model.DisplayVatNumber = false;
        }

        //COMMERCE FEATURES REMOVED - Phase C
        //Removed: await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors,
        //    defaultItemText: await _localizationService.GetResourceAsync("Admin.Customers.Customers.Fields.Vendor.None"));

        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: await PrepareCustomerAttributeModelsAsync(model.CustomerAttributes, customer);

        //prepare available customer roles
        var availableRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
        model.AvailableCustomerRoles = availableRoles.Select(role => new SelectListItem
        {
            Text = role.Name,
            Value = role.Id.ToString(),
            Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
        }).ToList();

        //prepare available time zones
        await _baseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

        //prepare available countries and states
        if (_customerSettings.CountryEnabled)
        {
            await _baseAdminModelFactory.PrepareCountriesAsync(model.AvailableCountries);
            if (_customerSettings.StateProvinceEnabled)
                await _baseAdminModelFactory.PrepareStatesAndProvincesAsync(model.AvailableStates, model.CountryId == 0 ? null : (int?)model.CountryId);
        }

        return model;
    }

    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: PrepareRewardPointsListModelAsync (reward points feature)

    /// <summary>
    /// Prepare paged customer address list model
    /// </summary>
    /// <param name="searchModel">Customer address search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer address list model
    /// </returns>
    public virtual async Task<CustomerAddressListModel> PrepareCustomerAddressListModelAsync(CustomerAddressSearchModel searchModel, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get customer addresses
        var addresses = (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
            .OrderByDescending(address => address.CreatedOnUtc).ThenByDescending(address => address.Id).ToList()
            .ToPagedList(searchModel);

        //prepare list model
        var model = await new CustomerAddressListModel().PrepareToGridAsync(searchModel, addresses, () =>
        {
            return addresses.SelectAwait(async address =>
            {
                //fill in model values from the entity        
                var addressModel = address.ToModel<AddressModel>();

                addressModel.CountryName = (await _countryService.GetCountryByAddressAsync(address))?.Name;
                addressModel.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Name;

                //fill in additional values (not existing in the entity)
                await PrepareModelAddressHtmlAsync(addressModel, address);

                return addressModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare customer address model
    /// </summary>
    /// <param name="model">Customer address model</param>
    /// <param name="customer">Customer</param>
    /// <param name="address">Address</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer address model
    /// </returns>
    public virtual async Task<CustomerAddressModel> PrepareCustomerAddressModelAsync(CustomerAddressModel model,
        Customer customer, Address address, bool excludeProperties = false)
    {
        ArgumentNullException.ThrowIfNull(customer);

        if (address != null)
        {
            //fill in model values from the entity
            model ??= new CustomerAddressModel();

            //whether to fill in some of properties
            if (!excludeProperties)
                model.Address = address.ToModel(model.Address);
        }

        model.CustomerId = customer.Id;

        //prepare address model
        await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
        model.Address.FirstNameRequired = true;
        model.Address.LastNameRequired = true;
        model.Address.EmailRequired = true;
        model.Address.CompanyRequired = _addressSettings.CompanyRequired;
        model.Address.CityRequired = _addressSettings.CityRequired;
        model.Address.CountyRequired = _addressSettings.CountyRequired;
        model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
        model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
        model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
        model.Address.PhoneRequired = _addressSettings.PhoneRequired;
        model.Address.FaxRequired = _addressSettings.FaxRequired;

        return model;
    }

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareCustomerOrderListModelAsync (commerce feature)
    //Removed: PrepareCustomerShoppingCartListModelAsync (commerce feature)

    /// <summary>
    /// Prepare paged customer activity log list model
    /// </summary>
    /// <param name="searchModel">Customer activity log search model</param>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer activity log list model
    /// </returns>
    public virtual async Task<CustomerActivityLogListModel> PrepareCustomerActivityLogListModelAsync(CustomerActivityLogSearchModel searchModel, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(customer);

        //get customer activity log
        var activityLog = await _customerActivityService.GetAllActivitiesAsync(customerId: customer.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new CustomerActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
        {
            return activityLog.SelectAwait(async logItem =>
            {
                //fill in model values from the entity
                var customerActivityLogModel = logItem.ToModel<CustomerActivityLogModel>();

                //fill in additional values (not existing in the entity)
                customerActivityLogModel.ActivityLogTypeName = (await _customerActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;

                //convert dates to the user time
                customerActivityLogModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

                return customerActivityLogModel;
            });
        });

        return model;
    }

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareCustomerBackInStockSubscriptionListModelAsync (commerce feature)

    /// <summary>
    /// Prepare online customer search model
    /// </summary>
    /// <param name="searchModel">Online customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the online customer search model
    /// </returns>
    public virtual Task<OnlineCustomerSearchModel> PrepareOnlineCustomerSearchModelAsync(OnlineCustomerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged online customer list model
    /// </summary>
    /// <param name="searchModel">Online customer search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the online customer list model
    /// </returns>
    public virtual async Task<OnlineCustomerListModel> PrepareOnlineCustomerListModelAsync(OnlineCustomerSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter customers
        var lastActivityFrom = DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes);

        //get online customers
        var customers = await _customerService.GetOnlineCustomersAsync(customerRoleIds: searchModel.SelectedCustomerRoleIds.ToArray(),
            lastActivityFromUtc: lastActivityFrom,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new OnlineCustomerListModel().PrepareToGridAsync(searchModel, customers, () =>
        {
            return customers.SelectAwait(async customer =>
            {
                //fill in model values from the entity
                var customerModel = customer.ToModel<OnlineCustomerModel>();

                //convert dates to the user time
                customerModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                customerModel.CustomerInfo = (await _customerService.IsRegisteredAsync(customer))
                    ? customer.Email
                    : await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                customerModel.LastIpAddress = _customerSettings.StoreIpAddresses
                    ? customer.LastIpAddress
                    : await _localizationService.GetResourceAsync("Admin.Customers.OnlineCustomers.Fields.IPAddress.Disabled");
                customerModel.Location = _geoLookupService.LookupCountryName(customer.LastIpAddress);
                customerModel.LastVisitedPage = _customerSettings.StoreLastVisitedPage
                    ? await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastVisitedPageAttribute)
                    : await _localizationService.GetResourceAsync("Admin.Customers.OnlineCustomers.Fields.LastVisitedPage.Disabled");

                return customerModel;
            });
        });

        return model;
    }

    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: PrepareGdprLogSearchModelAsync (GDPR feature)
    //Removed: PrepareGdprLogListModelAsync (GDPR feature)

    #endregion
}