using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
//COMMERCE DOMAIN REMOVED - Phase C
//Removed: using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
//Removed: using Nop.Core.Domain.Forums;
//Removed: using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
//Removed: using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
//Removed: using Nop.Core.Domain.Tax;
//Removed: using Nop.Core.Domain.Vendors;
using Nop.Core.Http;
//COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
//Removed: using Nop.Services.Attributes;
//Removed: using Nop.Services.Authentication.External;
//COMMERCE SERVICES REMOVED - Phase C
//Removed: using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
//Removed: using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
//Removed: using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the customer model factory
/// </summary>
public partial class CustomerModelFactory : ICustomerModelFactory
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CaptchaSettings _captchaSettings;
    //COMMERCE SETTINGS REMOVED - Phase C
    //Removed: protected readonly CatalogSettings _catalogSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly DateTimeSettings _dateTimeSettings;
    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: protected readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
    //Removed: protected readonly ForumSettings _forumSettings;
    //Removed: protected readonly GdprSettings _gdprSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    //Removed: protected readonly IAttributeParser<CustomerAttribute, CustomerAttributeValue> _customerAttributeParser;
    //Removed: protected readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    //Removed: protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: protected readonly IExternalAuthenticationModelFactory _externalAuthenticationModelFactory;
    //Removed: protected readonly IExternalAuthenticationService _externalAuthenticationService;
    //Removed: protected readonly IGdprService _gdprService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    //Removed: protected readonly IOrderService _orderService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    //Removed: protected readonly IProductService _productService;
    //Removed: protected readonly IReturnRequestService _returnRequestService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    //COMMERCE SETTINGS REMOVED - Phase C
    //Removed: protected readonly OrderSettings _orderSettings;
    //Removed: protected readonly RewardPointsSettings _rewardPointsSettings;
    protected readonly SecuritySettings _securitySettings;
    //Removed: protected readonly TaxSettings _taxSettings;
    //Removed: protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public CustomerModelFactory(AddressSettings addressSettings,
        CaptchaSettings captchaSettings,
        //COMMERCE SETTINGS REMOVED - Phase C
        //Removed: CatalogSettings catalogSettings,
        CommonSettings commonSettings,
        CustomerSettings customerSettings,
        DateTimeSettings dateTimeSettings,
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: ExternalAuthenticationSettings externalAuthenticationSettings,
        //Removed: ForumSettings forumSettings,
        //Removed: GdprSettings gdprSettings,
        IAddressModelFactory addressModelFactory,
        //Removed: IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
        //Removed: IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
        //Removed: IAuthenticationPluginManager authenticationPluginManager,
        ICountryService countryService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: IExternalAuthenticationModelFactory externalAuthenticationModelFactory,
        //Removed: IExternalAuthenticationService externalAuthenticationService,
        //Removed: IGdprService gdprService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        //Removed: IOrderService orderService,
        IPermissionService permissionService,
        IPictureService pictureService,
        //Removed: IProductService productService,
        //Removed: IReturnRequestService returnRequestService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        //COMMERCE SETTINGS REMOVED - Phase C
        //Removed: OrderSettings orderSettings,
        //Removed: RewardPointsSettings rewardPointsSettings,
        SecuritySettings securitySettings
        //Removed: TaxSettings taxSettings,
        //Removed: VendorSettings vendorSettings
        )
    {
        _addressSettings = addressSettings;
        _captchaSettings = captchaSettings;
        //COMMERCE SETTINGS REMOVED - Phase C
        //Removed: _catalogSettings = catalogSettings;
        _commonSettings = commonSettings;
        _customerSettings = customerSettings;
        _dateTimeSettings = dateTimeSettings;
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: _externalAuthenticationModelFactory = externalAuthenticationModelFactory;
        //Removed: _externalAuthenticationService = externalAuthenticationService;
        //Removed: _externalAuthenticationSettings = externalAuthenticationSettings;
        //Removed: _forumSettings = forumSettings;
        //Removed: _gdprSettings = gdprSettings;
        _addressModelFactory = addressModelFactory;
        //Removed: _customerAttributeParser = customerAttributeParser;
        //Removed: _customerAttributeService = customerAttributeService;
        //Removed: _authenticationPluginManager = authenticationPluginManager;
        _countryService = countryService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        //Removed: _gdprService = gdprService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
        //Removed: _orderService = orderService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        //Removed: _productService = productService;
        //Removed: _returnRequestService = returnRequestService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        //Removed: _orderSettings = orderSettings;
        //Removed: _rewardPointsSettings = rewardPointsSettings;
        _securitySettings = securitySettings;
        //Removed: _taxSettings = taxSettings;
        //Removed: _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: PrepareGdprConsentModelAsync (GDPR feature)

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the customer info model
    /// </summary>
    /// <param name="model">Customer info model</param>
    /// <param name="customer">Customer</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer info model
    /// </returns>
    public virtual async Task<CustomerInfoModel> PrepareCustomerInfoModelAsync(CustomerInfoModel model, Customer customer,
        bool excludeProperties)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(customer);

        model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
        foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
            model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

        var store = await _storeContext.GetCurrentStoreAsync();
        if (!excludeProperties)
        {
            //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
            //Removed: model.VatNumber = customer.VatNumber;
            model.FirstName = customer.FirstName;
            model.LastName = customer.LastName;
            model.Gender = customer.Gender;
            var dateOfBirth = customer.DateOfBirth;
            if (dateOfBirth.HasValue)
            {
                var currentCalendar = CultureInfo.CurrentCulture.Calendar;

                model.DateOfBirthDay = currentCalendar.GetDayOfMonth(dateOfBirth.Value);
                model.DateOfBirthMonth = currentCalendar.GetMonth(dateOfBirth.Value);
                model.DateOfBirthYear = currentCalendar.GetYear(dateOfBirth.Value);
            }
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

            //newsletter subscriptions
            var currentSubscriptions = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionsByEmailAsync(customer.Email, storeId: store.Id);
            var newsLetterSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(store.Id);
            foreach (var newsLetterSubscriptionType in newsLetterSubscriptionTypes)
            {
                var nsModel = new NewsLetterSubscriptionModel
                {
                    TypeId = newsLetterSubscriptionType.Id,
                    Name = await _localizationService.GetLocalizedAsync(newsLetterSubscriptionType, x => x.Name),
                    IsActive = currentSubscriptions.Any(subscription => subscription.TypeId == newsLetterSubscriptionType.Id && subscription.Active)
                };
                model.NewsLetterSubscriptions.Add(nsModel);
            }

            model.Signature = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SignatureAttribute);
            model.Email = customer.Email;
            model.Username = customer.Username;
        }
        else
        {
            if (_customerSettings.UsernamesEnabled && !_customerSettings.AllowUsersToChangeUsernames)
                model.Username = customer.Username;
        }

        if (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
            model.EmailToRevalidate = customer.EmailToRevalidate;

        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
        //countries and states
        if (_customerSettings.CountryEnabled)
        {
            if (model.CountryId == 0)
                model.CountryId = _customerSettings.DefaultCountryId ?? 0;

            model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
            foreach (var c in await _countryService.GetAllCountriesAsync(currentLanguage.Id))
            {
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == model.CountryId
                });
            }

            if (_customerSettings.StateProvinceEnabled)
            {
                //states
                var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
                if (states.Any())
                {
                    model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                    foreach (var s in states)
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                }
                else
                {
                    var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                    model.AvailableStates.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                        Value = "0"
                    });
                }

            }
        }

        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: Tax/VAT related code
        model.DisplayVatNumber = false;
        model.VatNumberRequired = false;
        //Removed: model.VatNumberStatusNote = await _localizationService.GetLocalizedEnumAsync(customer.VatNumberStatus);
        model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
        model.LastNameEnabled = _customerSettings.LastNameEnabled;
        model.FirstNameRequired = _customerSettings.FirstNameRequired;
        model.LastNameRequired = _customerSettings.LastNameRequired;
        model.GenderEnabled = _customerSettings.GenderEnabled;
        model.NeutralGenderEnabled = _customerSettings.NeutralGenderEnabled;
        model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
        model.CompanyEnabled = _customerSettings.CompanyEnabled;
        model.CompanyRequired = _customerSettings.CompanyRequired;
        model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
        model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
        model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
        model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
        model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
        model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
        model.CityEnabled = _customerSettings.CityEnabled;
        model.CityRequired = _customerSettings.CityRequired;
        model.CountyEnabled = _customerSettings.CountyEnabled;
        model.CountyRequired = _customerSettings.CountyRequired;
        model.CountryEnabled = _customerSettings.CountryEnabled;
        model.CountryRequired = _customerSettings.CountryRequired;
        model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
        model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
        model.PhoneEnabled = _customerSettings.PhoneEnabled;
        model.PhoneRequired = _customerSettings.PhoneRequired;
        model.FaxEnabled = _customerSettings.FaxEnabled;
        model.FaxRequired = _customerSettings.FaxRequired;
        model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
        model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
        model.AllowUsersToChangeUsernames = _customerSettings.AllowUsersToChangeUsernames;
        model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: model.SignatureEnabled = _forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled;
        model.SignatureEnabled = false;

        //Removed: External authentication code
        //Removed: Custom customer attributes code

        //COMMERCE FEATURE REMOVED - Phase C
        //Removed: GDPR (GDPR tools - commerce-driven)

        return model;
    }

    /// <summary>
    /// Prepare the customer register model
    /// </summary>
    /// <param name="model">Customer register model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
    /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer register model
    /// </returns>
    public virtual async Task<RegisterModel> PrepareRegisterModelAsync(RegisterModel model, bool excludeProperties,
        bool setDefaultValues = false)
    {
        ArgumentNullException.ThrowIfNull(model);

        var customer = await _workContext.GetCurrentCustomerAsync();

        model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
        foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
            model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: VAT/Tax related code
        model.DisplayVatNumber = false;
        model.VatNumberRequired = false;

        //form fields
        model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
        model.LastNameEnabled = _customerSettings.LastNameEnabled;
        model.FirstNameRequired = _customerSettings.FirstNameRequired;
        model.LastNameRequired = _customerSettings.LastNameRequired;
        model.GenderEnabled = _customerSettings.GenderEnabled;
        model.NeutralGenderEnabled = _customerSettings.NeutralGenderEnabled;
        model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
        model.CompanyEnabled = _customerSettings.CompanyEnabled;
        model.CompanyRequired = _customerSettings.CompanyRequired;
        model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
        model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
        model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
        model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
        model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
        model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
        model.CityEnabled = _customerSettings.CityEnabled;
        model.CityRequired = _customerSettings.CityRequired;
        model.CountyEnabled = _customerSettings.CountyEnabled;
        model.CountyRequired = _customerSettings.CountyRequired;
        model.CountryEnabled = _customerSettings.CountryEnabled;
        model.CountryRequired = _customerSettings.CountryRequired;
        model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
        model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
        model.PhoneEnabled = _customerSettings.PhoneEnabled;
        model.PhoneRequired = _customerSettings.PhoneRequired;
        model.FaxEnabled = _customerSettings.FaxEnabled;
        model.FaxRequired = _customerSettings.FaxRequired;
        model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
        model.AcceptPrivacyPolicyEnabled = _customerSettings.AcceptPrivacyPolicyEnabled;
        model.AcceptPrivacyPolicyPopup = _commonSettings.PopupForTermsOfServiceLinks;
        model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
        model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
        model.HoneypotEnabled = _securitySettings.HoneypotEnabled;
        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
        model.EnteringEmailTwice = _customerSettings.EnteringEmailTwice;
        if (setDefaultValues)
        {
            //newsletter subscriptions
            var store = await _storeContext.GetCurrentStoreAsync();
            var newsLetterSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync(store.Id);
            foreach (var newsLetterSubscriptionType in newsLetterSubscriptionTypes)
            {
                var nsModel = new NewsLetterSubscriptionModel
                {
                    TypeId = newsLetterSubscriptionType.Id,
                    Name = await _localizationService.GetLocalizedAsync(newsLetterSubscriptionType, x => x.Name),
                    IsActive = newsLetterSubscriptionType.TickedByDefault
                };
                model.NewsLetterSubscriptions.Add(nsModel);
            }
        }

        //countries and states
        if (_customerSettings.CountryEnabled)
        {
            model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
            model.CountryId = _customerSettings.DefaultCountryId ?? 0;
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            foreach (var c in await _countryService.GetAllCountriesAsync(currentLanguage.Id))
            {
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == model.CountryId
                });
            }

            if (_customerSettings.StateProvinceEnabled)
            {
                //states
                var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
                if (states.Any())
                {
                    model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                    foreach (var s in states)
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                }
                else
                {
                    var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                    model.AvailableStates.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                        Value = "0"
                    });
                }

            }
        }

        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: Custom customer attributes code
        //Removed: GDPR consents code

        return model;
    }

    /// <summary>
    /// Prepare the login model
    /// </summary>
    /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the login model
    /// </returns>
    public virtual Task<LoginModel> PrepareLoginModelAsync(bool? checkoutAsGuest)
    {
        var model = new LoginModel
        {
            UsernamesEnabled = _customerSettings.UsernamesEnabled,
            RegistrationType = _customerSettings.UserRegistrationType,
            CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault(),
            DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage
        };

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the password recovery model
    /// </summary>
    /// <param name="model">Password recovery model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the password recovery model
    /// </returns>
    public virtual Task<PasswordRecoveryModel> PreparePasswordRecoveryModelAsync(PasswordRecoveryModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage;

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the register result model
    /// </summary>
    /// <param name="resultId">Value of UserRegistrationType enum</param>
    /// <param name="returnUrl">URL to redirect</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the register result model
    /// </returns>
    public virtual async Task<RegisterResultModel> PrepareRegisterResultModelAsync(int resultId, string returnUrl)
    {
        var resultText = (UserRegistrationType)resultId switch
        {
            UserRegistrationType.Disabled => await _localizationService.GetResourceAsync("Account.Register.Result.Disabled"),
            UserRegistrationType.Standard => await _localizationService.GetResourceAsync("Account.Register.Result.Standard"),
            UserRegistrationType.AdminApproval => await _localizationService.GetResourceAsync("Account.Register.Result.AdminApproval"),
            UserRegistrationType.EmailValidation => await _localizationService.GetResourceAsync("Account.Register.Result.EmailValidation"),
            _ => null
        };

        var model = new RegisterResultModel
        {
            Result = resultText,
            ReturnUrl = returnUrl
        };

        return model;
    }

    /// <summary>
    /// Prepare the customer navigation model
    /// </summary>
    /// <param name="selectedTabId">Identifier of the selected tab</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer navigation model
    /// </returns>
    public virtual async Task<CustomerNavigationModel> PrepareCustomerNavigationModelAsync(int selectedTabId = 0)
    {
        var model = new CustomerNavigationModel();

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = NopRouteNames.General.CUSTOMER_INFO,
            Title = await _localizationService.GetResourceAsync("Account.CustomerInfo"),
            Tab = (int)CustomerNavigationEnum.Info,
            ItemClass = "customer-info"
        });

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = NopRouteNames.General.CUSTOMER_ADDRESSES,
            Title = await _localizationService.GetResourceAsync("Account.CustomerAddresses"),
            Tab = (int)CustomerNavigationEnum.Addresses,
            ItemClass = "customer-addresses"
        });

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = NopRouteNames.General.CUSTOMER_ORDERS,
            Title = await _localizationService.GetResourceAsync("Account.CustomerOrders"),
            Tab = (int)CustomerNavigationEnum.Orders,
            ItemClass = "customer-orders"
        });

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = NopRouteNames.Standard.CUSTOMER_RECURRING_PAYMENTS,
            Title = await _localizationService.GetResourceAsync("Account.CustomerRecurringPayments"),
            Tab = (int)CustomerNavigationEnum.RecurringPayments,
            ItemClass = "customer-recurring-payments"
        });

        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();

        //COMMERCE NAVIGATION ITEMS REMOVED - Phase C
        //Removed: ReturnRequests, DownloadableProducts, BackInStockSubscriptions, RewardPoints (commerce features)

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = NopRouteNames.Standard.CUSTOMER_CHANGE_PASSWORD,
            Title = await _localizationService.GetResourceAsync("Account.ChangePassword"),
            Tab = (int)CustomerNavigationEnum.ChangePassword,
            ItemClass = "change-password"
        });

        if (_customerSettings.AllowCustomersToUploadAvatars)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.Standard.CUSTOMER_AVATAR,
                Title = await _localizationService.GetResourceAsync("Account.Avatar"),
                Tab = (int)CustomerNavigationEnum.Avatar,
                ItemClass = "customer-avatar"
            });
        }

        //COMMERCE NAVIGATION ITEMS REMOVED - Phase C
        //Removed: ForumSubscriptions, ProductReviews, VendorInfo, GdprTools (commerce features)

        if (_customerSettings.AllowCustomersToCheckGiftCardBalance)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = NopRouteNames.General.CHECK_GIFT_CARD_BALANCE,
                Title = await _localizationService.GetResourceAsync("CheckGiftCardBalance"),
                Tab = (int)CustomerNavigationEnum.CheckGiftCardBalance,
                ItemClass = "customer-check-gift-card-balance"
            });
        }

        model.SelectedTab = selectedTabId;

        return model;
    }

    /// <summary>
    /// Prepare the customer address list model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer address list model
    /// </returns>
    public virtual async Task<CustomerAddressListModel> PrepareCustomerAddressListModelAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var addresses = await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
            //enabled for the current store
            .WhereAwait(async a => a.CountryId == null || await _storeMappingService.AuthorizeAsync(await _countryService.GetCountryByAddressAsync(a)))
            .ToListAsync();

        var model = new CustomerAddressListModel();
        foreach (var address in addresses)
        {
            var addressModel = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));
            model.Addresses.Add(addressModel);
        }
        return model;
    }

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareCustomerDownloadableProductsModelAsync (downloadable products - commerce feature)
    //Removed: PrepareUserAgreementModelAsync (user agreement - commerce feature)

    /// <summary>
    /// Prepare the change password model
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the change password model
    /// </returns>
    public virtual async Task<ChangePasswordModel> PrepareChangePasswordModelAsync(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer);

        return new ChangePasswordModel()
        {
            PasswordExpired = await _customerService.IsPasswordExpiredAsync(customer),
            PasswordMustBeChanged = customer.MustChangePassword
        };
    }

    /// <summary>
    /// Prepare the customer avatar model
    /// </summary>
    /// <param name="model">Customer avatar model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer avatar model
    /// </returns>
    public virtual async Task<CustomerAvatarModel> PrepareCustomerAvatarModelAsync(CustomerAvatarModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.AvatarUrl = await _pictureService.GetPictureUrlAsync(
            await _genericAttributeService.GetAttributeAsync<int>(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.AvatarPictureIdAttribute),
            _mediaSettings.AvatarPictureSize,
            false);

        return model;
    }

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareGdprToolsModelAsync (GDPR tools - commerce-driven)
    //Removed: PrepareCheckGiftCardBalanceModelAsync (gift card balance - commerce feature)

    /// <summary>
    /// Prepare the multi-factor authentication model
    /// </summary>
    /// <param name="model">Multi-factor authentication model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multi-factor authentication model
    /// </returns>
    public virtual async Task<MultiFactorAuthenticationModel> PrepareMultiFactorAuthenticationModelAsync(MultiFactorAuthenticationModel model)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        model.IsEnabled = !string.IsNullOrEmpty(
            await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute));

        var store = await _storeContext.GetCurrentStoreAsync();

        return model;
    }

    /// <summary>
    /// Prepare the multi-factor authentication provider model
    /// </summary>
    /// <param name="providerModel">Multi-factor authentication provider model</param>
    /// <param name="sysName">Multi-factor authentication provider system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multi-factor authentication model
    /// </returns>
    public virtual async Task<MultiFactorAuthenticationProviderModel> PrepareMultiFactorAuthenticationProviderModelAsync(MultiFactorAuthenticationProviderModel providerModel, string sysName, bool isLogin = false)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var selectedProvider = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
        var store = await _storeContext.GetCurrentStoreAsync();

        return providerModel;
    }

    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: PrepareCustomCustomerAttributesAsync (customer attributes feature)

    #endregion
}