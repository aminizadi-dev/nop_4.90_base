using System.Globalization;
using System.Text;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
//COMMERCE DOMAIN REMOVED - Compile Fix
//Removed: using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
//COMMERCE DOMAIN/SERVICES REMOVED - Phase B
//Removed: using Nop.Core.Domain.Catalog;
//Removed: using Nop.Core.Domain.News;
//Removed: using Nop.Core.Domain.Orders;
//Removed: using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
//COMMERCE SERVICE REMOVED - Compile Fix
//Removed: using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Media;
//Removed: using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Themes;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the common models factory
/// </summary>
public partial class CommonModelFactory : ICommonModelFactory
{
    #region Fields

    protected readonly CaptchaSettings _captchaSettings;
    //COMMERCE SETTINGS/SERVICES REMOVED - Phase B
    //Removed: protected readonly CatalogSettings _catalogSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly CustomerSettings _customerSettings;
    //COMMERCE SETTINGS REMOVED - Compile Fix
    //Removed: protected readonly ForumSettings _forumSettings;
    protected readonly ICustomerService _customerService;
    //Removed: protected readonly IForumService _forumService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INopHtmlHelper _nopHtmlHelper;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    //Removed: protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IThemeContext _themeContext;
    protected readonly IThemeProvider _themeProvider;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly MediaSettings _mediaSettings;
    protected readonly MessagesSettings _messagesSettings;
    //COMMERCE SETTINGS REMOVED - Phase B
    //Removed: protected readonly NewsSettings _newsSettings;
    protected readonly RobotsTxtSettings _robotsTxtSettings;
    protected readonly SitemapXmlSettings _sitemapXmlSettings;
    protected readonly StoreInformationSettings _storeInformationSettings;

    #endregion

    #region Ctor

    public CommonModelFactory(CaptchaSettings captchaSettings,
        //COMMERCE SETTINGS/SERVICES REMOVED - Phase B
        //Removed: CatalogSettings catalogSettings,
        CommonSettings commonSettings,
        CustomerSettings customerSettings,
        //COMMERCE SETTINGS REMOVED - Compile Fix
        //Removed: ForumSettings forumSettings,
        ICustomerService customerService,
        //Removed: IForumService forumService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INopFileProvider fileProvider,
        INopHtmlHelper nopHtmlHelper,
        IPermissionService permissionService,
        IPictureService pictureService,
        //Removed: IShoppingCartService shoppingCartService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IThemeContext themeContext,
        IThemeProvider themeProvider,
        IWebHelper webHelper,
        IWorkContext workContext,
        LocalizationSettings localizationSettings,
        MediaSettings mediaSettings,
        MessagesSettings messagesSettings,
        //COMMERCE SETTINGS REMOVED - Phase B
        //Removed: NewsSettings newsSettings,
        RobotsTxtSettings robotsTxtSettings,
        SitemapXmlSettings sitemapXmlSettings,
        StoreInformationSettings storeInformationSettings)
    {
        _captchaSettings = captchaSettings;
        //COMMERCE SETTINGS/SERVICES REMOVED - Phase B
        //Removed: _catalogSettings = catalogSettings;
        _commonSettings = commonSettings;
        _customerSettings = customerSettings;
        //COMMERCE SETTINGS REMOVED - Compile Fix
        //Removed: _forumSettings = forumSettings;
        _customerService = customerService;
        //Removed: _forumService = forumService;
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
        _localizationService = localizationService;
        _fileProvider = fileProvider;
        _nopHtmlHelper = nopHtmlHelper;
        _permissionService = permissionService;
        _pictureService = pictureService;
        //Removed: _shoppingCartService = shoppingCartService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _themeContext = themeContext;
        _themeProvider = themeProvider;
        _webHelper = webHelper;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _messagesSettings = messagesSettings;
        _localizationSettings = localizationSettings;
        //COMMERCE SETTINGS REMOVED - Phase B
        //Removed: _newsSettings = newsSettings;
        _robotsTxtSettings = robotsTxtSettings;
        _sitemapXmlSettings = sitemapXmlSettings;
        _storeInformationSettings = storeInformationSettings;
    }

    #endregion

    #region Utilities

    private async Task<bool> IsHomePageAsync()
    {
        var storeLocationUri = new Uri(_webHelper.GetStoreLocation().TrimEnd('/'));
        var currentPageUri = new Uri(_webHelper.GetThisPageUrl(false).TrimEnd('/'));

        if (!_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            return storeLocationUri.Equals(currentPageUri);

        var currentLanguage = await _workContext.GetWorkingLanguageAsync();

        return Uri.TryCreate(storeLocationUri, currentLanguage.UniqueSeoCode, out var result) && result.Equals(currentPageUri);
    }

    /// <summary>
    /// Get the number of unread private messages
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of private messages
    /// </returns>
    protected virtual async Task<int> GetUnreadPrivateMessagesAsync()
    {
        //COMMERCE FEATURE REMOVED - Compile Fix
        //Private messages (forums) removed
        return 0;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the logo model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the logo model
    /// </returns>
    public virtual async Task<LogoModel> PrepareLogoModelAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var model = new LogoModel
        {
            StoreName = await _localizationService.GetLocalizedAsync(store, x => x.Name)
        };

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.StoreLogoPath
            , store, await _themeContext.GetWorkingThemeNameAsync(), _webHelper.IsCurrentConnectionSecured());
        model.LogoPath = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var logo = string.Empty;
            var logoPictureId = _storeInformationSettings.LogoPictureId;

            if (logoPictureId > 0)
                logo = await _pictureService.GetPictureUrlAsync(logoPictureId, showDefaultPicture: false);

            if (string.IsNullOrEmpty(logo))
            {
                //use default logo
                var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty;
                var storeLocation = _mediaSettings.UseAbsoluteImagePath ? _webHelper.GetStoreLocation() : $"{pathBase}/";
                logo = $"{storeLocation}Themes/{await _themeContext.GetWorkingThemeNameAsync()}/Content/images/logo.png";
            }

            return logo;
        });

        return model;
    }

    /// <summary>
    /// Prepare the language selector model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language selector model
    /// </returns>
    public virtual async Task<LanguageSelectorModel> PrepareLanguageSelectorModelAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var availableLanguages = (await _languageService
                .GetAllLanguagesAsync(storeId: store.Id))
            .Select(x => new LanguageModel
            {
                Id = x.Id,
                Name = x.Name,
                FlagImageFileName = x.FlagImageFileName,
            }).ToList();

        var model = new LanguageSelectorModel
        {
            CurrentLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
            AvailableLanguages = availableLanguages,
            UseImages = _localizationSettings.UseImagesForLanguageSelection
        };

        return model;
    }

    //COMMERCE METHOD REMOVED - Phase B
    //Removed: PrepareTaxTypeSelectorModelAsync (tax type selector)

    /// <summary>
    /// Prepare the header links model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the header links model
    /// </returns>
    public virtual async Task<HeaderLinksModel> PrepareHeaderLinksModelAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        //COMMERCE FEATURE REMOVED - Compile Fix
        //Private messages (forums) removed
        var unreadMessageCount = 0;
        var unreadMessage = string.Empty;
        var alertMessage = string.Empty;

        var model = new HeaderLinksModel
        {
            RegistrationType = _customerSettings.UserRegistrationType,
            IsAuthenticated = await _customerService.IsRegisteredAsync(customer),
            CustomerName = await _customerService.IsRegisteredAsync(customer) ? await _customerService.FormatUsernameAsync(customer) : string.Empty,
            //COMMERCE PERMISSIONS REMOVED - Phase B
            //Removed: ShoppingCartEnabled, WishlistEnabled
            UsePopupNotifications = _messagesSettings.UsePopupNotifications,
            AllowPrivateMessages = false, //COMMERCE FEATURE REMOVED - forums removed
            UnreadPrivateMessages = unreadMessage,
            AlertMessage = alertMessage,
        };
        //COMMERCE SHOPPING CART CODE REMOVED - Phase B
        //Removed: ShoppingCartItems, WishlistItems calculation

        return model;
    }

    /// <summary>
    /// Prepare the admin header links model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the admin header links model
    /// </returns>
    public virtual async Task<AdminHeaderLinksModel> PrepareAdminHeaderLinksModelAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var model = new AdminHeaderLinksModel
        {
            ImpersonatedCustomerName = await _customerService.IsRegisteredAsync(customer) ? await _customerService.FormatUsernameAsync(customer) : string.Empty,
            IsCustomerImpersonated = _workContext.OriginalCustomerIfImpersonated != null,
            DisplayAdminLink = await _permissionService.AuthorizeAsync(StandardPermission.Security.ACCESS_ADMIN_PANEL),
            EditPageUrl = _nopHtmlHelper.GetEditPageUrl()
        };

        return model;
    }

    /// <summary>
    /// Prepare the social model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the social model
    /// </returns>
    public virtual async Task<SocialModel> PrepareSocialModelAsync()
    {
        var model = new SocialModel
        {
            FacebookLink = _storeInformationSettings.FacebookLink,
            TwitterLink = _storeInformationSettings.TwitterLink,
            YoutubeLink = _storeInformationSettings.YoutubeLink,
            InstagramLink = _storeInformationSettings.InstagramLink,
            WorkingLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
            //COMMERCE SETTING REMOVED - Phase B
            //Removed: NewsEnabled
        };

        return model;
    }

    /// <summary>
    /// Prepare the footer model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the footer model
    /// </returns>
    public virtual async Task<FooterModel> PrepareFooterModelAsync()
    {
        return new FooterModel
        {
            StoreName = await _localizationService.GetLocalizedAsync(await _storeContext.GetCurrentStoreAsync(), x => x.Name),
            HidePoweredByNopCommerce = _storeInformationSettings.HidePoweredByNopCommerce,
            //COMMERCE SETTING REMOVED - Phase B
            //Removed: DisplayTaxShippingInfoFooter
            IsHomePage = await IsHomePageAsync()
        };
    }

    /// <summary>
    /// Prepare the contact us model
    /// </summary>
    /// <param name="model">Contact us model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the contact us model
    /// </returns>
    public virtual async Task<ContactUsModel> PrepareContactUsModelAsync(ContactUsModel model, bool excludeProperties)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (!excludeProperties)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            model.Email = customer.Email;
            model.FullName = await _customerService.GetCustomerFullNameAsync(customer);
        }

        model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;

        return model;
    }

    //COMMERCE METHOD REMOVED - Phase B
    //Removed: PrepareContactVendorModelAsync (contact vendor functionality)

    /// <summary>
    /// Prepare the store theme selector model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store theme selector model
    /// </returns>
    public virtual async Task<StoreThemeSelectorModel> PrepareStoreThemeSelectorModelAsync()
    {
        var model = new StoreThemeSelectorModel();

        var currentTheme = await _themeProvider.GetThemeBySystemNameAsync(await _themeContext.GetWorkingThemeNameAsync());
        model.CurrentStoreTheme = new StoreThemeModel
        {
            Name = currentTheme?.SystemName,
            Title = currentTheme?.FriendlyName
        };

        model.AvailableStoreThemes = (await _themeProvider.GetThemesAsync()).Select(x => new StoreThemeModel
        {
            Name = x.SystemName,
            Title = x.FriendlyName
        }).ToList();

        return model;
    }

    /// <summary>
    /// Prepare the favicon model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the favicon model
    /// </returns>
    public virtual Task<FaviconAndAppIconsModel> PrepareFaviconAndAppIconsModelAsync()
    {
        var model = new FaviconAndAppIconsModel
        {
            HeadCode = _commonSettings.FaviconAndAppIconsHeadCode
        };

        return Task.FromResult(model);
    }

    /// <summary>
    /// Get robots.txt file
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the robots.txt file as string
    /// </returns>
    public virtual async Task<string> PrepareRobotsTextFileAsync()
    {
        var sb = new StringBuilder();

        //if robots.custom.txt exists, let's use it instead of hard-coded data below
        var robotsFilePath = _fileProvider.Combine(_fileProvider.MapPath("~/wwwroot"), RobotsTxtDefaults.RobotsCustomFileName);
        if (_fileProvider.FileExists(robotsFilePath))
        {
            //the robots.txt file exists
            var robotsFileContent = await _fileProvider.ReadAllTextAsync(robotsFilePath, Encoding.UTF8);
            sb.Append(robotsFileContent);
        }
        else
        {
            sb.AppendLine("User-agent: *");

            //sitemap
            if (_sitemapXmlSettings.SitemapXmlEnabled && _robotsTxtSettings.AllowSitemapXml)
                sb.AppendLine($"Sitemap: {_webHelper.GetStoreLocation()}sitemap.xml");
            else
                sb.AppendLine("Disallow: /sitemap.xml");

            //host
            sb.AppendLine($"Host: {_webHelper.GetStoreLocation()}");

            //usual paths
            foreach (var path in _robotsTxtSettings.DisallowPaths)
                sb.AppendLine($"Disallow: {path}");

            //localizable paths (without SEO code)
            foreach (var path in _robotsTxtSettings.LocalizableDisallowPaths)
                sb.AppendLine($"Disallow: {path}");

            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                //URLs are localizable. Append SEO code
                foreach (var language in await _languageService.GetAllLanguagesAsync(storeId: store.Id))
                    if (_robotsTxtSettings.DisallowLanguages.Contains(language.Id))
                    {
                        sb.AppendLine($"Disallow: /{language.UniqueSeoCode}$");
                        sb.AppendLine($"Disallow: /{language.UniqueSeoCode}/");
                    }
                    else
                        foreach (var path in _robotsTxtSettings.LocalizableDisallowPaths)
                            sb.AppendLine($"Disallow: /{language.UniqueSeoCode}{path}");
            }

            foreach (var additionsRule in _robotsTxtSettings.AdditionsRules)
                sb.AppendLine(additionsRule);

            //load and add robots.txt additions to the end of file.
            var robotsAdditionsFile = _fileProvider.Combine(_fileProvider.MapPath("~/wwwroot"), RobotsTxtDefaults.RobotsAdditionsFileName);
            if (_fileProvider.FileExists(robotsAdditionsFile))
            {
                sb.AppendLine();
                var robotsFileContent = await _fileProvider.ReadAllTextAsync(robotsAdditionsFile, Encoding.UTF8);
                sb.Append(robotsFileContent);
            }
        }

        return sb.ToString();
    }

    #endregion
}