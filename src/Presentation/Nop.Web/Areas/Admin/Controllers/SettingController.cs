using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Translation;
//COMMERCE DOMAIN REMOVED - Phase B
//Removed: ArtificialIntelligence, Blogs, Catalog, FilterLevels, Forums, Gdpr, News, Orders, Shipping, Tax, Vendors
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Configuration;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
//COMMERCE SERVICES REMOVED - Phase B
//Removed: Gdpr, Orders
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.WebOptimizer;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class SettingController : BaseAdminController
{
    #region Fields

    protected readonly AppSettings _appSettings;
    protected readonly IAddressService _addressService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly INopDataProvider _dataProvider;
    protected readonly IEncryptionService _encryptionService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizedEntityService _localizedEntityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INotificationService _notificationService;
    protected readonly IPictureService _pictureService;
    protected readonly ISettingModelFactory _settingModelFactory;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IWorkContext _workContext;
    private static readonly char[] _separator = [','];

    #endregion

    #region Ctor

    public SettingController(AppSettings appSettings,
        IAddressService addressService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        INopDataProvider dataProvider,
        IEncryptionService encryptionService,
        IEventPublisher eventPublisher,
        IGenericAttributeService genericAttributeService,
        //COMMERCE DEPENDENCIES REMOVED - Phase B
        //Removed: IGdprService gdprService, IOrderService orderService
        ILocalizedEntityService localizedEntityService,
        ILocalizationService localizationService,
        INopFileProvider fileProvider,
        INotificationService notificationService,
        IPictureService pictureService,
        ISettingModelFactory settingModelFactory,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreService storeService,
        IWorkContext workContext)
    {
        _appSettings = appSettings;
        _addressService = addressService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _dataProvider = dataProvider;
        _encryptionService = encryptionService;
        _eventPublisher = eventPublisher;
        _genericAttributeService = genericAttributeService;
        //COMMERCE DEPENDENCIES REMOVED - Phase B
        //Removed: _gdprService = gdprService, _orderService = orderService
        _localizedEntityService = localizedEntityService;
        _localizationService = localizationService;
        _fileProvider = fileProvider;
        _notificationService = notificationService;
        _pictureService = pictureService;
        _settingModelFactory = settingModelFactory;
        _settingService = settingService;
        _storeContext = storeContext;
        _storeService = storeService;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> ChangeStoreScopeConfiguration(int storeid, string returnUrl = "")
    {
        var store = await _storeService.GetStoreByIdAsync(storeid);
        if (store != null || storeid == 0)
        {
            await _genericAttributeService
                .SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.AdminAreaStoreScopeConfigurationAttribute, storeid);
        }

        //home page
        if (string.IsNullOrEmpty(returnUrl))
            returnUrl = Url.Action("Index", "Home", new { area = AreaNames.ADMIN });

        //prevent open redirection attack
        if (!Url.IsLocalUrl(returnUrl))
            return RedirectToAction("Index", "Home", new { area = AreaNames.ADMIN });

        return Redirect(returnUrl);
    }

    [CheckPermission(StandardPermission.System.MANAGE_APP_SETTINGS)]
    public virtual async Task<IActionResult> AppSettings()
    {
        //prepare model
        var model = await _settingModelFactory.PrepareAppSettingsModel();

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.System.MANAGE_APP_SETTINGS)]
    public virtual async Task<IActionResult> AppSettings(AppSettingsModel model)
    {
        if (ModelState.IsValid)
        {
            var configurations = new List<IConfig>
            {
                model.CacheConfigModel.ToConfig(_appSettings.Get<CacheConfig>()),
                model.HostingConfigModel.ToConfig(_appSettings.Get<HostingConfig>()),
                model.DistributedCacheConfigModel.ToConfig(_appSettings.Get<DistributedCacheConfig>()),
                model.InstallationConfigModel.ToConfig(_appSettings.Get<InstallationConfig>()),
                model.CommonConfigModel.ToConfig(_appSettings.Get<CommonConfig>()),
                model.DataConfigModel.ToConfig(_appSettings.Get<DataConfig>()),
                model.WebOptimizerConfigModel.ToConfig(_appSettings.Get<WebOptimizerConfig>())
            };

            await _eventPublisher.PublishAsync(new AppSettingsSavingEvent(configurations));

            AppSettingsHelper.SaveAppSettings(configurations, _fileProvider);

            await _customerActivityService.InsertActivityAsync("EditSettings",
                await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            var returnUrl = Url.Action("AppSettings", "Setting", new { area = AreaNames.ADMIN });
            return View("RestartApplication", returnUrl);
        }

        //prepare model
        model = await _settingModelFactory.PrepareAppSettingsModel(model);

        //if we got this far, something failed, redisplay form
        return View(model);
    }


    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Media()
    {
        //prepare model
        var model = await _settingModelFactory.PrepareMediaSettingsModelAsync();

        return View(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> Media(MediaSettingsModel model)
    {
        if (ModelState.IsValid)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>(storeScope);
            mediaSettings = model.ToSettings(mediaSettings);

            //we do not clear cache after each setting update.
            //this behavior can increase performance because cached settings will not be cleared 
            //and loaded from database after each update
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.AvatarPictureSize, model.AvatarPictureSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.ProductThumbPictureSize, model.ProductThumbPictureSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.ProductDetailsPictureSize, model.ProductDetailsPictureSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.AssociatedProductPictureSize, model.AssociatedProductPictureSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.CategoryThumbPictureSize, model.CategoryThumbPictureSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.ManufacturerThumbPictureSize, model.ManufacturerThumbPictureSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.VendorThumbPictureSize, model.VendorThumbPictureSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.CartThumbPictureSize, model.CartThumbPictureSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.OrderThumbPictureSize, model.OrderThumbPictureSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.MiniCartThumbPictureSize, model.MiniCartThumbPictureSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.MaximumImageSize, model.MaximumImageSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.MultipleThumbDirectories, model.MultipleThumbDirectories_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.DefaultImageQuality, model.DefaultImageQuality_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.ImportProductImagesUsingHash, model.ImportProductImagesUsingHash_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.DefaultPictureZoomEnabled, model.DefaultPictureZoomEnabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.AllowSvgUploads, model.AllowSvgUploads_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.ProductDefaultImageId, model.ProductDefaultImageId_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //activity log
            await _customerActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("Media");
        }

        //prepare model
        model = await _settingModelFactory.PrepareMediaSettingsModelAsync(model);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost, ActionName("Media")]
    [FormValueRequired("change-picture-storage")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ChangePictureStorage()
    {
        await _pictureService.SetIsStoreInDbAsync(!await _pictureService.IsStoreInDbAsync());

        //activity log
        await _customerActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

        return RedirectToAction("Media");
    }

    [HttpPost, ActionName("Media")]
    [FormValueRequired("change-picture-path")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> ChangeImagePath(MediaSettingsModel model)
    {
        try
        {
            if (!_fileProvider.CheckPermissions(model.PicturePath, true, true, true, true))
            {
                _notificationService.ErrorNotification(string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Media.PicturePath.NotGrantedPermission"), CurrentOSUser.FullName, model.PicturePath));

                return RedirectToAction("Media");
            }

            await _pictureService.ChangePicturesPathAsync(model.PicturePath);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            var returnUrl = Url.Action("Media", "Setting", new { area = AreaNames.ADMIN });
            return View("RestartApplication", returnUrl);
        }
        catch (Exception ex)
        {
            var exceptionMessage = ex.Message;
            var mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>();

            if (_fileProvider.EnumerateFiles(mediaSettings.PicturePath, "*.*", false).Any())
                exceptionMessage += $" {await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Media.ChangePicturePath.TryAgain")}";
            else
            {
                mediaSettings.PicturePath = model.PicturePath;
                await _settingService.SaveSettingAsync(mediaSettings, settings => settings.PicturePath);
            }

            _notificationService.ErrorNotification(exceptionMessage);
        }

        return RedirectToAction("Media");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> CustomerUser()
    {
        //prepare model
        var model = await _settingModelFactory.PrepareCustomerUserSettingsModelAsync();

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> CustomerUser(CustomerUserSettingsModel model)
    {
        if (ModelState.IsValid)
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var customerSettings = await _settingService.LoadSettingAsync<CustomerSettings>(storeScope);

            var lastUsernameValidationRule = customerSettings.UsernameValidationRule;
            var lastUsernameValidationEnabledValue = customerSettings.UsernameValidationEnabled;
            var lastUsernameValidationUseRegexValue = customerSettings.UsernameValidationUseRegex;

            //Phone number validation settings
            var lastPhoneNumberValidationRule = customerSettings.PhoneNumberValidationRule;
            var lastPhoneNumberValidationEnabledValue = customerSettings.PhoneNumberValidationEnabled;
            var lastPhoneNumberValidationUseRegexValue = customerSettings.PhoneNumberValidationUseRegex;

            var addressSettings = await _settingService.LoadSettingAsync<AddressSettings>(storeScope);
            var dateTimeSettings = await _settingService.LoadSettingAsync<DateTimeSettings>(storeScope);
            var externalAuthenticationSettings = await _settingService.LoadSettingAsync<ExternalAuthenticationSettings>(storeScope);
            var multiFactorAuthenticationSettings = await _settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>(storeScope);

            customerSettings = model.CustomerSettings.ToSettings(customerSettings);

            if (customerSettings.UsernameValidationEnabled && customerSettings.UsernameValidationUseRegex)
            {
                try
                {
                    //validate regex rule
                    var unused = Regex.IsMatch("test_user_name", customerSettings.UsernameValidationRule);
                }
                catch (ArgumentException)
                {
                    //restoring previous settings
                    customerSettings.UsernameValidationRule = lastUsernameValidationRule;
                    customerSettings.UsernameValidationEnabled = lastUsernameValidationEnabledValue;
                    customerSettings.UsernameValidationUseRegex = lastUsernameValidationUseRegexValue;

                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.CustomerSettings.RegexValidationRule.Error"));
                }
            }

            if (customerSettings.PhoneNumberValidationEnabled && customerSettings.PhoneNumberValidationUseRegex)
            {
                try
                {
                    //validate regex rule
                    var unused = Regex.IsMatch("123456789", customerSettings.PhoneNumberValidationRule);
                }
                catch (ArgumentException)
                {
                    //restoring previous settings
                    customerSettings.PhoneNumberValidationRule = lastPhoneNumberValidationRule;
                    customerSettings.PhoneNumberValidationEnabled = lastPhoneNumberValidationEnabledValue;
                    customerSettings.PhoneNumberValidationUseRegex = lastPhoneNumberValidationUseRegexValue;

                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.CustomerSettings.PhoneNumberRegexValidationRule.Error"));
                }
            }

            await _settingService.SaveSettingAsync(customerSettings);

            addressSettings = model.AddressSettings.ToSettings(addressSettings);
            await _settingService.SaveSettingAsync(addressSettings);

            dateTimeSettings.DefaultStoreTimeZoneId = model.DateTimeSettings.DefaultStoreTimeZoneId;
            dateTimeSettings.AllowCustomersToSetTimeZone = model.DateTimeSettings.AllowCustomersToSetTimeZone;
            await _settingService.SaveSettingAsync(dateTimeSettings);

            multiFactorAuthenticationSettings = model.MultiFactorAuthenticationSettings.ToSettings(multiFactorAuthenticationSettings);
            await _settingService.SaveSettingAsync(multiFactorAuthenticationSettings);

            //activity log
            await _customerActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("CustomerUser");
        }

        //prepare model
        model = await _settingModelFactory.PrepareCustomerUserSettingsModelAsync(model);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> GeneralCommon(GeneralCommonSettingsModel model)
    {
        if (ModelState.IsValid)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();

            //store information settings
            var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeScope);
            var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeScope);
            var sitemapSettings = await _settingService.LoadSettingAsync<SitemapSettings>(storeScope);

            storeInformationSettings.StoreClosed = model.StoreInformationSettings.StoreClosed;
            storeInformationSettings.DefaultStoreTheme = model.StoreInformationSettings.DefaultStoreTheme;
            storeInformationSettings.AllowCustomerToSelectTheme = model.StoreInformationSettings.AllowCustomerToSelectTheme;
            storeInformationSettings.LogoPictureId = model.StoreInformationSettings.LogoPictureId;
            //EU Cookie law
            storeInformationSettings.DisplayEuCookieLawWarning = model.StoreInformationSettings.DisplayEuCookieLawWarning;
            //social pages
            storeInformationSettings.FacebookLink = model.StoreInformationSettings.FacebookLink;
            storeInformationSettings.TwitterLink = model.StoreInformationSettings.TwitterLink;
            storeInformationSettings.YoutubeLink = model.StoreInformationSettings.YoutubeLink;
            storeInformationSettings.InstagramLink = model.StoreInformationSettings.InstagramLink;
            //contact us
            commonSettings.SubjectFieldOnContactUsForm = model.StoreInformationSettings.SubjectFieldOnContactUsForm;
            commonSettings.UseSystemEmailForContactUsForm = model.StoreInformationSettings.UseSystemEmailForContactUsForm;
            //terms of service
            commonSettings.PopupForTermsOfServiceLinks = model.StoreInformationSettings.PopupForTermsOfServiceLinks;
            //sitemap
            sitemapSettings.SitemapEnabled = model.SitemapSettings.SitemapEnabled;
            sitemapSettings.SitemapPageSize = model.SitemapSettings.SitemapPageSize;
            sitemapSettings.SitemapIncludeCategories = model.SitemapSettings.SitemapIncludeCategories;
            sitemapSettings.SitemapIncludeManufacturers = model.SitemapSettings.SitemapIncludeManufacturers;
            sitemapSettings.SitemapIncludeProducts = model.SitemapSettings.SitemapIncludeProducts;
            sitemapSettings.SitemapIncludeProductTags = model.SitemapSettings.SitemapIncludeProductTags;
            sitemapSettings.SitemapIncludeBlogPosts = model.SitemapSettings.SitemapIncludeBlogPosts;
            sitemapSettings.SitemapIncludeNews = model.SitemapSettings.SitemapIncludeNews;
            sitemapSettings.SitemapIncludeTopics = model.SitemapSettings.SitemapIncludeTopics;

            //minification
            commonSettings.EnableHtmlMinification = model.MinificationSettings.EnableHtmlMinification;
            //use response compression
            commonSettings.UseResponseCompression = model.MinificationSettings.UseResponseCompression;
            //custom header and footer HTML
            commonSettings.HeaderCustomHtml = model.CustomHtmlSettings.HeaderCustomHtml;
            commonSettings.FooterCustomHtml = model.CustomHtmlSettings.FooterCustomHtml;

            //we do not clear cache after each setting update.
            //this behavior can increase performance because cached settings will not be cleared 
            //and loaded from database after each update
            await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.StoreClosed, model.StoreInformationSettings.StoreClosed_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.DefaultStoreTheme, model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.AllowCustomerToSelectTheme, model.StoreInformationSettings.AllowCustomerToSelectTheme_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.LogoPictureId, model.StoreInformationSettings.LogoPictureId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.DisplayEuCookieLawWarning, model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.FacebookLink, model.StoreInformationSettings.FacebookLink_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.TwitterLink, model.StoreInformationSettings.TwitterLink_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.YoutubeLink, model.StoreInformationSettings.YoutubeLink_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.InstagramLink, model.StoreInformationSettings.InstagramLink_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.SubjectFieldOnContactUsForm, model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.UseSystemEmailForContactUsForm, model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.PopupForTermsOfServiceLinks, model.StoreInformationSettings.PopupForTermsOfServiceLinks_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapEnabled, model.SitemapSettings.SitemapEnabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapPageSize, model.SitemapSettings.SitemapPageSize_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeCategories, model.SitemapSettings.SitemapIncludeCategories_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeManufacturers, model.SitemapSettings.SitemapIncludeManufacturers_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeProducts, model.SitemapSettings.SitemapIncludeProducts_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeProductTags, model.SitemapSettings.SitemapIncludeProductTags_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeBlogPosts, model.SitemapSettings.SitemapIncludeBlogPosts_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeNews, model.SitemapSettings.SitemapIncludeNews_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeTopics, model.SitemapSettings.SitemapIncludeTopics_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.EnableHtmlMinification, model.MinificationSettings.EnableHtmlMinification_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.UseResponseCompression, model.MinificationSettings.UseResponseCompression_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.HeaderCustomHtml, model.CustomHtmlSettings.HeaderCustomHtml_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.FooterCustomHtml, model.CustomHtmlSettings.FooterCustomHtml_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //seo settings
            var seoSettings = await _settingService.LoadSettingAsync<SeoSettings>(storeScope);
            seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
            seoSettings.PageTitleSeoAdjustment = (PageTitleSeoAdjustment)model.SeoSettings.PageTitleSeoAdjustment;
            seoSettings.GenerateProductMetaDescription = model.SeoSettings.GenerateProductMetaDescription;
            seoSettings.ConvertNonWesternChars = model.SeoSettings.ConvertNonWesternChars;
            seoSettings.CanonicalUrlsEnabled = model.SeoSettings.CanonicalUrlsEnabled;
            seoSettings.WwwRequirement = (WwwRequirement)model.SeoSettings.WwwRequirement;
            seoSettings.TwitterMetaTags = model.SeoSettings.TwitterMetaTags;
            seoSettings.OpenGraphMetaTags = model.SeoSettings.OpenGraphMetaTags;
            seoSettings.MicrodataEnabled = model.SeoSettings.MicrodataEnabled;
            seoSettings.CustomHeadTags = model.SeoSettings.CustomHeadTags;

            //we do not clear cache after each setting update.
            //this behavior can increase performance because cached settings will not be cleared 
            //and loaded from database after each update
            await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.PageTitleSeparator, model.SeoSettings.PageTitleSeparator_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.PageTitleSeoAdjustment, model.SeoSettings.PageTitleSeoAdjustment_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.GenerateProductMetaDescription, model.SeoSettings.GenerateProductMetaDescription_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.ConvertNonWesternChars, model.SeoSettings.ConvertNonWesternChars_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.CanonicalUrlsEnabled, model.SeoSettings.CanonicalUrlsEnabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.WwwRequirement, model.SeoSettings.WwwRequirement_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.TwitterMetaTags, model.SeoSettings.TwitterMetaTags_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.OpenGraphMetaTags, model.SeoSettings.OpenGraphMetaTags_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.CustomHeadTags, model.SeoSettings.CustomHeadTags_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.MicrodataEnabled, model.SeoSettings.MicrodataEnabled_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //security settings
            var securitySettings = await _settingService.LoadSettingAsync<SecuritySettings>(storeScope);
            if (securitySettings.AdminAreaAllowedIpAddresses == null)
                securitySettings.AdminAreaAllowedIpAddresses = new List<string>();
            securitySettings.AdminAreaAllowedIpAddresses.Clear();
            if (!string.IsNullOrEmpty(model.SecuritySettings.AdminAreaAllowedIpAddresses))
                foreach (var s in model.SecuritySettings.AdminAreaAllowedIpAddresses.Split(_separator, StringSplitOptions.RemoveEmptyEntries))
                    if (!string.IsNullOrWhiteSpace(s))
                        securitySettings.AdminAreaAllowedIpAddresses.Add(s.Trim());
            securitySettings.HoneypotEnabled = model.SecuritySettings.HoneypotEnabled;
            await _settingService.SaveSettingAsync(securitySettings);

            //robots.txt settings
            var robotsTxtSettings = await _settingService.LoadSettingAsync<RobotsTxtSettings>(storeScope);
            robotsTxtSettings.AllowSitemapXml = model.RobotsTxtSettings.AllowSitemapXml;
            robotsTxtSettings.AdditionsRules = model.RobotsTxtSettings.AdditionsRules?.Split(Environment.NewLine).ToList();
            robotsTxtSettings.DisallowLanguages = model.RobotsTxtSettings.DisallowLanguages?.ToList() ?? new List<int>();
            robotsTxtSettings.DisallowPaths = model.RobotsTxtSettings.DisallowPaths?.Split(Environment.NewLine).ToList();
            robotsTxtSettings.LocalizableDisallowPaths = model.RobotsTxtSettings.LocalizableDisallowPaths?.Split(Environment.NewLine).ToList();

            //we do not clear cache after each setting update.
            //this behavior can increase performance because cached settings will not be cleared 
            //and loaded from database after each update
            await _settingService.SaveSettingOverridablePerStoreAsync(robotsTxtSettings, x => x.AllowSitemapXml, model.RobotsTxtSettings.AllowSitemapXml_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(robotsTxtSettings, x => x.AdditionsRules, model.RobotsTxtSettings.AdditionsRules_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(robotsTxtSettings, x => x.DisallowLanguages, model.RobotsTxtSettings.DisallowLanguages_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(robotsTxtSettings, x => x.DisallowPaths, model.RobotsTxtSettings.DisallowPaths_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(robotsTxtSettings, x => x.LocalizableDisallowPaths, model.RobotsTxtSettings.LocalizableDisallowPaths_OverrideForStore, storeScope, false);

            // now clear settings cache
            await _settingService.ClearCacheAsync();

            //captcha settings
            var captchaSettings = await _settingService.LoadSettingAsync<CaptchaSettings>(storeScope);
            captchaSettings.Enabled = model.CaptchaSettings.Enabled;
            captchaSettings.ShowOnLoginPage = model.CaptchaSettings.ShowOnLoginPage;
            captchaSettings.ShowOnRegistrationPage = model.CaptchaSettings.ShowOnRegistrationPage;
            captchaSettings.ShowOnContactUsPage = model.CaptchaSettings.ShowOnContactUsPage;
            captchaSettings.ShowOnEmailWishlistToFriendPage = model.CaptchaSettings.ShowOnEmailWishlistToFriendPage;
            captchaSettings.ShowOnEmailProductToFriendPage = model.CaptchaSettings.ShowOnEmailProductToFriendPage;
            captchaSettings.ShowOnBlogCommentPage = model.CaptchaSettings.ShowOnBlogCommentPage;
            captchaSettings.ShowOnNewsCommentPage = model.CaptchaSettings.ShowOnNewsCommentPage;
            captchaSettings.ShowOnNewsletterPage = model.CaptchaSettings.ShowOnNewsLetterPage;
            captchaSettings.ShowOnProductReviewPage = model.CaptchaSettings.ShowOnProductReviewPage;
            captchaSettings.ShowOnForgotPasswordPage = model.CaptchaSettings.ShowOnForgotPasswordPage;
            captchaSettings.ShowOnApplyVendorPage = model.CaptchaSettings.ShowOnApplyVendorPage;
            captchaSettings.ShowOnForum = model.CaptchaSettings.ShowOnForum;
            captchaSettings.ShowOnCheckoutPageForGuests = model.CaptchaSettings.ShowOnCheckoutPageForGuests;
            captchaSettings.ShowOnCheckGiftCardBalance = model.CaptchaSettings.ShowOnCheckGiftCardBalance;
            captchaSettings.ReCaptchaPublicKey = model.CaptchaSettings.ReCaptchaPublicKey;
            captchaSettings.ReCaptchaPrivateKey = model.CaptchaSettings.ReCaptchaPrivateKey;
            captchaSettings.CaptchaType = (CaptchaType)model.CaptchaSettings.CaptchaType;
            captchaSettings.ReCaptchaV3ScoreThreshold = model.CaptchaSettings.ReCaptchaV3ScoreThreshold;

            //we do not clear cache after each setting update.
            //this behavior can increase performance because cached settings will not be cleared 
            //and loaded from database after each update
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.Enabled, model.CaptchaSettings.Enabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnLoginPage, model.CaptchaSettings.ShowOnLoginPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnRegistrationPage, model.CaptchaSettings.ShowOnRegistrationPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnContactUsPage, model.CaptchaSettings.ShowOnContactUsPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnEmailWishlistToFriendPage, model.CaptchaSettings.ShowOnEmailWishlistToFriendPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnEmailProductToFriendPage, model.CaptchaSettings.ShowOnEmailProductToFriendPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnBlogCommentPage, model.CaptchaSettings.ShowOnBlogCommentPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnNewsCommentPage, model.CaptchaSettings.ShowOnNewsCommentPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnNewsletterPage, model.CaptchaSettings.ShowOnNewsLetterPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnProductReviewPage, model.CaptchaSettings.ShowOnProductReviewPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnApplyVendorPage, model.CaptchaSettings.ShowOnApplyVendorPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnForgotPasswordPage, model.CaptchaSettings.ShowOnForgotPasswordPage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnForum, model.CaptchaSettings.ShowOnForum_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnCheckoutPageForGuests, model.CaptchaSettings.ShowOnCheckoutPageForGuests_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnCheckGiftCardBalance, model.CaptchaSettings.ShowOnCheckGiftCardBalance_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ReCaptchaPublicKey, model.CaptchaSettings.ReCaptchaPublicKey_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ReCaptchaPrivateKey, model.CaptchaSettings.ReCaptchaPrivateKey_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ReCaptchaV3ScoreThreshold, model.CaptchaSettings.ReCaptchaV3ScoreThreshold_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.CaptchaType, model.CaptchaSettings.CaptchaType_OverrideForStore, storeScope, false);

            // now clear settings cache
            await _settingService.ClearCacheAsync();

            if (captchaSettings.Enabled &&
                (string.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPublicKey) || string.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPrivateKey)))
            {
                //captcha is enabled but the keys are not entered
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.CaptchaAppropriateKeysNotEnteredError"));
            }

            //PDF settings
            var pdfSettings = await _settingService.LoadSettingAsync<PdfSettings>(storeScope);
            pdfSettings.LetterPageSizeEnabled = model.PdfSettings.LetterPageSizeEnabled;
            pdfSettings.LogoPictureId = model.PdfSettings.LogoPictureId;
            pdfSettings.DisablePdfInvoicesForPendingOrders = model.PdfSettings.DisablePdfInvoicesForPendingOrders;
            pdfSettings.InvoiceFooterTextColumn1 = model.PdfSettings.InvoiceFooterTextColumn1;
            pdfSettings.InvoiceFooterTextColumn2 = model.PdfSettings.InvoiceFooterTextColumn2;

            //we do not clear cache after each setting update.
            //this behavior can increase performance because cached settings will not be cleared 
            //and loaded from database after each update
            await _settingService.SaveSettingOverridablePerStoreAsync(pdfSettings, x => x.LetterPageSizeEnabled, model.PdfSettings.LetterPageSizeEnabled_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(pdfSettings, x => x.LogoPictureId, model.PdfSettings.LogoPictureId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, model.PdfSettings.DisablePdfInvoicesForPendingOrders_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(pdfSettings, x => x.InvoiceFooterTextColumn1, model.PdfSettings.InvoiceFooterTextColumn1_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(pdfSettings, x => x.InvoiceFooterTextColumn2, model.PdfSettings.InvoiceFooterTextColumn2_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //localization settings
            var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>(storeScope);
            localizationSettings.UseImagesForLanguageSelection = model.LocalizationSettings.UseImagesForLanguageSelection;
            if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled != model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
            }

            localizationSettings.AutomaticallyDetectLanguage = model.LocalizationSettings.AutomaticallyDetectLanguage;
            localizationSettings.LoadAllLocaleRecordsOnStartup = model.LocalizationSettings.LoadAllLocaleRecordsOnStartup;
            localizationSettings.LoadAllLocalizedPropertiesOnStartup = model.LocalizationSettings.LoadAllLocalizedPropertiesOnStartup;
            localizationSettings.LoadAllUrlRecordsOnStartup = model.LocalizationSettings.LoadAllUrlRecordsOnStartup;
            await _settingService.SaveSettingAsync(localizationSettings);

            //translation settings
            var translationSettings = await _settingService.LoadSettingAsync<TranslationSettings>(storeScope);
            translationSettings.AllowPreTranslate = model.TranslationSettings.AllowPreTranslate;
            translationSettings.TranslateFromLanguageId = model.TranslationSettings.TranslateFromLanguageId;
            translationSettings.NotTranslateLanguages = model.TranslationSettings.NotTranslateLanguages?.ToList() ?? new List<int>();
            translationSettings.GoogleApiKey = model.TranslationSettings.GoogleApiKey;
            translationSettings.DeepLAuthKey = model.TranslationSettings.DeepLAuthKey;
            translationSettings.TranslationServiceId = model.TranslationSettings.TranslationServiceId;
            await _settingService.SaveSettingAsync(translationSettings);

            //admin area
            var adminAreaSettings = await _settingService.LoadSettingAsync<AdminAreaSettings>(storeScope);

            //we do not clear cache after each setting update.
            //this behavior can increase performance because cached settings will not be cleared 
            //and loaded from database after each update
            adminAreaSettings.UseRichEditorInMessageTemplates = model.AdminAreaSettings.UseRichEditorInMessageTemplates;
            adminAreaSettings.UseStickyHeaderLayout = model.AdminAreaSettings.UseStickyHeaderLayout;

            await _settingService.SaveSettingOverridablePerStoreAsync(adminAreaSettings, x => x.UseRichEditorInMessageTemplates, model.AdminAreaSettings.UseRichEditorInMessageTemplates_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingAsync(adminAreaSettings, x => x.UseStickyHeaderLayout, clearCache: false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //activity log
            await _customerActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("GeneralCommon");
        }

        //prepare model
        model = await _settingModelFactory.PrepareGeneralCommonSettingsModelAsync(model);

        //if we got this far, something failed, redisplay form
        return View(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> AllSettings(string settingName)
    {
        //prepare model
        var model = await _settingModelFactory.PrepareSettingSearchModelAsync(new SettingSearchModel { SearchSettingName = WebUtility.HtmlEncode(settingName) });

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> AllSettings(SettingSearchModel searchModel)
    {
        //prepare model
        var model = await _settingModelFactory.PrepareSettingListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> SettingUpdate(SettingModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        //try to get a setting with the specified id
        var setting = await _settingService.GetSettingByIdAsync(model.Id)
            ?? throw new ArgumentException("No setting found with the specified id");

        if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
        {
            //setting name has been changed
            await _settingService.DeleteSettingAsync(setting);
        }

        await _settingService.SetSettingAsync(model.Name, model.Value, setting.StoreId);

        //activity log
        await _customerActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"), setting);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> SettingAdd(SettingModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var storeId = model.StoreId;
        await _settingService.SetSettingAsync(model.Name, model.Value, storeId);

        //activity log
        await _customerActivityService.InsertActivityAsync("AddNewSetting",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewSetting"), model.Name),
            await _settingService.GetSettingAsync(model.Name, storeId));

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SETTINGS)]
    public virtual async Task<IActionResult> SettingDelete(int id)
    {
        //try to get a setting with the specified id
        var setting = await _settingService.GetSettingByIdAsync(id)
            ?? throw new ArgumentException("No setting found with the specified id", nameof(id));

        await _settingService.DeleteSettingAsync(setting);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteSetting",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteSetting"), setting.Name), setting);

        return new NullJsonResult();
    }

    public virtual async Task<IActionResult> DistributedCacheHighTrafficWarning(bool loadAllLocaleRecordsOnStartup)
    {
        //LoadAllLocaleRecordsOnStartup is set and distributed cache is used, so display warning
        if (_appSettings.Get<DistributedCacheConfig>().Enabled && _appSettings.Get<DistributedCacheConfig>().DistributedCacheType != DistributedCacheType.Memory && loadAllLocaleRecordsOnStartup)
        {
            return Json(new
            {
                Result = await _localizationService
                    .GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup.Warning")
            });
        }

        return Json(new { Result = string.Empty });
    }

    //Action that displays a notification (warning) to the store owner about the need to restart the application after changing the setting
    public virtual async Task<IActionResult> SeoFriendlyUrlsForLanguagesEnabledWarning(bool seoFriendlyUrlsForLanguagesEnabled)
    {
        //load settings for a chosen store scope
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>(storeScope);

        if (seoFriendlyUrlsForLanguagesEnabled != localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
        {
            return Json(new
            {
                Result = await _localizationService
                    .GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled.Warning")
            });
        }

        return Json(new { Result = string.Empty });
    }

    #endregion
}