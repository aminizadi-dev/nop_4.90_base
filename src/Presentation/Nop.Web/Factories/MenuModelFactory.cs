using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Menus;
using Nop.Core.Http;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Menus;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Media;
using Nop.Web.Models.Menus;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the menu model factory
/// </summary>
public partial class MenuModelFactory : IMenuModelFactory
{
    #region Fields

    protected readonly IAclService _aclService;
    protected readonly ICustomerService _customerService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IMenuService _menuService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IPictureService _pictureService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly ITopicService _topicService;
    protected readonly IWorkContext _workContext;
    protected readonly MenuSettings _menuSettings;

    #endregion

    #region Ctor

    public MenuModelFactory(IAclService aclService,
        ICustomerService customerService,
        ILocalizationService localizationService,
        ILogger logger,
        IMenuService menuService,
        INopUrlHelper nopUrlHelper,
        IPictureService pictureService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        ITopicService topicService,
        IWorkContext workContext,
        MenuSettings menuSettings)
    {
        _aclService = aclService;
        _customerService = customerService;
        _localizationService = localizationService;
        _logger = logger;
        _menuService = menuService;
        _nopUrlHelper = nopUrlHelper;
        _pictureService = pictureService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _topicService = topicService;
        _workContext = workContext;
        _menuSettings = menuSettings;
    }

    #endregion

    #region Utilities


    /// <summary>
    /// Get topic info with authorization check
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains topic's title and URL
    ///</returns>
    protected virtual async Task<(string title, string url)> GetAuthorizedTopicInfoAsync(MenuItem menuItem)
    {
        var topicId = menuItem.EntityId ?? 0;
        if (topicId == 0)
            return (await _localizationService.GetLocalizedAsync(menuItem, m => m.Title), string.Empty);

        var topic = await _topicService.GetTopicByIdAsync(topicId);

        if (topic is null || !topic.Published ||
            !await _aclService.AuthorizeAsync(topic) ||
            !await _storeMappingService.AuthorizeAsync(topic))
        {
            return (string.Empty, string.Empty);
        }

        var title = await _localizationService.GetLocalizedAsync(topic, m => m.Title);
        var url = await _nopUrlHelper.RouteGenericUrlAsync(topic);

        return (title, url);
    }

    /// <summary>
    /// Prepare picture model
    /// </summary>
    /// <param name="pictureId">Picture identifier</param>
    /// <param name="title">Title</param>
    /// <param name="alternateText">Alternate text</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the picture model
    /// </returns>
    protected virtual async Task<PictureModel> PreparePictureModelAsync(int pictureId, string title, string alternateText)
    {
        var picture = await _pictureService.GetPictureByIdAsync(pictureId);
        (var fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
        (var imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, _menuSettings.GridThumbPictureSize);

        return new PictureModel
        {
            FullSizeImageUrl = fullSizeImageUrl,
            ImageUrl = imageUrl,
            Title = title,
            AlternateText = alternateText
        };
    }

    /// <summary>
    /// Prepare the menu item model
    /// </summary>
    /// <param name="menuItem">Menu item</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the menu item model
    /// </returns>
    protected virtual async Task<MenuItemModel> PrepareMenuItemModelAsync(MenuItem menuItem)
    {
        var entityId = menuItem.EntityId ?? 0;
        var title = string.Empty;
        var url = string.Empty;

        try
        {
            if (menuItem is null || !menuItem.Published || !await _aclService.AuthorizeAsync(menuItem) || !await _storeMappingService.AuthorizeAsync(menuItem))
                throw new NopException("Menu item not found");

            (title, url) = menuItem.MenuItemType switch
            {
                MenuItemType.StandardPage => (await _localizationService.GetLocalizedAsync(menuItem, mi => mi.Title), _nopUrlHelper.RouteUrl(menuItem.RouteName)),
                MenuItemType.TopicPage => await GetAuthorizedTopicInfoAsync(menuItem),
                MenuItemType.CustomLink => (await _localizationService.GetLocalizedAsync(menuItem, mi => mi.Title), menuItem.Url),
                MenuItemType.Text => (await _localizationService.GetLocalizedAsync(menuItem, m => m.Title), string.Empty),
                _ => (await _localizationService.GetLocalizedAsync(menuItem, m => m.Title), menuItem.Url)
            };
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync(ex.Message);
        }

        return new MenuItemModel
        {
            Id = menuItem.Id,
            CssClass = menuItem.CssClass,
            MenuItemType = menuItem.MenuItemType,
            Template = menuItem.Template,
            ParentId = menuItem.ParentId,
            EntityId = entityId,
            MaximumNumberEntities = menuItem.MaximumNumberEntities ?? _menuSettings.MaximumNumberEntities,
            NumberOfSubItemsPerGridElement = menuItem.NumberOfSubItemsPerGridElement ?? _menuSettings.NumberOfSubItemsPerGridElement,
            NumberOfItemsPerGridRow = menuItem.NumberOfItemsPerGridRow ?? _menuSettings.NumberOfItemsPerGridRow,
            Title = title,
            Url = url,
            ChildrenItems = new()
        };
    }

    /// <summary>
    /// Prepare menu item models for provided menu 
    /// </summary>
    /// <param name="menu">Menu</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains menu item models
    /// </returns>
    protected virtual async Task<List<MenuItemModel>> PrepareMenuItemModelsAsync(Menu menu)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var allItems = await _menuService.GetAllMenuItemsAsync(menuId: menu.Id, storeId: store.Id);
        var result = new List<MenuItemModel>();
        var rootItems = allItems.Where(item => item.ParentId == 0);

        foreach (var rootItem in rootItems)
        {
            var rootItemModel = await PrepareMenuItemModelAsync(rootItem);

            if (string.IsNullOrEmpty(rootItemModel.Title) && string.IsNullOrEmpty(rootItemModel.Url))
                continue;

            if (allItems.Any(i => i.ParentId == rootItemModel.Id))
                rootItemModel.Template = MenuItemTemplate.Simple;

            await getItemTree(rootItemModel);

            result.Add(rootItemModel);
        }

        return result;

        async Task getItemTree(MenuItemModel parentItem)
        {
            var children = allItems
                .Where(item => item.ParentId == parentItem.Id);

            foreach (var item in children)
            {
                var menuItemModel = await PrepareMenuItemModelAsync(item);

                if (string.IsNullOrEmpty(menuItemModel.Title) && string.IsNullOrEmpty(menuItemModel.Url))
                    continue;

                await getItemTree(menuItemModel);

                parentItem.ChildrenItems.Add(menuItemModel);
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the menu model
    /// </summary>
    /// <param name="menuType">Menu type</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains list of menu models
    /// </returns>
    public virtual async Task<IList<MenuModel>> PrepareMenuModelsAsync(MenuType menuType)
    {
        var language = await _workContext.GetWorkingLanguageAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
        var store = await _storeContext.GetCurrentStoreAsync();
        var menuCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.MenuByTypeModelKey, menuType, customerRoleIds, store, language);

        return await _staticCacheManager.GetAsync(menuCacheKey, async () =>
        {
            var menus = await _menuService.GetAllMenusAsync(menuType, store.Id);

            return await menus.SelectAwait(async menu => new MenuModel
            {
                Id = menu.Id,
                MenuType = (MenuType)menu.MenuTypeId,
                Name = await _localizationService.GetLocalizedAsync(menu, m => m.Name),
                CssClass = menu.CssClass,
                Items = await PrepareMenuItemModelsAsync(menu)
            }).ToListAsync();
        });
    }

    #endregion
}