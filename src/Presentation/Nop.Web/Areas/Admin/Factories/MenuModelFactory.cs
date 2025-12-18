using System.util;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Menus;
using Nop.Core.Http;
using Nop.Services;
//COMMERCE SERVICES REMOVED - Phase B
//Removed: Catalog, Vendors
using Nop.Services.Localization;
using Nop.Services.Menus;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the menu model factory implementation
/// </summary>
public partial class MenuModelFactory : IMenuModelFactory
{
    #region Fields

    //COMMERCE SETTINGS REMOVED - Phase B
    //Removed: CatalogSettings _catalogSettings
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    //COMMERCE SERVICES REMOVED - Phase B
    //Removed: ICategoryService, IManufacturerService, IProductService, IVendorService
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IMenuService _menuService;
    protected readonly INotificationService _notificationService;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly MenuSettings _menuSettings;

    #endregion

    #region Ctor

    public MenuModelFactory(
        //COMMERCE DEPENDENCIES REMOVED - Phase B
        //Removed: CatalogSettings catalogSettings, ICategoryService categoryService, IManufacturerService manufacturerService, IProductService productService, IVendorService vendorService
        IBaseAdminModelFactory baseAdminModelFactory,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IMenuService menuService,
        INotificationService notificationService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        MenuSettings menuSettings)
    {
        //COMMERCE ASSIGNMENTS REMOVED - Phase B
        //Removed: _catalogSettings, _categoryService, _manufacturerService, _productService, _vendorService
        _baseAdminModelFactory = baseAdminModelFactory;
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
        _menuService = menuService;
        _notificationService = notificationService;
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        _menuSettings = menuSettings;
    }

    #endregion


    

    #region Methods

    #region Menus

    /// <summary>
    /// Prepare menu search model
    /// </summary>
    /// <param name="searchModel">Menu search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu search model
    /// </returns>
    public virtual Task<MenuSearchModel> PrepareMenuSearchModelAsync(MenuSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged menu list model
    /// </summary>
    /// <param name="searchModel">Menu search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu list model
    /// </returns>
    public virtual async Task<MenuListModel> PrepareMenuListModelAsync(MenuSearchModel searchModel)
    {
        var menus = await _menuService.GetAllMenusAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize, showHidden: true);

        //prepare list model
        return await new MenuListModel().PrepareToGridAsync(searchModel, menus, () =>
        {
            //fill in model values from the entity
            return menus.SelectAwait(async menu =>
            {
                var model = menu.ToModel<MenuModel>();
                model.MenuTypeName = await _localizationService.GetLocalizedEnumAsync((MenuType)model.MenuTypeId);

                return model;
            });
        });
    }

    /// <summary>
    /// Prepare menu model
    /// </summary>
    /// <param name="model">Menu model</param>
    /// <param name="menu">Menu</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu model
    /// </returns>
    public virtual async Task<MenuModel> PrepareMenuModelAsync(MenuModel model, Menu menu, bool excludeProperties = false)
    {
        Func<MenuLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (menu != null)
        {
            if (model == null)
                model = menu.ToModel<MenuModel>();

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(menu, entity => entity.Name, languageId, false, false);
            };

            model.MenuItemSearchModel = await PrepareMenuItemSearchModelAsync(new MenuItemSearchModel { MenuId = menu.Id });
        }

        //prepare model stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, menu, excludeProperties);
        model.AvailableMenuTypes = (await MenuType.Footer.ToSelectListAsync(false)).ToList();

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        return model;
    }

    #endregion

    #region Menu items

    /// <summary>
    /// Prepare menu items search model
    /// </summary>
    /// <param name="searchModel">Menu item search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu item search model
    /// </returns>
    public virtual Task<MenuItemSearchModel> PrepareMenuItemSearchModelAsync(MenuItemSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    #endregion

    #endregion
}