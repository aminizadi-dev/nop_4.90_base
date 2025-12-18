using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models.Translation;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the base model factory that implements a most common admin model factories methods
/// </summary>
public partial interface IBaseAdminModelFactory
{
    /// <summary>
    /// Prepare available activity log types
    /// </summary>
    /// <param name="items">Activity log type items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareActivityLogTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

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
    Task PrepareCountriesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

    /// <summary>
    /// Prepare available states and provinces
    /// </summary>
    /// <param name="items">State and province items</param>
    /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareStatesAndProvincesAsync(IList<SelectListItem> items, int? countryId,
        bool withSpecialDefaultItem = true, string defaultItemText = null);

    /// <summary>
    /// Prepare available languages
    /// </summary>
    /// <param name="items">Language items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareLanguagesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

    /// <summary>
    /// Prepare available stores
    /// </summary>
    /// <param name="items">Store items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareStoresAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

    /// <summary>
    /// Prepare available customer roles
    /// </summary>
    /// <param name="items">Customer role items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareCustomerRolesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

    /// <summary>
    /// Prepare available newsletter subscription types
    /// </summary>
    /// <param name="items">Newsletter subscription type items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareSubscriptionTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

    /// <summary>
    /// Prepare available email accounts
    /// </summary>
    /// <param name="items">Email account items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareEmailAccountsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

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
    Task PrepareTimeZonesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareShoppingCartTypesAsync (commerce feature)
    //Removed: PrepareTaxDisplayTypesAsync (commerce feature)

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: PrepareDiscountTypesAsync (commerce feature)

    /// <summary>
    /// Prepare available log levels
    /// </summary>
    /// <param name="items">Log level items</param>
    /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
    /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task PrepareLogLevelsAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null);

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
    Task PreparePreTranslationSupportModelAsync(ITranslationSupportedModel model);
}