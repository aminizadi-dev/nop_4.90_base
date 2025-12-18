using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Seo;
using Nop.Services.Seo;

namespace Nop.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents the helper implementation to build specific URLs within an application 
/// </summary>
public partial class NopUrlHelper : INopUrlHelper
{
    #region Fields

    protected readonly IActionContextAccessor _actionContextAccessor;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlHelperFactory _urlHelperFactory;
    protected readonly IUrlRecordService _urlRecordService;

    #endregion

    #region Ctor

    public NopUrlHelper(
        IActionContextAccessor actionContextAccessor,
        IStoreContext storeContext,
        IUrlHelperFactory urlHelperFactory,
        IUrlRecordService urlRecordService)
    {
        _actionContextAccessor = actionContextAccessor;
        _storeContext = storeContext;
        _urlHelperFactory = urlHelperFactory;
        _urlRecordService = urlRecordService;
    }

    #endregion

    #region Utilities

    //COMMERCE METHOD REMOVED - Phase B
    //Removed: RouteProductUrlAsync (product URL generation with category/manufacturer support)

    #endregion

    #region Methods

    /// <summary>
    /// Generate a generic URL for the specified entity which supports slug
    /// </summary>
    /// <typeparam name="TEntity">Entity type that supports slug</typeparam>
    /// <param name="entity">An entity which supports slug</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <param name="host">The host name for the URL</param>
    /// <param name="fragment">The fragment for the URL</param>
    /// <param name="languageId">Language identifier; pass null to use the current language</param>
    /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated URL
    /// </returns>
    public virtual async Task<string> RouteGenericUrlAsync<TEntity>(TEntity entity,
        string protocol = null, string host = null, string fragment = null, int? languageId = null, bool ensureTwoPublishedLanguages = true)
        where TEntity : BaseEntity, ISlugSupported
    {
        if (entity is null)
            return string.Empty;

        var seName = await _urlRecordService.GetSeNameAsync(entity, languageId, true, ensureTwoPublishedLanguages);
        return await RouteGenericUrlAsync<TEntity>(new { SeName = seName }, protocol, host, fragment);
    }

    /// <summary>
    /// Generate a generic URL for the specified entity type and route values
    /// </summary>
    /// <typeparam name="TEntity">Entity type that supports slug</typeparam>
    /// <param name="values">An object that contains route values</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <param name="host">The host name for the URL</param>
    /// <param name="fragment">The fragment for the URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated URL
    /// </returns>
    public virtual async Task<string> RouteGenericUrlAsync<TEntity>(object values = null, string protocol = null, string host = null, string fragment = null)
        where TEntity : BaseEntity, ISlugSupported
    {
        return typeof(TEntity) switch
        {
            //COMMERCE ENTITY TYPES REMOVED - Phase B
            //Removed: Product, Category, Manufacturer, Vendor, NewsItem, BlogPost, ProductTag
            //var entityType when entityType == typeof(Topic)
            //    => RouteUrl(NopRoutingDefaults.RouteName.Generic.Topic, values, protocol, host, fragment),
            //var entityType => RouteUrl(entityType.Name, values, protocol, host, fragment)
        };
    }

    /// <summary>
    /// Generate a URL for topic by the specified system name
    /// </summary>
    /// <param name="systemName">Topic system name</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <param name="host">The host name for the URL</param>
    /// <param name="fragment">The fragment for the URL</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the generated URL
    /// </returns>
    public virtual async Task<string> RouteTopicUrlAsync(string systemName, string protocol = null, string host = null, string fragment = null)
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        //var topic = await _topicService.GetTopicBySystemNameAsync(systemName, store.Id);

        //return await RouteGenericUrlAsync(topic, protocol, host, fragment);
        return null;
    }

    /// <summary>
    /// Generate a URL for the specified route name
    /// </summary>
    /// <param name="routeName">The name of the route that is used to generate URL</param>
    /// <param name="values">An object that contains route values</param>
    /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
    /// <param name="host">The host name for the URL</param>
    /// <param name="fragment">The fragment for the URL</param>
    /// <returns>
    /// The generated URL
    /// </returns>
    public virtual string RouteUrl(string routeName, object values = null, string protocol = null, string host = null, string fragment = null)
    {
        if (_actionContextAccessor.ActionContext is null)
            return string.Empty;

        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

        return urlHelper.RouteUrl(routeName, values, protocol, host, fragment);
    }

    #endregion
}