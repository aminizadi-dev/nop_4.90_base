using Nop.Core.Domain.Menus;
using Nop.Web.Areas.Admin.Models.Menus;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the menu model factory
/// </summary>
public partial interface IMenuModelFactory
{
    #region Menus

    /// <summary>
    /// Prepare menu search model
    /// </summary>
    /// <param name="searchModel">Menu search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu search model
    /// </returns>
    Task<MenuSearchModel> PrepareMenuSearchModelAsync(MenuSearchModel searchModel);

    /// <summary>
    /// Prepare paged menu list model
    /// </summary>
    /// <param name="searchModel">Menu search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu list model
    /// </returns>
    Task<MenuListModel> PrepareMenuListModelAsync(MenuSearchModel searchModel);

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
    Task<MenuModel> PrepareMenuModelAsync(MenuModel model, Menu menu, bool excludeProperties = false);

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
    Task<MenuItemSearchModel> PrepareMenuItemSearchModelAsync(MenuItemSearchModel searchModel);


    #endregion
}