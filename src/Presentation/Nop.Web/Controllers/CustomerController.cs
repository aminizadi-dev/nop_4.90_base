using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain;
//COMMERCE DOMAIN REMOVED - Compile Fix
//Removed: using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
//Removed: using Nop.Core.Domain.Forums;
//Removed: using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
//Removed: using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Http.Extensions;
using Nop.Services.Authentication;
//COMMERCE SERVICES REMOVED - Compile Fix
//Removed: using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
//Removed: using Nop.Services.ExportImport;
//Removed: using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
//Removed: using Nop.Services.Orders;
using Nop.Services.Security;
//Removed: using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class CustomerController : BasePublicController
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly DateTimeSettings _dateTimeSettings;
    protected readonly HtmlEncoder _htmlEncoder;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IAuthenticationService _authenticationService;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerModelFactory _customerModelFactory;
    protected readonly ICustomerRegistrationService _customerRegistrationService;
    protected readonly ICustomerService _customerService;
    protected readonly IDownloadService _downloadService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly INotificationService _notificationService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IWorkContext _workContext;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly MediaSettings _mediaSettings;
    protected readonly MultiFactorAuthenticationSettings _multiFactorAuthenticationSettings;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public CustomerController(AddressSettings addressSettings,
        CaptchaSettings captchaSettings,
        CustomerSettings customerSettings,
        DateTimeSettings dateTimeSettings,
        //COMMERCE SETTINGS REMOVED - Compile Fix
        //Removed: ForumSettings forumSettings,
        //Removed: GdprSettings gdprSettings,
        HtmlEncoder htmlEncoder,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IAuthenticationService authenticationService,
        ICountryService countryService,
        ICustomerActivityService customerActivityService,
        ICustomerModelFactory customerModelFactory,
        ICustomerRegistrationService customerRegistrationService,
        ICustomerService customerService,
        IDownloadService downloadService,
        IEventPublisher eventPublisher,
        //Removed: IGdprService gdprService,
        IGenericAttributeService genericAttributeService,
        //Removed: IGiftCardService giftCardService,
        ILocalizationService localizationService,
        ILogger logger,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        INotificationService notificationService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IWorkContext workContext,
        LocalizationSettings localizationSettings,
        MediaSettings mediaSettings,
        MultiFactorAuthenticationSettings multiFactorAuthenticationSettings
        )
    {
        _addressSettings = addressSettings;
        _captchaSettings = captchaSettings;
        _customerSettings = customerSettings;
        _dateTimeSettings = dateTimeSettings;
        _htmlEncoder = htmlEncoder;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _authenticationService = authenticationService;
        _countryService = countryService;
        _customerActivityService = customerActivityService;
        _customerModelFactory = customerModelFactory;
        _customerRegistrationService = customerRegistrationService;
        _customerService = customerService;
        _downloadService = downloadService;
        _eventPublisher = eventPublisher;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _logger = logger;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _workContext = workContext;
        _localizationSettings = localizationSettings;
        _mediaSettings = mediaSettings;
        _multiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
    }

    #endregion

    #region Utilities

    #endregion

    #region Methods

    #region Login / logout

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> Login(bool? checkoutAsGuest)
    {
        var model = await _customerModelFactory.PrepareLoginModelAsync(checkoutAsGuest);
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (await _customerService.IsRegisteredAsync(customer))
        {
            var fullName = await _customerService.GetCustomerFullNameAsync(customer);
            var message = await _localizationService.GetResourceAsync("Account.Login.AlreadyLogin");
            _notificationService.SuccessNotification(string.Format(message, _htmlEncoder.Encode(fullName)));
        }

        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> Login(LoginModel model, string returnUrl, bool captchaValid)
    {
        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        if (ModelState.IsValid)
        {
            var customerUserName = model.Username;
            var customerEmail = model.Email;
            var userNameOrEmail = _customerSettings.UsernamesEnabled ? customerUserName : customerEmail;

            var loginResult = await _customerRegistrationService.ValidateCustomerAsync(userNameOrEmail, model.Password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                {
                    var customer = _customerSettings.UsernamesEnabled
                        ? await _customerService.GetCustomerByUsernameAsync(customerUserName)
                        : await _customerService.GetCustomerByEmailAsync(customerEmail);

                    return await _customerRegistrationService.SignInCustomerAsync(customer, returnUrl, model.RememberMe);
                }
                case CustomerLoginResults.MultiFactorAuthenticationRequired:
                {
                    var customerMultiFactorAuthenticationInfo = new CustomerMultiFactorAuthenticationInfo
                    {
                        UserName = userNameOrEmail,
                        RememberMe = model.RememberMe,
                        ReturnUrl = returnUrl
                    };
                    await HttpContext.Session.SetAsync(
                        NopCustomerDefaults.CustomerMultiFactorAuthenticationInfo,
                        customerMultiFactorAuthenticationInfo);
                    return RedirectToRoute(NopRouteNames.Standard.MULTIFACTOR_VERIFICATION);
                }
                case CustomerLoginResults.CustomerNotExist:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.CustomerNotExist"));
                    break;
                case CustomerLoginResults.Deleted:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.Deleted"));
                    break;
                case CustomerLoginResults.NotActive:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotActive"));
                    break;
                case CustomerLoginResults.NotRegistered:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotRegistered"));
                    break;
                case CustomerLoginResults.LockedOut:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.LockedOut"));
                    break;
                case CustomerLoginResults.WrongPassword:
                default:
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials"));
                    break;
            }

            if (loginResult == CustomerLoginResults.WrongPassword && _customerSettings.NotifyFailedLoginAttempt)
            {
                var customer = _customerSettings.UsernamesEnabled
                        ? await _customerService.GetCustomerByUsernameAsync(customerUserName)
                        : await _customerService.GetCustomerByEmailAsync(customerEmail);
            }

            await _customerActivityService.InsertActivityAsync("PublicStore.FailedLogin",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Login.Fail"), _customerSettings.UsernamesEnabled ? customerUserName : customerEmail));
        }

        //If we got this far, something failed, redisplay form
        model = await _customerModelFactory.PrepareLoginModelAsync(model.CheckoutAsGuest);
        return View(model);
    }

    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> Logout()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (_workContext.OriginalCustomerIfImpersonated != null)
        {
            //activity log
            await _customerActivityService.InsertActivityAsync(_workContext.OriginalCustomerIfImpersonated, "Impersonation.Finished",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.Impersonation.Finished.StoreOwner"),
                    customer.Email, customer.Id),
                customer);

            await _customerActivityService.InsertActivityAsync("Impersonation.Finished",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.Impersonation.Finished.Customer"),
                    _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id),
                _workContext.OriginalCustomerIfImpersonated);

            //logout impersonated customer
            await _genericAttributeService
                .SaveAttributeAsync<int?>(_workContext.OriginalCustomerIfImpersonated, NopCustomerDefaults.ImpersonatedCustomerIdAttribute, null);

            //redirect back to customer details page (admin area)
            return RedirectToAction("Edit", "Customer", new { id = customer.Id, area = AreaNames.ADMIN });
        }

        //activity log
        await _customerActivityService.InsertActivityAsync(customer, "PublicStore.Logout",
            await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Logout"), customer);

        //standard logout 
        await _authenticationService.SignOutAsync();

        //raise logged out event       
        await _eventPublisher.PublishAsync(new CustomerLoggedOutEvent(customer));

        return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
    }

    #endregion

    #region Password recovery

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> PasswordRecovery()
    {
        var model = new PasswordRecoveryModel();
        model = await _customerModelFactory.PreparePasswordRecoveryModelAsync(model);

        return View(model);
    }

    [ValidateCaptcha]
    [HttpPost, ActionName("PasswordRecovery")]
    [FormValueRequired("send-email")]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> PasswordRecoverySend(PasswordRecoveryModel model, bool captchaValid)
    {
        // validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        if (ModelState.IsValid)
        {
            var customer = await _customerService.GetCustomerByEmailAsync(model.Email);
            if (customer != null && customer.Active && !customer.Deleted)
            {
                //save token and current date
                var passwordRecoveryToken = Guid.NewGuid();
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute,
                    passwordRecoveryToken.ToString());
                DateTime? generatedDateTime = DateTime.UtcNow;
                await _genericAttributeService.SaveAttributeAsync(customer,
                    NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                ////send email
                //await _workflowMessageService.SendCustomerPasswordRecoveryMessageAsync(customer,
                //    (await _workContext.GetWorkingLanguageAsync()).Id);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.PasswordRecovery.EmailHasBeenSent"));
            }
            else
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Account.PasswordRecovery.EmailNotFound"));
            }
        }

        model = await _customerModelFactory.PreparePasswordRecoveryModelAsync(model);

        return View(model);
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> PasswordRecoveryConfirm(string token, string email, Guid guid)
    {
        //For backward compatibility with previous versions where email was used as a parameter in the URL
        var customer = await _customerService.GetCustomerByEmailAsync(email)
                       ?? await _customerService.GetCustomerByGuidAsync(guid);

        if (customer == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new PasswordRecoveryConfirmModel { ReturnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE) };
        if (string.IsNullOrEmpty(await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute)))
        {
            model.DisablePasswordChanging = true;
            model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.PasswordAlreadyHasBeenChanged");
            return View(model);
        }

        //validate token
        if (!await _customerService.IsPasswordRecoveryTokenValidAsync(customer, token))
        {
            model.DisablePasswordChanging = true;
            model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.WrongToken");
            return View(model);
        }

        //validate token expiration date
        if (await _customerService.IsPasswordRecoveryLinkExpiredAsync(customer))
        {
            model.DisablePasswordChanging = true;
            model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.LinkExpired");
            return View(model);
        }

        return View(model);
    }

    [HttpPost, ActionName("PasswordRecoveryConfirm")]
    [FormValueRequired("set-password")]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    //available even when a store is closed
    [CheckAccessClosedStore(ignore: true)]
    public virtual async Task<IActionResult> PasswordRecoveryConfirmPOST(string token, string email, Guid guid, PasswordRecoveryConfirmModel model)
    {
        //For backward compatibility with previous versions where email was used as a parameter in the URL
        var customer = await _customerService.GetCustomerByEmailAsync(email)
                       ?? await _customerService.GetCustomerByGuidAsync(guid);

        if (customer == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        model.ReturnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

        //validate token
        if (!await _customerService.IsPasswordRecoveryTokenValidAsync(customer, token))
        {
            model.DisablePasswordChanging = true;
            model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.WrongToken");
            return View(model);
        }

        //validate token expiration date
        if (await _customerService.IsPasswordRecoveryLinkExpiredAsync(customer))
        {
            model.DisablePasswordChanging = true;
            model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.LinkExpired");
            return View(model);
        }

        if (!ModelState.IsValid)
            return View(model);

        var response = await _customerRegistrationService
            .ChangePasswordAsync(new ChangePasswordRequest(customer.Email, false, _customerSettings.DefaultPasswordFormat, model.NewPassword));
        if (!response.Success)
        {
            model.Result = string.Join(';', response.Errors);
            return View(model);
        }

        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute, "");

        await _customerActivityService.InsertActivityAsync(customer, "PublicStore.PasswordChanged", await
            _localizationService.GetResourceAsync("ActivityLog.PublicStore.PasswordChanged"));

        //authenticate customer after changing password
        await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

        model.DisablePasswordChanging = true;
        model.Result = await _localizationService.GetResourceAsync("Account.PasswordRecovery.PasswordHasBeenChanged");
        return View(model);
    }

    #endregion     

    #region Register

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> Register(string returnUrl)
    {
        //check whether registration is allowed
        if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
            return RedirectToRoute(NopRouteNames.Standard.REGISTER_RESULT, new { resultId = (int)UserRegistrationType.Disabled, returnUrl });

        var model = new RegisterModel();
        model = await _customerModelFactory.PrepareRegisterModelAsync(model, false, setDefaultValues: true);

        return View(model);
    }

    [HttpPost]
    [ValidateCaptcha]
    [ValidateHoneypot]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> Register(RegisterModel model, string returnUrl, bool captchaValid, IFormCollection form)
    {
        //check whether registration is allowed
        if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
            return RedirectToRoute(NopRouteNames.Standard.REGISTER_RESULT, new { resultId = (int)UserRegistrationType.Disabled, returnUrl });

        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var language = await _workContext.GetWorkingLanguageAsync();

        if (await _customerService.IsRegisteredAsync(customer))
        {
            //Already registered customer. 
            await _authenticationService.SignOutAsync();

            //raise logged out event       
            await _eventPublisher.PublishAsync(new CustomerLoggedOutEvent(customer));

            customer = await _customerService.InsertGuestCustomerAsync();

            //Save a new record
            await _workContext.SetCurrentCustomerAsync(customer);
        }

        customer.RegisteredInStoreId = store.Id;

        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        if (ModelState.IsValid)
        {
            var customerUserName = model.Username;
            var customerEmail = model.Email;

            var isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
            var registrationRequest = new CustomerRegistrationRequest(customer,
                customerEmail,
                _customerSettings.UsernamesEnabled ? customerUserName : customerEmail,
                model.Password,
                _customerSettings.DefaultPasswordFormat,
                store.Id,
                isApproved);
            var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
            if (registrationResult.Success)
            {
                //properties
                if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    customer.TimeZoneId = model.TimeZoneId;

                //form fields
                if (_customerSettings.GenderEnabled)
                    customer.Gender = model.Gender;
                if (_customerSettings.FirstNameEnabled)
                    customer.FirstName = model.FirstName;
                if (_customerSettings.LastNameEnabled)
                    customer.LastName = model.LastName;
                if (_customerSettings.DateOfBirthEnabled)
                    customer.DateOfBirth = model.ParseDateOfBirth();
                if (_customerSettings.CompanyEnabled)
                    customer.Company = model.Company;
                if (_customerSettings.StreetAddressEnabled)
                    customer.StreetAddress = model.StreetAddress;
                if (_customerSettings.StreetAddress2Enabled)
                    customer.StreetAddress2 = model.StreetAddress2;
                if (_customerSettings.ZipPostalCodeEnabled)
                    customer.ZipPostalCode = model.ZipPostalCode;
                if (_customerSettings.CityEnabled)
                    customer.City = model.City;
                if (_customerSettings.CountyEnabled)
                    customer.County = model.County;
                if (_customerSettings.CountryEnabled)
                    customer.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    customer.StateProvinceId = model.StateProvinceId;
                if (_customerSettings.PhoneEnabled)
                    customer.Phone = model.Phone;
                if (_customerSettings.FaxEnabled)
                    customer.Fax = model.Fax;

                //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
                //Removed: customer.CustomCustomerAttributesXML = customerAttributesXml;
                await _customerService.UpdateCustomerAsync(customer);

                //newsletter subscriptions
                if (_customerSettings.NewsletterEnabled)
                {
                    var anyNewSubscriptions = false;
                    var isNewsletterActive = _customerSettings.UserRegistrationType != UserRegistrationType.EmailValidation;
                    var activeSubscriptions = model.NewsLetterSubscriptions.Where(subscriptionModel => subscriptionModel.IsActive);
                    var currentSubscriptions = await _newsLetterSubscriptionService
                        .GetNewsLetterSubscriptionsByEmailAsync(customerEmail, storeId: store.Id);
                    if (currentSubscriptions.Any())
                    {
                        var subscriptionGuid = currentSubscriptions.FirstOrDefault().NewsLetterSubscriptionGuid;
                        foreach (var activeSubscription in activeSubscriptions)
                        {
                            var existingSubscription = currentSubscriptions
                                ?.FirstOrDefault(subscription => subscription.TypeId == activeSubscription.TypeId);
                            if (existingSubscription is not null)
                            {
                                if (!existingSubscription.Active && isNewsletterActive)
                                {
                                    existingSubscription.Active = true;
                                    existingSubscription.LanguageId = customer.LanguageId ?? language.Id;
                                    await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(existingSubscription);
                                }
                            }
                            else
                            {
                                await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new()
                                {
                                    NewsLetterSubscriptionGuid = subscriptionGuid,
                                    Email = customer.Email,
                                    Active = isNewsletterActive,
                                    TypeId = activeSubscription.TypeId,
                                    StoreId = store.Id,
                                    LanguageId = customer.LanguageId ?? language.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                                anyNewSubscriptions = true;
                            }
                        }
                    }
                    else
                    {
                        var subscriptionGuid = Guid.NewGuid();
                        foreach (var activeSubscription in activeSubscriptions)
                        {
                            await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new()
                            {
                                NewsLetterSubscriptionGuid = subscriptionGuid,
                                Email = customer.Email,
                                Active = isNewsletterActive,
                                TypeId = activeSubscription.TypeId,
                                StoreId = store.Id,
                                LanguageId = customer.LanguageId ?? language.Id,
                                CreatedOnUtc = DateTime.UtcNow
                            });
                            anyNewSubscriptions = true;
                        }
                    }
                }

                //insert default address (if possible)
                var defaultAddress = new Address
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Company = customer.Company,
                    CountryId = customer.CountryId > 0
                        ? (int?)customer.CountryId
                        : null,
                    StateProvinceId = customer.StateProvinceId > 0
                        ? (int?)customer.StateProvinceId
                        : null,
                    County = customer.County,
                    City = customer.City,
                    Address1 = customer.StreetAddress,
                    Address2 = customer.StreetAddress2,
                    ZipPostalCode = customer.ZipPostalCode,
                    PhoneNumber = customer.Phone,
                    FaxNumber = customer.Fax,
                    CreatedOnUtc = customer.CreatedOnUtc
                };
                if (await _addressService.IsAddressValidAsync(defaultAddress))
                {
                    //some validation
                    if (defaultAddress.CountryId == 0)
                        defaultAddress.CountryId = null;
                    if (defaultAddress.StateProvinceId == 0)
                        defaultAddress.StateProvinceId = null;
                    //set default address
                    //customer.Addresses.Add(defaultAddress);

                    await _addressService.InsertAddressAsync(defaultAddress);

                    await _customerService.InsertCustomerAddressAsync(customer, defaultAddress);

                    customer.BillingAddressId = defaultAddress.Id;
                    customer.ShippingAddressId = defaultAddress.Id;

                    await _customerService.UpdateCustomerAsync(customer);
                }

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));

                switch (_customerSettings.UserRegistrationType)
                {
                    case UserRegistrationType.EmailValidation:
                        //email validation message
                        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());

                        //result
                        return RedirectToRoute(NopRouteNames.Standard.REGISTER_RESULT, new { resultId = (int)UserRegistrationType.EmailValidation, returnUrl });

                    case UserRegistrationType.AdminApproval:
                        return RedirectToRoute(NopRouteNames.Standard.REGISTER_RESULT, new { resultId = (int)UserRegistrationType.AdminApproval, returnUrl });

                    case UserRegistrationType.Standard:

                        //raise event       
                        await _eventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

                        returnUrl = Url.RouteUrl(NopRouteNames.Standard.REGISTER_RESULT, new { resultId = (int)UserRegistrationType.Standard, returnUrl });
                        return await _customerRegistrationService.SignInCustomerAsync(customer, returnUrl, true);

                    default:
                        return RedirectToRoute(NopRouteNames.General.HOMEPAGE);
                }
            }

            //errors
            foreach (var error in registrationResult.Errors)
                ModelState.AddModelError("", error);
        }

        //If we got this far, something failed, redisplay form
        model = await _customerModelFactory.PrepareRegisterModelAsync(model, true);

        return View(model);
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> RegisterResult(int resultId, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

        var model = await _customerModelFactory.PrepareRegisterResultModelAsync(resultId, returnUrl);
        return View(model);
    }

    [HttpPost]
    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> CheckUsernameAvailability(string username)
    {
        var usernameAvailable = false;
        var statusText = await _localizationService.GetResourceAsync("Account.CheckUsernameAvailability.NotAvailable");

        if (!UsernamePropertyValidator<string, string>.IsValid(username, _customerSettings))
        {
            statusText = await _localizationService.GetResourceAsync("Account.Fields.Username.NotValid");
        }
        else if (_customerSettings.UsernamesEnabled && !string.IsNullOrWhiteSpace(username))
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            if (currentCustomer != null &&
                currentCustomer.Username != null &&
                currentCustomer.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
            {
                statusText = await _localizationService.GetResourceAsync("Account.CheckUsernameAvailability.CurrentUsername");
            }
            else
            {
                var customer = await _customerService.GetCustomerByUsernameAsync(username);
                if (customer == null)
                {
                    statusText = await _localizationService.GetResourceAsync("Account.CheckUsernameAvailability.Available");
                    usernameAvailable = true;
                }
            }
        }

        return Json(new { Available = usernameAvailable, Text = statusText });
    }

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> AccountActivation(string token, string email, Guid guid)
    {
        //For backward compatibility with previous versions where email was used as a parameter in the URL
        var customer = await _customerService.GetCustomerByEmailAsync(email)
                       ?? await _customerService.GetCustomerByGuidAsync(guid);

        if (customer == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new AccountActivationModel { ReturnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE) };
        var cToken = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.AccountActivationTokenAttribute);
        if (string.IsNullOrEmpty(cToken))
        {
            model.Result = await _localizationService.GetResourceAsync("Account.AccountActivation.AlreadyActivated");
            return View(model);
        }

        if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        //activate user account
        customer.Active = true;
        await _customerService.UpdateCustomerAsync(customer);
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AccountActivationTokenAttribute, "");

        //raise event       
        await _eventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

        //authenticate customer after activation
        await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

        //activating newsletter subscriptions
        var store = await _storeContext.GetCurrentStoreAsync();
        var subscriptions = await _newsLetterSubscriptionService
            .GetNewsLetterSubscriptionsByEmailAsync(customer.Email, storeId: store.Id, isActive: false);
        foreach (var subscription in subscriptions)
        {
            subscription.Active = true;
            await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(subscription);
        }

        model.Result = await _localizationService.GetResourceAsync("Account.AccountActivation.Activated");
        return View(model);
    }

    #endregion

    #region My account / Info

    public virtual async Task<IActionResult> Info()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var model = new CustomerInfoModel();
        model = await _customerModelFactory.PrepareCustomerInfoModelAsync(model, customer, false);

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> Info(CustomerInfoModel model, IFormCollection form)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var language = await _workContext.GetWorkingLanguageAsync();

        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: GDPR log preparation
        //Removed: Custom customer attributes parsing
        //Removed: GDPR consents validation
        var customerAttributesXml = ""; // Empty since attributes feature is removed

        try
        {
            if (ModelState.IsValid)
            {
                //username 
                if (_customerSettings.UsernamesEnabled && _customerSettings.AllowUsersToChangeUsernames)
                {
                    var userName = model.Username;
                    if (!customer.Username.Equals(userName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change username
                        await _customerRegistrationService.SetUsernameAsync(customer, userName);

                        //re-authenticate
                        //do not authenticate users in impersonation mode
                        if (_workContext.OriginalCustomerIfImpersonated == null)
                            await _authenticationService.SignInAsync(customer, true);
                    }
                }
                //email
                var email = model.Email;
                if (!customer.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                {
                    //change email
                    var requireValidation = _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                    await _customerRegistrationService.SetEmailAsync(customer, email, requireValidation);

                    //do not authenticate users in impersonation mode
                    if (_workContext.OriginalCustomerIfImpersonated == null)
                    {
                        //re-authenticate (if usernames are disabled)
                        if (!_customerSettings.UsernamesEnabled && !requireValidation)
                            await _authenticationService.SignInAsync(customer, true);
                    }
                }

                //properties
                if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    customer.TimeZoneId = model.TimeZoneId;
                //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
                //Removed: VAT number handling (tax feature)

                //form fields
                if (_customerSettings.GenderEnabled)
                    customer.Gender = model.Gender;
                if (_customerSettings.FirstNameEnabled)
                    customer.FirstName = model.FirstName;
                if (_customerSettings.LastNameEnabled)
                    customer.LastName = model.LastName;
                if (_customerSettings.DateOfBirthEnabled)
                    customer.DateOfBirth = model.ParseDateOfBirth();
                if (_customerSettings.CompanyEnabled)
                    customer.Company = model.Company;
                if (_customerSettings.StreetAddressEnabled)
                    customer.StreetAddress = model.StreetAddress;
                if (_customerSettings.StreetAddress2Enabled)
                    customer.StreetAddress2 = model.StreetAddress2;
                if (_customerSettings.ZipPostalCodeEnabled)
                    customer.ZipPostalCode = model.ZipPostalCode;
                if (_customerSettings.CityEnabled)
                    customer.City = model.City;
                if (_customerSettings.CountyEnabled)
                    customer.County = model.County;
                if (_customerSettings.CountryEnabled)
                    customer.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    customer.StateProvinceId = model.StateProvinceId;
                if (_customerSettings.PhoneEnabled)
                    customer.Phone = model.Phone;
                if (_customerSettings.FaxEnabled)
                    customer.Fax = model.Fax;

                customer.CustomCustomerAttributesXML = customerAttributesXml;
                await _customerService.UpdateCustomerAsync(customer);

                //newsletter subscriptions
                if (_customerSettings.NewsletterEnabled)
                {
                    var currentSubscriptions = await _newsLetterSubscriptionService
                        .GetNewsLetterSubscriptionsByEmailAsync(customer.Email, storeId: store.Id);
                    if (currentSubscriptions.Any())
                    {
                        var subscriptionGuid = currentSubscriptions.FirstOrDefault().NewsLetterSubscriptionGuid;
                        foreach (var newsLetterSubscriptionModel in model.NewsLetterSubscriptions)
                        {
                            var existingSubscription = currentSubscriptions
                                .FirstOrDefault(subscription => subscription.TypeId == newsLetterSubscriptionModel.TypeId);
                            if (existingSubscription is not null && existingSubscription.Active != newsLetterSubscriptionModel.IsActive)
                            {
                                existingSubscription.Active = newsLetterSubscriptionModel.IsActive;
                                await _newsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(existingSubscription);
                            }

                            if (existingSubscription is null && newsLetterSubscriptionModel.IsActive)
                            {
                                await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new()
                                {
                                    NewsLetterSubscriptionGuid = subscriptionGuid,
                                    Email = customer.Email,
                                    Active = true,
                                    TypeId = newsLetterSubscriptionModel.TypeId,
                                    StoreId = store.Id,
                                    LanguageId = customer.LanguageId ?? language.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }
                    else
                    {
                        var subscriptionGuid = Guid.NewGuid();
                        var activeSubscriptions = model.NewsLetterSubscriptions.Where(subscriptionModel => subscriptionModel.IsActive);
                        foreach (var activeSubscription in activeSubscriptions)
                        {
                            await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new()
                            {
                                NewsLetterSubscriptionGuid = subscriptionGuid,
                                Email = customer.Email,
                                Active = true,
                                StoreId = store.Id,
                                TypeId = activeSubscription.TypeId,
                                LanguageId = customer.LanguageId ?? language.Id,
                                CreatedOnUtc = DateTime.UtcNow
                            });
                        }
                    }
                }

                //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
                //Removed: Forum signature handling
                //Removed: GDPR logging

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.CustomerInfo.Updated"));

                return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);
            }
        }
        catch (Exception exc)
        {
            ModelState.AddModelError("", exc.Message);
        }

        //If we got this far, something failed, redisplay form
        model = await _customerModelFactory.PrepareCustomerInfoModelAsync(model, customer, true);

        return View(model);
    }

    [HttpPost]
    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: RemoveExternalAssociation (external authentication feature)

    //available even when navigation is not allowed
    [CheckAccessPublicStore(ignore: true)]
    public virtual async Task<IActionResult> EmailRevalidation(string token, string email, Guid guid)
    {
        //For backward compatibility with previous versions where email was used as a parameter in the URL
        var customer = await _customerService.GetCustomerByEmailAsync(email)
                       ?? await _customerService.GetCustomerByGuidAsync(guid);

        if (customer == null)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        var model = new EmailRevalidationModel { ReturnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE) };
        var cToken = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute);
        if (string.IsNullOrEmpty(cToken))
        {
            model.Result = await _localizationService.GetResourceAsync("Account.EmailRevalidation.AlreadyChanged");
            return View(model);
        }

        if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (string.IsNullOrEmpty(customer.EmailToRevalidate))
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        if (_customerSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
            return RedirectToRoute(NopRouteNames.General.HOMEPAGE);

        //change email
        try
        {
            await _customerRegistrationService.SetEmailAsync(customer, customer.EmailToRevalidate, false);
        }
        catch (Exception exc)
        {
            model.Result = await _localizationService.GetResourceAsync(exc.Message);
            return View(model);
        }

        customer.EmailToRevalidate = null;
        await _customerService.UpdateCustomerAsync(customer);
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute, "");

        //authenticate customer after changing email
        await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

        model.Result = await _localizationService.GetResourceAsync("Account.EmailRevalidation.Changed");
        return View(model);
    }

    #endregion

    #region My account / Addresses

    public virtual async Task<IActionResult> Addresses()
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var model = await _customerModelFactory.PrepareCustomerAddressListModelAsync();

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> AddressDelete(int addressId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        //find address (ensure that it belongs to the current customer)
        var address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId);
        if (address != null)
        {
            await _customerService.RemoveCustomerAddressAsync(customer, address);
            await _customerService.UpdateCustomerAsync(customer);
            //now delete the address record
            await _addressService.DeleteAddressAsync(address);
        }

        //redirect to the address list page
        return Json(new
        {
            redirect = Url.RouteUrl(NopRouteNames.General.CUSTOMER_ADDRESSES),
        });
    }

    public virtual async Task<IActionResult> AddressAdd()
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        var model = new CustomerAddressEditModel();
        await _addressModelFactory.PrepareAddressModelAsync(model.Address,
            address: null,
            excludeProperties: false,
            addressSettings: _addressSettings,
            loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> AddressAdd(CustomerAddressEditModel model, IFormCollection form)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        if (ModelState.IsValid)
        {
            var address = model.Address.ToEntity();
            address.CreatedOnUtc = DateTime.UtcNow;
            //some validation
            if (address.CountryId == 0)
                address.CountryId = null;
            if (address.StateProvinceId == 0)
                address.StateProvinceId = null;


            await _addressService.InsertAddressAsync(address);

            await _customerService.InsertCustomerAddressAsync(customer, address);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.CustomerAddresses.Added"));

            return RedirectToRoute(NopRouteNames.General.CUSTOMER_ADDRESSES);
        }

        //If we got this far, something failed, redisplay form
        await _addressModelFactory.PrepareAddressModelAsync(model.Address,
            address: null,
            excludeProperties: true,
            addressSettings: _addressSettings,
            loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id),
            overrideAttributesXml: ""); // Attributes feature removed

        return View(model);
    }

    public virtual async Task<IActionResult> AddressEdit(int addressId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        //find address (ensure that it belongs to the current customer)
        var address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId);
        if (address == null)
            //address is not found
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_ADDRESSES);

        var model = new CustomerAddressEditModel();
        await _addressModelFactory.PrepareAddressModelAsync(model.Address,
            address: address,
            excludeProperties: false,
            addressSettings: _addressSettings,
            loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> AddressEdit(CustomerAddressEditModel model, IFormCollection form)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        //find address (ensure that it belongs to the current customer)
        var address = await _customerService.GetCustomerAddressAsync(customer.Id, model.Address.Id);
        if (address == null)
            //address is not found
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_ADDRESSES);

        //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
        //Removed: Custom address attributes parsing
        var customAttributes = ""; // Empty since attributes feature is removed

        if (ModelState.IsValid)
        {
            address = model.Address.ToEntity(address);
            //Removed: address.CustomAttributes = customAttributes;
            await _addressService.UpdateAddressAsync(address);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.CustomerAddresses.Updated"));

            return RedirectToRoute(NopRouteNames.General.CUSTOMER_ADDRESSES);
        }

        //If we got this far, something failed, redisplay form
        await _addressModelFactory.PrepareAddressModelAsync(model.Address,
            address: address,
            excludeProperties: true,
            addressSettings: _addressSettings,
            loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id),
            overrideAttributesXml: ""); // Attributes feature removed

        return View(model);
    }

    #endregion

    #region My account / Change password

    public virtual async Task<IActionResult> ChangePassword()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        var model = await _customerModelFactory.PrepareChangePasswordModelAsync(customer);

        return View(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> ChangePassword(ChangePasswordModel model, string returnUrl)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        if (ModelState.IsValid)
        {
            var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
            var changePasswordResult = await _customerRegistrationService.ChangePasswordAsync(changePasswordRequest);
            if (changePasswordResult.Success)
            {
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Account.ChangePassword.Success"));

                await _customerActivityService.InsertActivityAsync(customer, "PublicStore.PasswordChanged", await
                    _localizationService.GetResourceAsync("ActivityLog.PublicStore.PasswordChanged"));

                //authenticate customer after changing password
                await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

                if (string.IsNullOrEmpty(returnUrl))
                    return View(model);

                //prevent open redirection attack
                if (!Url.IsLocalUrl(returnUrl))
                    returnUrl = Url.RouteUrl(NopRouteNames.General.HOMEPAGE);

                return new RedirectResult(returnUrl);
            }

            //errors
            foreach (var error in changePasswordResult.Errors)
                ModelState.AddModelError("", error);
        }

        //If we got this far, something failed, redisplay form
        model = await _customerModelFactory.PrepareChangePasswordModelAsync(customer);

        return View(model);
    }

    #endregion

    #region My account / Avatar

    public virtual async Task<IActionResult> Avatar()
    {
        if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            return Challenge();

        if (!_customerSettings.AllowCustomersToUploadAvatars)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var model = new CustomerAvatarModel();
        model = await _customerModelFactory.PrepareCustomerAvatarModelAsync(model);

        return View(model);
    }

    [HttpPost, ActionName("Avatar")]
    [FormValueRequired("upload-avatar")]
    public virtual async Task<IActionResult> UploadAvatar(CustomerAvatarModel model, IFormFile uploadedFile)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        if (!_customerSettings.AllowCustomersToUploadAvatars)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var contentType = uploadedFile?.ContentType.ToLowerInvariant();

        if (contentType != null && !contentType.Equals("image/jpeg") && !contentType.Equals("image/gif"))
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Avatar.UploadRules"));

        if (ModelState.IsValid)
        {
            try
            {
                var customerAvatar = await _pictureService.GetPictureByIdAsync(await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute));
                if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
                {
                    var avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                    if (uploadedFile.Length > avatarMaxSize)
                        throw new NopException(string.Format(await _localizationService.GetResourceAsync("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                    var customerPictureBinary = await _downloadService.GetDownloadBitsAsync(uploadedFile);
                    if (customerAvatar != null)
                        customerAvatar = await _pictureService.UpdatePictureAsync(customerAvatar.Id, customerPictureBinary, contentType, null);
                    else
                        customerAvatar = await _pictureService.InsertPictureAsync(customerPictureBinary, contentType, null);
                }

                var customerAvatarId = 0;
                if (customerAvatar != null)
                    customerAvatarId = customerAvatar.Id;

                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AvatarPictureIdAttribute, customerAvatarId);

                model.AvatarUrl = await _pictureService.GetPictureUrlAsync(
                    await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                    _mediaSettings.AvatarPictureSize,
                    false);

                return View(model);
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }
        }

        //If we got this far, something failed, redisplay form
        model = await _customerModelFactory.PrepareCustomerAvatarModelAsync(model);
        return View(model);
    }

    [HttpPost, ActionName("Avatar")]
    [FormValueRequired("remove-avatar")]
    public virtual async Task<IActionResult> RemoveAvatar(CustomerAvatarModel model)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (!await _customerService.IsRegisteredAsync(customer))
            return Challenge();

        if (!_customerSettings.AllowCustomersToUploadAvatars)
            return RedirectToRoute(NopRouteNames.General.CUSTOMER_INFO);

        var customerAvatar = await _pictureService.GetPictureByIdAsync(await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute));
        if (customerAvatar != null)
            await _pictureService.DeletePictureAsync(customerAvatar);
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AvatarPictureIdAttribute, 0);

        return RedirectToRoute(NopRouteNames.Standard.CUSTOMER_AVATAR);
    }

    #endregion

    #endregion
}