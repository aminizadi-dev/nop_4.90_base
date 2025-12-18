using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Menus;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Web.Framework.Models.Cms;

namespace Nop.Web.Infrastructure.Cache;

/// <summary>
/// Model cache event consumer (used for caching of presentation layer models)
/// </summary>
public partial class ModelCacheEventConsumer :
    //languages
    IConsumer<EntityInsertedEvent<Language>>,
    IConsumer<EntityUpdatedEvent<Language>>,
    IConsumer<EntityDeletedEvent<Language>>,
    //settings
    IConsumer<EntityUpdatedEvent<Setting>>,
    //Picture
    IConsumer<EntityInsertedEvent<Picture>>,
    IConsumer<EntityUpdatedEvent<Picture>>,
    IConsumer<EntityDeletedEvent<Picture>>,
    //menus
    IConsumer<EntityInsertedEvent<Menu>>,
    IConsumer<EntityUpdatedEvent<Menu>>,
    IConsumer<EntityDeletedEvent<Menu>>,
    IConsumer<EntityInsertedEvent<MenuItem>>,
    IConsumer<EntityUpdatedEvent<MenuItem>>,
    IConsumer<EntityDeletedEvent<MenuItem>>

{
    #region Fields

    protected readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public ModelCacheEventConsumer(IStaticCacheManager staticCacheManager)
    {
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    #region Languages

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Language> eventMessage)
    {
        //clear all localizable models (infrastructure only - removed commerce-specific cache keys)
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityUpdatedEvent<Language> eventMessage)
    {
        //clear all localizable models (infrastructure only - removed commerce-specific cache keys)
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityDeletedEvent<Language> eventMessage)
    {
        //clear all localizable models (infrastructure only - removed commerce-specific cache keys)
    }

    #endregion

    #region Setting

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
    {
        //clear models which depend on settings
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.SitemapPrefixCacheKey); //depends on distinct sitemap settings
        await _staticCacheManager.RemoveByPrefixAsync(WidgetModelDefaults.WidgetPrefixCacheKey); //depends on WidgetSettings and certain settings of widgets
    }

    #endregion

    #region Pictures

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Picture> eventMessage)
    {
        // Clear picture-related caches (infrastructure only - removed commerce-specific cache keys)
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityUpdatedEvent<Picture> eventMessage)
    {
        // Clear picture-related caches (infrastructure only - removed commerce-specific cache keys)
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityDeletedEvent<Picture> eventMessage)
    {
        // Clear picture-related caches (infrastructure only - removed commerce-specific cache keys)
    }

    #endregion

    #region Menus

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<Menu> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.MenuPrefixCacheKey);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityUpdatedEvent<Menu> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.MenuPrefixCacheKey);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityDeletedEvent<Menu> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.MenuPrefixCacheKey);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityInsertedEvent<MenuItem> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.MenuPrefixCacheKey);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityUpdatedEvent<MenuItem> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.MenuPrefixCacheKey);
    }

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityDeletedEvent<MenuItem> eventMessage)
    {
        await _staticCacheManager.RemoveByPrefixAsync(NopModelCacheDefaults.MenuPrefixCacheKey);
    }

    #endregion

    #endregion
}