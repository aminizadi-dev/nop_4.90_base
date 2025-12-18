using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Events;

namespace Nop.Web.Framework.Mvc.Routing;

/// <summary>
/// Represents slug route transformer
/// </summary>
public partial class SlugRouteTransformer : DynamicRouteValueTransformer
{
    #region Fields

    protected readonly IEventPublisher _eventPublisher;
    protected readonly ILanguageService _languageService;
    protected readonly IStoreContext _storeContext;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly LocalizationSettings _localizationSettings;

    #endregion

    #region Ctor

    public SlugRouteTransformer(
        IEventPublisher eventPublisher,
        ILanguageService languageService,
        IStoreContext storeContext,
        IUrlRecordService urlRecordService,
        LocalizationSettings localizationSettings)
    {
        _eventPublisher = eventPublisher;
        _languageService = languageService;
        _storeContext = storeContext;
        _urlRecordService = urlRecordService;
        _localizationSettings = localizationSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Transform route values according to the passed URL record
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="values">The route values associated with the current match</param>
    /// <param name="urlRecord">Record found by the URL slug</param>
    /// <param name="catalogPath">URL catalog path</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SingleSlugRoutingAsync(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord, string catalogPath)
    {
        //if URL record is not active let's find the latest one
        var slug = urlRecord.IsActive
            ? urlRecord.Slug
            : await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
        if (string.IsNullOrEmpty(slug))
            return;

        if (!urlRecord.IsActive || !string.IsNullOrEmpty(catalogPath))
        {
            //permanent redirect to new URL with active single slug
            InternalRedirect(httpContext, values, $"/{slug}", true);
            return;
        }

        //Ensure that the slug is the same for the current language, 
        //otherwise it can cause some issues when customers choose a new language but a slug stays the same
        if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && values.TryGetValue(NopRoutingDefaults.RouteValue.Language, out var langValue))
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var languages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);
            var language = languages
                .FirstOrDefault(lang => lang.UniqueSeoCode.Equals(langValue?.ToString(), StringComparison.InvariantCultureIgnoreCase))
                ?? languages.FirstOrDefault();

            var slugLocalized = await _urlRecordService.GetSeNameAsync(urlRecord.EntityId, urlRecord.EntityName, language.Id, true, false);
            if (!string.IsNullOrEmpty(slugLocalized) && !slugLocalized.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
            {
                //redirect to the page for current language
                InternalRedirect(httpContext, values, $"/{language.UniqueSeoCode}/{slugLocalized}", false);
                return;
            }
        }
    }

    /// <summary>
    /// Transform route values to redirect the request
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="values">The route values associated with the current match</param>
    /// <param name="path">Path</param>
    /// <param name="permanent">Whether the redirect should be permanent</param>
    protected virtual void InternalRedirect(HttpContext httpContext, RouteValueDictionary values, string path, bool permanent)
    {
        values[NopRoutingDefaults.RouteValue.Controller] = "Common";
        values[NopRoutingDefaults.RouteValue.Action] = "InternalRedirect";
        values[NopRoutingDefaults.RouteValue.Url] = $"{httpContext.Request.PathBase}{path}{httpContext.Request.QueryString}";
        values[NopRoutingDefaults.RouteValue.PermanentRedirect] = permanent;
        httpContext.Items[NopHttpDefaults.GenericRouteInternalRedirect] = true;
    }

    /// <summary>
    /// Transform route values to set controller, action and action parameters
    /// </summary>
    /// <param name="values">The route values associated with the current match</param>
    /// <param name="controller">Controller name</param>
    /// <param name="action">Action name</param>
    /// <param name="slug">URL slug</param>
    /// <param name="parameters">Action parameters</param>
    protected virtual void RouteToAction(RouteValueDictionary values, string controller, string action, string slug, params (string Key, object Value)[] parameters)
    {
        values[NopRoutingDefaults.RouteValue.Controller] = controller;
        values[NopRoutingDefaults.RouteValue.Action] = action;
        values[NopRoutingDefaults.RouteValue.SeName] = slug;
        foreach (var (key, value) in parameters)
        {
            values[key] = value;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create a set of transformed route values that will be used to select an action
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="routeValues">The route values associated with the current match</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the set of values
    /// </returns>
    public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary routeValues)
    {
        //get values to transform for action selection
        var values = new RouteValueDictionary(routeValues);

        if (!values.TryGetValue(NopRoutingDefaults.RouteValue.SeName, out var slug) || slug is null)
            return values;

        //find record by the URL slug
        if (await _urlRecordService.GetBySlugAsync(slug.ToString()) is not UrlRecord urlRecord)
            return values;

        //allow third-party handlers to select an action by the found URL record
        var routingEvent = new GenericRoutingEvent(httpContext, values, urlRecord);
        await _eventPublisher.PublishAsync(routingEvent);
        if (routingEvent.StopProcessing)
            return values;

        //select an action by the URL record only
        var catalogPath = values.TryGetValue(NopRoutingDefaults.RouteValue.CatalogSeName, out var catalogPathValue)
            ? catalogPathValue.ToString()
            : string.Empty;
        await SingleSlugRoutingAsync(httpContext, values, urlRecord, catalogPath);

        return values;
    }

    #endregion
}