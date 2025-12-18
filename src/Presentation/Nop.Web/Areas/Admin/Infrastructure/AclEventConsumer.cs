using Nop.Core;
using Nop.Core.Domain.Menus;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Menus;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Menus;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Infrastructure;

/// <summary>
/// Represents ACL event consumer
/// </summary>
public partial class AclEventConsumer : IConsumer<ModelPreparedEvent<BaseNopModel>>,
    IConsumer<ModelReceivedEvent<BaseNopModel>>,
    IConsumer<EntityInsertedEvent<Menu>>,
    IConsumer<EntityInsertedEvent<MenuItem>>
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly IAclSupportedModelFactory _aclSupportedModelFactory;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMenuService _menuService;

    private static readonly Dictionary<string, IList<int>> _tempData = new(comparer: StringComparer.InvariantCultureIgnoreCase);

    #endregion

    #region Ctor

    public AclEventConsumer(IAclService aclService,
        IAclSupportedModelFactory aclSupportedModelFactory,
        ICustomerService customerService,
        ILocalizationService localizationService,
        IMenuService menuService)
    {
        _aclService = aclService;
        _aclSupportedModelFactory = aclSupportedModelFactory;
        _customerService = customerService;
        _localizationService = localizationService;
        _menuService = menuService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Save ACL mapping
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <param name="key">Key to load mapping</param>
    /// <param name="entity">Entity to store mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SaveStoredDataAsync<TEntity>(string key, TEntity entity) where TEntity : BaseEntity, IAclSupported
    {
        if (!_tempData.ContainsKey(key))
            return;

        await _aclService.SaveAclAsync(entity, _tempData[key]);
        _tempData.Remove(key);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle model prepared event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(ModelPreparedEvent<BaseNopModel> eventMessage)
    {
        if (eventMessage.Model is not IAclSupportedModel)
            return;

        switch (eventMessage.Model)
        {
            case MenuModel menuModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(menuModel, nameof(Menu));
                break;
            case MenuItemModel menuItemModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(menuItemModel, nameof(MenuItem));
                break;
            case CustomerSearchModel customerSearchModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(customerSearchModel);
                break;
            case OnlineCustomerSearchModel onlineCustomerSearchModel:
                await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(onlineCustomerSearchModel);
                break;
        }
    }

    /// <summary>
    /// Handle model received event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(ModelReceivedEvent<BaseNopModel> eventMessage)
    {
        if (eventMessage.Model is not IAclSupportedModel model)
            return;

        var key = string.Empty;

        switch (eventMessage.Model)
        {
            case MenuModel menuModel:
                var menu = await _menuService.GetMenuByIdAsync(menuModel.Id);
                await _aclService.SaveAclAsync(menu, menuModel.SelectedCustomerRoleIds);
                key = menu == null ? menuModel.Name : string.Empty;
                break;

            case MenuItemModel menuItemModel:
                var menuItem = await _menuService.GetMenuItemByIdAsync(menuItemModel.Id);
                await _aclService.SaveAclAsync(menuItem, menuItemModel.SelectedCustomerRoleIds);
                key = menuItem == null ? $"{nameof(MenuItem)}:{menuItemModel.Title}:{menuItemModel.MenuId}:{menuItemModel.MenuItemTypeId}" : string.Empty;
                break;
        }

        if (!string.IsNullOrEmpty(key))
            _tempData[key] = model.SelectedCustomerRoleIds;
    }

    /// <summary>
    /// Handle menu inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Menu> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync(entity.Name, entity);
    }

    /// <summary>
    /// Handle menu item inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<MenuItem> eventMessage)
    {
        var entity = eventMessage.Entity;
        await SaveStoredDataAsync($"{nameof(MenuItem)}:{entity.Title}:{entity.MenuId}:{entity.MenuItemTypeId}", entity);
    }

    #endregion
}