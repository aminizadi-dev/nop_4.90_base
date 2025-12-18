using Nop.Web.Areas.Admin.Models.Settings;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the setting model factory
/// </summary>
public partial interface ISettingModelFactory
{
    /// <summary>
    /// Prepare app settings model
    /// </summary>
    /// <param name="model">AppSettings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the app settings model
    /// </returns>
    Task<AppSettingsModel> PrepareAppSettingsModel(AppSettingsModel model = null);

    //COMMERCE FEATURES REMOVED - Phase C
    //Removed: PrepareBlogSettingsModelAsync (blog feature)
    //Removed: PrepareVendorSettingsModelAsync (vendor feature)
    //Removed: PrepareForumSettingsModelAsync (forum feature)
    //Removed: PrepareNewsSettingsModelAsync (news feature)
    //Removed: PrepareShippingSettingsModelAsync (shipping feature)
    //Removed: PrepareTaxSettingsModelAsync (tax feature)
    //Removed: PrepareCatalogSettingsModelAsync (catalog feature)
    //Removed: PrepareFilterLevelSettingsModelAsync (filter level feature)
    //Removed: PrepareFilterLevelListModelAsync (filter level feature)
    //Removed: PrepareFilterLevelModelAsync (filter level feature)

    //COMMERCE FEATURES REMOVED - Phase C
    //Removed: PrepareRewardPointsSettingsModelAsync (reward points feature)
    //Removed: PrepareOrderSettingsModelAsync (order feature)
    //Removed: PrepareShoppingCartSettingsModelAsync (shopping cart feature)

    /// <summary>
    /// Prepare media settings model
    /// </summary>
    /// <param name="model">Media settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the media settings model
    /// </returns>
    Task<MediaSettingsModel> PrepareMediaSettingsModelAsync(MediaSettingsModel model = null);

    /// <summary>
    /// Prepare customer user settings model
    /// </summary>
    /// <param name="model">Customer user settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer user settings model
    /// </returns>
    Task<CustomerUserSettingsModel> PrepareCustomerUserSettingsModelAsync(CustomerUserSettingsModel model = null);

    //COMMERCE/ADDITIONAL FEATURES REMOVED - Phase C
    //Removed: PrepareGdprSettingsModelAsync (GDPR feature)
    //Removed: PrepareGdprConsentListModelAsync (GDPR feature)
    //Removed: PrepareGdprConsentModelAsync (GDPR feature)

    /// <summary>
    /// Prepare general and common settings model
    /// </summary>
    /// <param name="model">General common settings model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the general and common settings model
    /// </returns>
    Task<GeneralCommonSettingsModel> PrepareGeneralCommonSettingsModelAsync(GeneralCommonSettingsModel model = null);

    //COMMERCE FEATURES REMOVED - Phase C
    //Removed: PrepareProductEditorSettingsModelAsync (product editor feature)

    /// <summary>
    /// Prepare setting search model
    /// </summary>
    /// <param name="searchModel">Setting search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting search model
    /// </returns>
    Task<SettingSearchModel> PrepareSettingSearchModelAsync(SettingSearchModel searchModel);

    /// <summary>
    /// Prepare paged setting list model
    /// </summary>
    /// <param name="searchModel">Setting search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting list model
    /// </returns>
    Task<SettingListModel> PrepareSettingListModelAsync(SettingSearchModel searchModel);

    /// <summary>
    /// Prepare setting mode model
    /// </summary>
    /// <param name="modeName">Mode name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the setting mode model
    /// </returns>
    Task<SettingModeModel> PrepareSettingModeModelAsync(string modeName);

    /// <summary>
    /// Prepare store scope configuration model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store scope configuration model
    /// </returns>
    Task<StoreScopeConfigurationModel> PrepareStoreScopeConfigurationModelAsync();
}