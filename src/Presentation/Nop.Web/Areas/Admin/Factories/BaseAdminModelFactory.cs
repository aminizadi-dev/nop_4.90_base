using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Core.Domain.Translation;
using Nop.Services;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Framework.Models.Translation;
using LogLevel = Nop.Core.Domain.Logging.LogLevel;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the implementation of the base model factory that implements a most common admin model factories methods
/// </summary>
public partial class BaseAdminModelFactory : IBaseAdminModelFactory
{
    #region Fields

    protected readonly ICountryService _countryService;
    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INewsLetterSubscriptionTypeService _newsLetterSubscriptionTypeService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IPluginService _pluginService;
    protected readonly IStoreService _storeService;
    protected readonly TranslationSettings _translationSettings;

    #endregion

    #region Ctor

    public BaseAdminModelFactory(
        ICountryService countryService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IEmailAccountService emailAccountService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INewsLetterSubscriptionTypeService newsLetterSubscriptionTypeService,
        IStateProvinceService stateProvinceService,
        IStaticCacheManager staticCacheManager,
        IPluginService pluginService,
        IStoreService storeService,
        TranslationSettings translationSettings)
    {

        _countryService = countryService;
        _customerActivityService = customerActivityService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _emailAccountService = emailAccountService;
        _languageService = languageService;
        _localizationService = localizationService;
        _newsLetterSubscriptionTypeService = newsLetterSubscriptionTypeService;
        _stateProvinceService = stateProvinceService;
        _staticCacheManager = staticCacheManager;
        _pluginService = pluginService;
        _storeService = storeService;
        _translationSettings = translationSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare default item
    /// </summary>
    /// <param name="items">Available items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use "All" text</param>
    /// <param name="defaultItemValue">Default item value; defaults 0</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareDefaultItemAsync(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null, string defaultItemValue = "0")
    {
        ArgumentNullException.ThrowIfNull(items);

        //whether to insert the first special item for the default value
        if (!withSpecialDefaultItem)
            return;

        //prepare item text
        defaultItemText ??= await _localizationService.GetResourceAsync("Admin.Common.All");

        //insert this default item at first
        items.Insert(0, new SelectListItem { Text = defaultItemText, Value = defaultItemValue });
    }

    //COMMERCE UTILITY REMOVED - Phase C
    //Removed: GetVendorListAsync (commerce feature)

    #endregion

    #region Methods

    /// <summary>
    /// Prepare available activity log types
    /// </summary>
    /// <param name="items">Activity log type items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareActivityLogTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available activity log types
        var availableActivityTypes = await _customerActivityService.GetAllActivityTypesAsync();
        foreach (var activityType in availableActivityTypes)
        {
            items.Add(new SelectListItem { Value = activityType.Id.ToString(), Text = activityType.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
    }

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareOrderStatusesAsync (commerce feature)
    //Removed: PreparePaymentStatusesAsync (commerce feature)
    //Removed: PrepareShippingStatusesAsync (commerce feature)

    /// <summary>
    /// Prepare available countries
    /// </summary>
    /// <param name="items">Country items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareCountriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available countries
        var availableCountries = await _countryService.GetAllCountriesAsync(showHidden: true);
        foreach (var country in availableCountries)
        {
            items.Add(new SelectListItem { Value = country.Id.ToString(), Text = country.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.SelectCountry"));
    }

    /// <summary>
    /// Prepare available states and provinces
    /// </summary>
    /// <param name="items">State and province items</param>
    /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareStatesAndProvincesAsync(IList<SelectListItem> items, int? countryId,
        bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (countryId.HasValue)
        {
            //prepare available states and provinces of the country
            var availableStates = await _stateProvinceService.GetStateProvincesByCountryIdAsync(countryId.Value, showHidden: true);
            foreach (var state in availableStates)
            {
                items.Add(new SelectListItem { Value = state.Id.ToString(), Text = state.Name });
            }

            //insert special item for the default value
            if (items.Count > 1)
                await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.SelectState"));
        }

        //insert special item for the default value
        if (!items.Any())
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText ?? await _localizationService.GetResourceAsync("Admin.Address.Other"));
    }

    /// <summary>
    /// Prepare available load plugin modes
    /// </summary>
    /// <param name="items">Load plugin mode items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareLoadPluginModesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available load plugin modes
        var availableLoadPluginModeItems = await LoadPluginsMode.All.ToSelectListAsync(false);
        foreach (var loadPluginModeItem in availableLoadPluginModeItems)
        {
            items.Add(loadPluginModeItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available plugin groups
    /// </summary>
    /// <param name="items">Plugin group items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PreparePluginGroupsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available plugin groups
        var availablePluginGroups = (await _pluginService.GetPluginDescriptorsAsync<IPlugin>(LoadPluginsMode.All))
            .Select(plugin => plugin.Group).Distinct().OrderBy(groupName => groupName).ToList();
        foreach (var group in availablePluginGroups)
            items.Add(new SelectListItem { Value = @group, Text = @group });

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available languages
    /// </summary>
    /// <param name="items">Language items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareLanguagesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available languages
        var availableLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
        foreach (var language in availableLanguages)
        {
            items.Add(new SelectListItem { Value = language.Id.ToString(), Text = language.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available stores
    /// </summary>
    /// <param name="items">Store items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareStoresAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available stores
        var availableStores = await _storeService.GetAllStoresAsync();
        foreach (var store in availableStores)
        {
            items.Add(new SelectListItem { Value = store.Id.ToString(), Text = store.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available customer roles
    /// </summary>
    /// <param name="items">Customer role items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareCustomerRolesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available customer roles
        var availableCustomerRoles = await _customerService.GetAllCustomerRolesAsync();
        foreach (var customerRole in availableCustomerRoles)
        {
            items.Add(new SelectListItem { Value = customerRole.Id.ToString(), Text = customerRole.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available newsletter subscription types
    /// </summary>
    /// <param name="items">Newsletter subscription type items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareSubscriptionTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available newsletter subscription types
        var availableSubscriptionTypes = await _newsLetterSubscriptionTypeService.GetAllNewsLetterSubscriptionTypesAsync();
        foreach (var subscriptionType in availableSubscriptionTypes)
        {
            items.Add(new SelectListItem { Value = subscriptionType.Id.ToString(), Text = subscriptionType.Name });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
    }

    /// <summary>
    /// Prepare available email accounts
    /// </summary>
    /// <param name="items">Email account items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareEmailAccountsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available email accounts
        var availableEmailAccounts = await _emailAccountService.GetAllEmailAccountsAsync();
        foreach (var emailAccount in availableEmailAccounts)
        {
            items.Add(new SelectListItem { Value = emailAccount.Id.ToString(), Text = $"{emailAccount.DisplayName} ({emailAccount.Email})" });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
    }

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareTaxCategoriesAsync (commerce feature)
    //Removed: PrepareCategoriesAsync (commerce feature)
    //Removed: PrepareManufacturersAsync (commerce feature)
    //Removed: PrepareVendorsAsync (commerce feature)
    //Removed: PrepareProductTypesAsync (commerce feature)
    //Removed: PrepareCategoryTemplatesAsync (commerce feature)

    /// <summary>
    /// Prepare available time zones
    /// </summary>
    /// <param name="items">Time zone items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareTimeZonesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available time zones
        var availableTimeZones = _dateTimeHelper.GetSystemTimeZones();
        foreach (var timeZone in availableTimeZones)
        {
            items.Add(new SelectListItem { Value = timeZone.Id, Text = timeZone.DisplayName });
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
    }

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareShoppingCartTypesAsync (commerce feature)
    //Removed: PrepareTaxDisplayTypesAsync (commerce feature)

    /// <summary>
    /// Prepare available currencies
    /// </summary>
    /// <param name="items">Currency items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareDiscountTypesAsync (commerce feature)

    /// <summary>
    /// Prepare available log levels
    /// </summary>
    /// <param name="items">Log level items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PrepareLogLevelsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        //prepare available log levels
        var availableLogLevelItems = await LogLevel.Debug.ToSelectListAsync(false);
        foreach (var logLevelItem in availableLogLevelItems)
        {
            items.Add(logLevelItem);
        }

        //insert special item for the default value
        await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
    }

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareManufacturerTemplatesAsync (commerce feature)

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareReturnRequestStatusesAsync (commerce feature)
    //Removed: PrepareProductTemplatesAsync (commerce feature)

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareWarehousesAsync (commerce feature)
    //Removed: PrepareDeliveryDatesAsync (commerce feature)
    //Removed: PrepareProductAvailabilityRangesAsync (commerce feature)
    //Removed: PrepareGdprRequestTypesAsync (commerce-driven GDPR feature)
    //Removed: PrepareSpecificationAttributeGroupsAsync (commerce feature)

    /// <summary>
    /// Prepare translation supported model
    /// </summary>
    /// <param name="model">Translation supported model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task PreparePreTranslationSupportModelAsync(ITranslationSupportedModel model)
    {
        if (!_translationSettings.AllowPreTranslate)
            return;

        var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
        model.PreTranslationAvailable = allLanguages.Any(l =>
            !_translationSettings.NotTranslateLanguages.Contains(l.Id) &&
            l.Id != _translationSettings.TranslateFromLanguageId);
    }

    #endregion
}