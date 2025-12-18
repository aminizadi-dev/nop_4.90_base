using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Translation;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Configuration;
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Services.Themes;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.WebOptimizer;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the setting model factory implementation
/// </summary>
public partial class SettingModelFactory : ISettingModelFactory
{
    #region Fields

    protected readonly AppSettings _appSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly INopDataProvider _dataProvider;
    protected readonly INopFileProvider _fileProvider;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPictureService _pictureService;
    protected readonly ISettingService _settingService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IThemeProvider _themeProvider;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public SettingModelFactory(AppSettings appSettings,
        IAddressModelFactory addressModelFactory,
        IAddressService addressService,
        IBaseAdminModelFactory baseAdminModelFactory,
        INopDataProvider dataProvider,
        INopFileProvider fileProvider,
        IDateTimeHelper dateTimeHelper,
        ILocalizedModelFactory localizedModelFactory,
        IGenericAttributeService genericAttributeService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        IPictureService pictureService,
        ISettingService settingService,
        IStoreContext storeContext,
        IStoreService storeService,
        IThemeProvider themeProvider,
        IWorkContext workContext)
    {
        _appSettings = appSettings;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _dataProvider = dataProvider;
        _fileProvider = fileProvider;
        _dateTimeHelper = dateTimeHelper;
        _localizedModelFactory = localizedModelFactory;
        _genericAttributeService = genericAttributeService;
        _languageService = languageService;
        _localizationService = localizationService;
        _pictureService = pictureService;
        _settingService = settingService;
        _storeContext = storeContext;
        _storeService = storeService;
        _themeProvider = themeProvider;
        _workContext = workContext;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare store theme models
    /// </summary>
    /// <param name="models">List of store theme models</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareStoreThemeModelsAsync(IList<StoreInformationSettingsModel.ThemeModel> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeId);

        //get available themes
        var availableThemes = await _themeProvider.GetThemesAsync();
        foreach (var theme in availableThemes)
        {
            models.Add(new StoreInformationSettingsModel.ThemeModel
            {
                FriendlyName = theme.FriendlyName,
                SystemName = theme.SystemName,
                PreviewImageUrl = theme.PreviewImageUrl,
                PreviewText = theme.PreviewText,
                SupportRtl = theme.SupportRtl,
                Selected = theme.SystemName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
            });
        }
    }

    protected virtual async Task<StoreInformationSettingsModel> PrepareStoreInformationSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeId);
        var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);

        //fill in model values from the entity
        var model = new StoreInformationSettingsModel
        {
            StoreClosed = storeInformationSettings.StoreClosed,
            DefaultStoreTheme = storeInformationSettings.DefaultStoreTheme,
            AllowCustomerToSelectTheme = storeInformationSettings.AllowCustomerToSelectTheme,
            LogoPictureId = storeInformationSettings.LogoPictureId,
            DisplayEuCookieLawWarning = storeInformationSettings.DisplayEuCookieLawWarning,
            FacebookLink = storeInformationSettings.FacebookLink,
            TwitterLink = storeInformationSettings.TwitterLink,
            YoutubeLink = storeInformationSettings.YoutubeLink,
            InstagramLink = storeInformationSettings.InstagramLink,
            SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm,
            UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm,
            PopupForTermsOfServiceLinks = commonSettings.PopupForTermsOfServiceLinks
        };

        //prepare available themes
        await PrepareStoreThemeModelsAsync(model.AvailableStoreThemes);

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.StoreClosed_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.StoreClosed, storeId);
        model.DefaultStoreTheme_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.DefaultStoreTheme, storeId);
        model.AllowCustomerToSelectTheme_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.AllowCustomerToSelectTheme, storeId);
        model.LogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.LogoPictureId, storeId);
        model.DisplayEuCookieLawWarning_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeId);
        model.FacebookLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.FacebookLink, storeId);
        model.TwitterLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.TwitterLink, storeId);
        model.YoutubeLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.YoutubeLink, storeId);
        model.InstagramLink_OverrideForStore = await _settingService.SettingExistsAsync(storeInformationSettings, x => x.InstagramLink, storeId);
        model.SubjectFieldOnContactUsForm_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.SubjectFieldOnContactUsForm, storeId);
        model.UseSystemEmailForContactUsForm_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.UseSystemEmailForContactUsForm, storeId);
        model.PopupForTermsOfServiceLinks_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.PopupForTermsOfServiceLinks, storeId);

        return model;
    }


    /// <summary>
    /// Prepare sort option search model
    /// </summary>
    /// <param name="searchModel">Sort option search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sort option search model
    /// </returns>
    protected virtual Task<SortOptionSearchModel> PrepareSortOptionSearchModelAsync(SortOptionSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare address settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the address settings model
    /// </returns>
    protected virtual async Task<AddressSettingsModel> PrepareAddressSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var addressSettings = await _settingService.LoadSettingAsync<AddressSettings>(storeId);

        //fill in model values from the entity
        var model = addressSettings.ToSettingsModel<AddressSettingsModel>();

        return model;
    }

    /// <summary>
    /// Prepare customer settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer settings model
    /// </returns>
    protected virtual async Task<CustomerSettingsModel> PrepareCustomerSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var customerSettings = await _settingService.LoadSettingAsync<CustomerSettings>(storeId);

        //fill in model values from the entity
        var model = customerSettings.ToSettingsModel<CustomerSettingsModel>();

        return model;
    }

    /// <summary>
    /// Prepare multi-factor authentication settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multiFactorAuthenticationSettingsModel
    /// </returns>
    protected virtual async Task<MultiFactorAuthenticationSettingsModel> PrepareMultiFactorAuthenticationSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var multiFactorAuthenticationSettings = await _settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>(storeId);

        //fill in model values from the entity
        var model = multiFactorAuthenticationSettings.ToSettingsModel<MultiFactorAuthenticationSettingsModel>();

        return model;

    }

    /// <summary>
    /// Prepare date time settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the date time settings model
    /// </returns>
    protected virtual async Task<DateTimeSettingsModel> PrepareDateTimeSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var dateTimeSettings = await _settingService.LoadSettingAsync<DateTimeSettings>(storeId);

        //fill in model values from the entity
        var model = new DateTimeSettingsModel
        {
            AllowCustomersToSetTimeZone = dateTimeSettings.AllowCustomersToSetTimeZone
        };

        //fill in additional values (not existing in the entity)
        model.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id;

        //prepare available time zones
        await _baseAdminModelFactory.PrepareTimeZonesAsync(model.AvailableTimeZones, false);

        return model;
    }

    /// <summary>
    /// Prepare Sitemap settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap settings model
    /// </returns>
    protected virtual async Task<SitemapSettingsModel> PrepareSitemapSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var sitemapSettings = await _settingService.LoadSettingAsync<SitemapSettings>(storeId);

        //fill in model values from the entity
        var model = new SitemapSettingsModel
        {
            SitemapEnabled = sitemapSettings.SitemapEnabled,
            SitemapPageSize = sitemapSettings.SitemapPageSize,
            SitemapIncludeCategories = sitemapSettings.SitemapIncludeCategories,
            SitemapIncludeManufacturers = sitemapSettings.SitemapIncludeManufacturers,
            SitemapIncludeProducts = sitemapSettings.SitemapIncludeProducts,
            SitemapIncludeProductTags = sitemapSettings.SitemapIncludeProductTags,
            SitemapIncludeBlogPosts = sitemapSettings.SitemapIncludeBlogPosts,
            SitemapIncludeNews = sitemapSettings.SitemapIncludeNews,
            SitemapIncludeTopics = sitemapSettings.SitemapIncludeTopics
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.SitemapEnabled_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapEnabled, storeId);
        model.SitemapPageSize_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapPageSize, storeId);
        model.SitemapIncludeCategories_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeCategories, storeId);
        model.SitemapIncludeManufacturers_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeManufacturers, storeId);
        model.SitemapIncludeProducts_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeProducts, storeId);
        model.SitemapIncludeProductTags_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeProductTags, storeId);
        model.SitemapIncludeBlogPosts_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeBlogPosts, storeId);
        model.SitemapIncludeNews_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeNews, storeId);
        model.SitemapIncludeTopics_OverrideForStore = await _settingService.SettingExistsAsync(sitemapSettings, x => x.SitemapIncludeTopics, storeId);

        return model;
    }

    /// <summary>
    /// Prepare minification settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the minification settings model
    /// </returns>
    protected virtual async Task<MinificationSettingsModel> PrepareMinificationSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var minificationSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);

        //fill in model values from the entity
        var model = new MinificationSettingsModel
        {
            EnableHtmlMinification = minificationSettings.EnableHtmlMinification,
            UseResponseCompression = minificationSettings.UseResponseCompression
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.EnableHtmlMinification_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.EnableHtmlMinification, storeId);
        model.UseResponseCompression_OverrideForStore = await _settingService.SettingExistsAsync(minificationSettings, x => x.UseResponseCompression, storeId);

        return model;
    }

    /// <summary>
    /// Prepare SEO settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sEO settings model
    /// </returns>
    protected virtual async Task<SeoSettingsModel> PrepareSeoSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var seoSettings = await _settingService.LoadSettingAsync<SeoSettings>(storeId);

        //fill in model values from the entity
        var model = new SeoSettingsModel
        {
            PageTitleSeparator = seoSettings.PageTitleSeparator,
            PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment,
            PageTitleSeoAdjustmentValues = await seoSettings.PageTitleSeoAdjustment.ToSelectListAsync(),
            GenerateProductMetaDescription = seoSettings.GenerateProductMetaDescription,
            ConvertNonWesternChars = seoSettings.ConvertNonWesternChars,
            CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled,
            WwwRequirement = (int)seoSettings.WwwRequirement,
            WwwRequirementValues = await seoSettings.WwwRequirement.ToSelectListAsync(),

            TwitterMetaTags = seoSettings.TwitterMetaTags,
            OpenGraphMetaTags = seoSettings.OpenGraphMetaTags,
            CustomHeadTags = seoSettings.CustomHeadTags,
            MicrodataEnabled = seoSettings.MicrodataEnabled
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.PageTitleSeparator_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeparator, storeId);
        model.PageTitleSeoAdjustment_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.PageTitleSeoAdjustment, storeId);
        model.GenerateProductMetaDescription_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.GenerateProductMetaDescription, storeId);
        model.ConvertNonWesternChars_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.ConvertNonWesternChars, storeId);
        model.CanonicalUrlsEnabled_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.CanonicalUrlsEnabled, storeId);
        model.WwwRequirement_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.WwwRequirement, storeId);
        model.TwitterMetaTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.TwitterMetaTags, storeId);
        model.OpenGraphMetaTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.OpenGraphMetaTags, storeId);
        model.CustomHeadTags_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.CustomHeadTags, storeId);
        model.MicrodataEnabled_OverrideForStore = await _settingService.SettingExistsAsync(seoSettings, x => x.MicrodataEnabled, storeId);

        return model;
    }

    /// <summary>
    /// Prepare security settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the security settings model
    /// </returns>
    protected virtual async Task<SecuritySettingsModel> PrepareSecuritySettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var securitySettings = await _settingService.LoadSettingAsync<SecuritySettings>(storeId);

        //fill in model values from the entity
        var model = new SecuritySettingsModel
        {
            EncryptionKey = securitySettings.EncryptionKey,
            HoneypotEnabled = securitySettings.HoneypotEnabled
        };

        //fill in additional values (not existing in the entity)
        if (securitySettings.AdminAreaAllowedIpAddresses != null)
            model.AdminAreaAllowedIpAddresses = string.Join(",", securitySettings.AdminAreaAllowedIpAddresses);

        return model;
    }

    /// <summary>
    /// Prepare captcha settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the captcha settings model
    /// </returns>
    protected virtual async Task<CaptchaSettingsModel> PrepareCaptchaSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var captchaSettings = await _settingService.LoadSettingAsync<CaptchaSettings>(storeId);

        //fill in model values from the entity
        var model = captchaSettings.ToSettingsModel<CaptchaSettingsModel>();

        model.CaptchaTypeValues = await captchaSettings.CaptchaType.ToSelectListAsync();

        if (storeId <= 0)
            return model;

        model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.Enabled, storeId);
        model.ShowOnLoginPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnLoginPage, storeId);
        model.ShowOnRegistrationPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnRegistrationPage, storeId);
        model.ShowOnContactUsPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnContactUsPage, storeId);
        model.ShowOnEmailWishlistToFriendPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailWishlistToFriendPage, storeId);
        model.ShowOnEmailProductToFriendPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnEmailProductToFriendPage, storeId);
        model.ShowOnBlogCommentPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnBlogCommentPage, storeId);
        model.ShowOnNewsCommentPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnNewsCommentPage, storeId);
        model.ShowOnNewsLetterPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnNewsletterPage, storeId);
        model.ShowOnProductReviewPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnProductReviewPage, storeId);
        model.ShowOnApplyVendorPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnApplyVendorPage, storeId);
        model.ShowOnForgotPasswordPage_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForgotPasswordPage, storeId);
        model.ShowOnForum_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnForum, storeId);
        model.ShowOnCheckoutPageForGuests_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ShowOnCheckoutPageForGuests, storeId);
        model.ReCaptchaPublicKey_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPublicKey, storeId);
        model.ReCaptchaPrivateKey_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaPrivateKey, storeId);
        model.CaptchaType_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.CaptchaType, storeId);
        model.ReCaptchaV3ScoreThreshold_OverrideForStore = await _settingService.SettingExistsAsync(captchaSettings, x => x.ReCaptchaV3ScoreThreshold, storeId);

        return model;
    }

    /// <summary>
    /// Prepare PDF settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pDF settings model
    /// </returns>
    protected virtual async Task<PdfSettingsModel> PreparePdfSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var pdfSettings = await _settingService.LoadSettingAsync<PdfSettings>(storeId);

        //fill in model values from the entity
        var model = new PdfSettingsModel
        {
            LetterPageSizeEnabled = pdfSettings.LetterPageSizeEnabled,
            LogoPictureId = pdfSettings.LogoPictureId,
            DisablePdfInvoicesForPendingOrders = pdfSettings.DisablePdfInvoicesForPendingOrders,
            InvoiceFooterTextColumn1 = pdfSettings.InvoiceFooterTextColumn1,
            InvoiceFooterTextColumn2 = pdfSettings.InvoiceFooterTextColumn2
        };

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.LetterPageSizeEnabled_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.LetterPageSizeEnabled, storeId);
        model.LogoPictureId_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.LogoPictureId, storeId);
        model.DisablePdfInvoicesForPendingOrders_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, storeId);
        model.InvoiceFooterTextColumn1_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn1, storeId);
        model.InvoiceFooterTextColumn2_OverrideForStore = await _settingService.SettingExistsAsync(pdfSettings, x => x.InvoiceFooterTextColumn2, storeId);

        return model;
    }

    /// <summary>
    /// Prepare localization settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the localization settings model
    /// </returns>
    protected virtual async Task<LocalizationSettingsModel> PrepareLocalizationSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>(storeId);

        //fill in model values from the entity
        var model = new LocalizationSettingsModel
        {
            UseImagesForLanguageSelection = localizationSettings.UseImagesForLanguageSelection,
            SeoFriendlyUrlsForLanguagesEnabled = localizationSettings.SeoFriendlyUrlsForLanguagesEnabled,
            AutomaticallyDetectLanguage = localizationSettings.AutomaticallyDetectLanguage,
            LoadAllLocaleRecordsOnStartup = localizationSettings.LoadAllLocaleRecordsOnStartup,
            LoadAllLocalizedPropertiesOnStartup = localizationSettings.LoadAllLocalizedPropertiesOnStartup,
            LoadAllUrlRecordsOnStartup = localizationSettings.LoadAllUrlRecordsOnStartup
        };

        return model;
    }

    /// <summary>
    /// Prepare translation settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the translation settings model
    /// </returns>
    protected virtual async Task<TranslationSettingsModel> PrepareTranslationSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var translationSettings = await _settingService.LoadSettingAsync<TranslationSettings>(storeId);

        //fill in model values from the entity
        var model = new TranslationSettingsModel
        {
            AllowPreTranslate = translationSettings.AllowPreTranslate,
            TranslateFromLanguageId = translationSettings.TranslateFromLanguageId,
            NotTranslateLanguages = translationSettings.NotTranslateLanguages ?? new List<int>(),
            GoogleApiKey = translationSettings.GoogleApiKey,
            DeepLAuthKey = translationSettings.DeepLAuthKey,
            TranslationServiceId = translationSettings.TranslationServiceId
        };

        //prepare available translation services
        var availableTranslationServices = await TranslationServiceType.GoogleTranslate.ToSelectListAsync(false);
        model.AvailableTranslationService = availableTranslationServices.ToList();

        //prepare available languages
        await _baseAdminModelFactory.PrepareLanguagesAsync(model.AvailableLanguages, false);

        return model;
    }

    /// <summary>
    /// Prepare admin area settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the admin area settings model
    /// </returns>
    protected virtual async Task<AdminAreaSettingsModel> PrepareAdminAreaSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var adminAreaSettings = await _settingService.LoadSettingAsync<AdminAreaSettings>(storeId);

        //fill in model values from the entity
        var model = new AdminAreaSettingsModel
        {
            UseRichEditorInMessageTemplates = adminAreaSettings.UseRichEditorInMessageTemplates,
            UseStickyHeaderLayout = adminAreaSettings.UseStickyHeaderLayout
        };

        //fill in overridden values
        if (storeId > 0)
        {
            model.UseRichEditorInMessageTemplates_OverrideForStore = await _settingService.SettingExistsAsync(adminAreaSettings, x => x.UseRichEditorInMessageTemplates, storeId);
        }

        return model;
    }

    /// <summary>
    /// Prepare setting model to add
    /// </summary>
    /// <param name="model">Setting model to add</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareAddSettingModelAsync(SettingModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);
    }

    /// <summary>
    /// Prepare custom HTML settings model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the custom HTML settings model
    /// </returns>
    protected virtual async Task<CustomHtmlSettingsModel> PrepareCustomHtmlSettingsModelAsync()
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeId);

        //fill in model values from the entity
        var model = new CustomHtmlSettingsModel
        {
            HeaderCustomHtml = commonSettings.HeaderCustomHtml,
            FooterCustomHtml = commonSettings.FooterCustomHtml
        };

        //fill in overridden values
        if (storeId > 0)
        {
            model.HeaderCustomHtml_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.HeaderCustomHtml, storeId);
            model.FooterCustomHtml_OverrideForStore = await _settingService.SettingExistsAsync(commonSettings, x => x.FooterCustomHtml, storeId);
        }

        return model;
    }

    /// <summary>
    /// Prepare robots.txt settings model
    /// </summary>
    /// <param name="model">robots.txt model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the robots.txt settings model
    /// </returns>
    protected virtual async Task<RobotsTxtSettingsModel> PrepareRobotsTxtSettingsModelAsync(RobotsTxtSettingsModel model = null)
    {
        var additionsInstruction =
            string.Format(
                await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsInstruction"),
                RobotsTxtDefaults.RobotsAdditionsFileName);

        if (_fileProvider.FileExists(_fileProvider.Combine(_fileProvider.MapPath("~/wwwroot"), RobotsTxtDefaults.RobotsCustomFileName)))
            return new RobotsTxtSettingsModel { CustomFileExists = string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.RobotsCustomFileExists"), RobotsTxtDefaults.RobotsCustomFileName), AdditionsInstruction = additionsInstruction };

        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var robotsTxtSettings = await _settingService.LoadSettingAsync<RobotsTxtSettings>(storeId);

        model ??= new RobotsTxtSettingsModel
        {
            AllowSitemapXml = robotsTxtSettings.AllowSitemapXml,
            DisallowPaths = string.Join(Environment.NewLine, robotsTxtSettings.DisallowPaths),
            LocalizableDisallowPaths =
                string.Join(Environment.NewLine, robotsTxtSettings.LocalizableDisallowPaths),
            DisallowLanguages = robotsTxtSettings.DisallowLanguages.ToList(),
            AdditionsRules = string.Join(Environment.NewLine, robotsTxtSettings.AdditionsRules),
            AvailableLanguages = new List<SelectListItem>()
        };

        if (!model.AvailableLanguages.Any())
            (model.AvailableLanguages as List<SelectListItem>)?.AddRange((await _languageService.GetAllLanguagesAsync(storeId: storeId)).Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }));

        model.AdditionsInstruction = additionsInstruction;

        if (storeId <= 0)
            return model;

        model.AdditionsRules_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.AdditionsRules, storeId);
        model.AllowSitemapXml_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.AllowSitemapXml, storeId);
        model.DisallowLanguages_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.DisallowLanguages, storeId);
        model.DisallowPaths_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.DisallowPaths, storeId);
        model.LocalizableDisallowPaths_OverrideForStore = await _settingService.SettingExistsAsync(robotsTxtSettings, x => x.LocalizableDisallowPaths, storeId);

        return model;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare app settings model
    /// </summary>
    /// <param name="model">AppSettings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the app settings model
    /// </returns>
    public virtual async Task<AppSettingsModel> PrepareAppSettingsModel(AppSettingsModel model = null)
    {
        model ??= new AppSettingsModel
        {
            CacheConfigModel = _appSettings.Get<CacheConfig>().ToConfigModel<CacheConfigModel>(),
            HostingConfigModel = _appSettings.Get<HostingConfig>().ToConfigModel<HostingConfigModel>(),
            DistributedCacheConfigModel = _appSettings.Get<DistributedCacheConfig>().ToConfigModel<DistributedCacheConfigModel>(),
            InstallationConfigModel = _appSettings.Get<InstallationConfig>().ToConfigModel<InstallationConfigModel>(),
            CommonConfigModel = _appSettings.Get<CommonConfig>().ToConfigModel<CommonConfigModel>(),
            DataConfigModel = _appSettings.Get<DataConfig>().ToConfigModel<DataConfigModel>(),
            WebOptimizerConfigModel = _appSettings.Get<WebOptimizerConfig>().ToConfigModel<WebOptimizerConfigModel>(),
        };

        model.DistributedCacheConfigModel.DistributedCacheTypeValues = await _appSettings.Get<DistributedCacheConfig>().DistributedCacheType.ToSelectListAsync();

        model.DataConfigModel.DataProviderTypeValues = await _appSettings.Get<DataConfig>().DataProvider.ToSelectListAsync();

        //Since we decided to use the naming of the DB connections section as in the .net core - "ConnectionStrings",
        //we are forced to adjust our internal model naming to this convention in this check.
        model.EnvironmentVariables.AddRange(from property in model.GetType().GetProperties()
                                            where property.Name != nameof(AppSettingsModel.EnvironmentVariables)
                                            from pp in property.PropertyType.GetProperties()
                                            where Environment.GetEnvironmentVariables().Contains($"{property.Name.Replace("Model", "").Replace("DataConfig", "ConnectionStrings")}__{pp.Name}")
                                            select $"{property.Name}_{pp.Name}");
        return model;
    }

    /// <summary>
    /// Prepare paged sort option list model
    /// </summary>
    /// <param name="searchModel">Sort option search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sort option list model
    /// </returns>

    /// <summary>
    /// Prepare media settings model
    /// </summary>
    /// <param name="model">Media settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the media settings model
    /// </returns>
    public virtual async Task<MediaSettingsModel> PrepareMediaSettingsModelAsync(MediaSettingsModel model = null)
    {
        //load settings for a chosen store scope
        var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>(storeId);

        //fill in model values from the entity
        model ??= mediaSettings.ToSettingsModel<MediaSettingsModel>();

        //fill in additional values (not existing in the entity)
        model.ActiveStoreScopeConfiguration = storeId;
        model.PicturesStoredIntoDatabase = await _pictureService.IsStoreInDbAsync();

        if (storeId <= 0)
            return model;

        //fill in overridden values
        model.AvatarPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AvatarPictureSize, storeId);
        model.ProductThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductThumbPictureSize, storeId);
        model.ProductDetailsPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductDetailsPictureSize, storeId);
        model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, storeId);
        model.AssociatedProductPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AssociatedProductPictureSize, storeId);
        model.CategoryThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.CategoryThumbPictureSize, storeId);
        model.ManufacturerThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ManufacturerThumbPictureSize, storeId);
        model.VendorThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.VendorThumbPictureSize, storeId);
        model.CartThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.CartThumbPictureSize, storeId);
        model.OrderThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.OrderThumbPictureSize, storeId);
        model.MiniCartThumbPictureSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MiniCartThumbPictureSize, storeId);
        model.MaximumImageSize_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MaximumImageSize, storeId);
        model.MultipleThumbDirectories_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.MultipleThumbDirectories, storeId);
        model.DefaultImageQuality_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.DefaultImageQuality, storeId);
        model.ImportProductImagesUsingHash_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ImportProductImagesUsingHash, storeId);
        model.DefaultPictureZoomEnabled_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.DefaultPictureZoomEnabled, storeId);
        model.AllowSvgUploads_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.AllowSvgUploads, storeId);
        model.ProductDefaultImageId_OverrideForStore = await _settingService.SettingExistsAsync(mediaSettings, x => x.ProductDefaultImageId, storeId);

        return model;
    }

    /// <summary>
    /// Prepare customer user settings model
    /// </summary>
    /// <param name="model">Customer user settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer user settings model
    /// </returns>
    public virtual async Task<CustomerUserSettingsModel> PrepareCustomerUserSettingsModelAsync(CustomerUserSettingsModel model = null)
    {
        model ??= new CustomerUserSettingsModel
        {
            ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync()
        };

        //prepare customer settings model
        model.CustomerSettings = await PrepareCustomerSettingsModelAsync();

        //prepare CustomerSettings list availableCountries
        await _baseAdminModelFactory.PrepareCountriesAsync(model.CustomerSettings.AvailableCountries);

        //prepare multi-factor authentication settings model
        model.MultiFactorAuthenticationSettings = await PrepareMultiFactorAuthenticationSettingsModelAsync();

        //prepare address settings model
        model.AddressSettings = await PrepareAddressSettingsModelAsync();

        //prepare AddressSettings list availableCountries
        await _baseAdminModelFactory.PrepareCountriesAsync(model.AddressSettings.AvailableCountries);

        //prepare date time settings model
        model.DateTimeSettings = await PrepareDateTimeSettingsModelAsync();

        return model;
    }

    /// <summary>
    /// Prepare general and common settings model
    /// </summary>
    /// <param name="model">General common settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the general and common settings model
    /// </returns>
    public virtual async Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModelAsync(GeneralCommonSettingsModel model = null)
    {
        model ??= new GeneralCommonSettingsModel
        {
            ActiveStoreScopeConfiguration = await _storeContext.GetActiveStoreScopeConfigurationAsync()
        };

        //prepare store information settings model
        model.StoreInformationSettings = await PrepareStoreInformationSettingsModelAsync();

        //prepare Sitemap settings model
        model.SitemapSettings = await PrepareSitemapSettingsModelAsync();

        //prepare Minification settings model
        model.MinificationSettings = await PrepareMinificationSettingsModelAsync();

        //prepare SEO settings model
        model.SeoSettings = await PrepareSeoSettingsModelAsync();

        //prepare security settings model
        model.SecuritySettings = await PrepareSecuritySettingsModelAsync();

        //prepare robots.txt settings model
        model.RobotsTxtSettings = await PrepareRobotsTxtSettingsModelAsync();

        //prepare captcha settings model
        model.CaptchaSettings = await PrepareCaptchaSettingsModelAsync();

        //prepare PDF settings model
        model.PdfSettings = await PreparePdfSettingsModelAsync();

        //prepare localization settings model
        model.LocalizationSettings = await PrepareLocalizationSettingsModelAsync();

        //prepare translation settings model
        model.TranslationSettings = await PrepareTranslationSettingsModelAsync();

        //prepare admin area settings model
        model.AdminAreaSettings = await PrepareAdminAreaSettingsModelAsync();

        //prepare custom HTML settings model
        model.CustomHtmlSettings = await PrepareCustomHtmlSettingsModelAsync();

        return model;
    }

    /// <summary>
    /// Prepare setting search model
    /// </summary>
    /// <param name="searchModel">Setting search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting search model
    /// </returns>
    public virtual async Task<SettingSearchModel> PrepareSettingSearchModelAsync(SettingSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare model to add
        await PrepareAddSettingModelAsync(searchModel.AddSetting);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged setting list model
    /// </summary>
    /// <param name="searchModel">Setting search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting list model
    /// </returns>
    public virtual async Task<SettingListModel> PrepareSettingListModelAsync(SettingSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get settings
        var settings = (await _settingService.GetAllSettingsAsync()).AsQueryable();

        //filter settings
        if (!string.IsNullOrEmpty(searchModel.SearchSettingName))
            settings = settings.Where(setting => setting.Name.ToLowerInvariant().Contains(searchModel.SearchSettingName.ToLowerInvariant()));
        if (!string.IsNullOrEmpty(searchModel.SearchSettingValue))
            settings = settings.Where(setting => setting.Value.ToLowerInvariant().Contains(searchModel.SearchSettingValue.ToLowerInvariant()));

        var pagedSettings = settings.ToList().ToPagedList(searchModel);

        //prepare list model
        var model = await new SettingListModel().PrepareToGridAsync(searchModel, pagedSettings, () =>
        {
            return pagedSettings.SelectAwait(async setting =>
            {
                //fill in model values from the entity
                var settingModel = setting.ToModel<SettingModel>();

                //fill in additional values (not existing in the entity)
                settingModel.Store = setting.StoreId > 0
                    ? (await _storeService.GetStoreByIdAsync(setting.StoreId))?.Name ?? "Deleted"
                    : await _localizationService.GetResourceAsync("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");

                return settingModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare setting mode model
    /// </summary>
    /// <param name="modeName">Mode name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting mode model
    /// </returns>
    public virtual async Task<SettingModeModel> PrepareSettingModeModelAsync(string modeName)
    {
        var model = new SettingModeModel
        {
            ModeName = modeName,
            Enabled = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), modeName)
        };

        return model;
    }

    /// <summary>
    /// Prepare store scope configuration model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store scope configuration model
    /// </returns>
    public virtual async Task<StoreScopeConfigurationModel> PrepareStoreScopeConfigurationModelAsync()
    {
        var model = new StoreScopeConfigurationModel
        {
            Stores = (await _storeService.GetAllStoresAsync()).Select(store => store.ToModel<StoreModel>()).ToList(),
            StoreId = await _storeContext.GetActiveStoreScopeConfigurationAsync()
        };

        return model;
    }

    #endregion
}