using System.Globalization;
using System.Text;
using System.Xml;
using Nop.Core;
using Nop.Core.Caching;
//COMMERCE DOMAIN REMOVED - Phase C
//Removed: using Nop.Core.Domain.Blogs;
//Removed: using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
//Removed: using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
//Removed: using Nop.Core.Domain.News;
using Nop.Core.Domain.Seo;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Infrastructure;
//COMMERCE SERVICES REMOVED - Phase C
//Removed: using Nop.Services.Blogs;
//Removed: using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
//Removed: using Nop.Services.News;
using Nop.Services.Seo;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Sitemap;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the sitemap model factory implementation
/// </summary>
public partial class SitemapModelFactory : ISitemapModelFactory
{
    #region Fields

    //COMMERCE SETTINGS/SERVICES REMOVED - Phase C
    //Removed: protected readonly BlogSettings _blogSettings;
    //Removed: protected readonly ForumSettings _forumSettings;
    //Removed: protected readonly IBlogService _blogService;
    //Removed: protected readonly ICategoryService _categoryService;
    protected readonly ICustomerService _customerService;
    protected readonly IEventPublisher _eventPublisher;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocker _locker;
    //Removed: protected readonly IManufacturerService _manufacturerService;
    //Removed: protected readonly INewsService _newsService;
    protected readonly INopFileProvider _nopFileProvider;
    protected readonly INopUrlHelper _nopUrlHelper;
    //Removed: protected readonly IProductService _productService;
    //Removed: protected readonly IProductTagService _productTagService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly LocalizationSettings _localizationSettings;
    //Removed: protected readonly NewsSettings _newsSettings;
    protected readonly SitemapSettings _sitemapSettings;
    protected readonly SitemapXmlSettings _sitemapXmlSettings;

    #endregion

    #region Ctor

    public SitemapModelFactory(
        //COMMERCE SETTINGS/SERVICES REMOVED - Phase C
        //Removed: BlogSettings blogSettings,
        //Removed: ForumSettings forumSettings,
        //Removed: IBlogService blogService,
        //Removed: ICategoryService categoryService,
        ICustomerService customerService,
        IEventPublisher eventPublisher,
        IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ILocker locker,
        //Removed: IManufacturerService manufacturerService,
        //Removed: INewsService newsService,
        INopFileProvider nopFileProvider,
        INopUrlHelper nopUrlHelper,
        //Removed: IProductService productService,
        //Removed: IProductTagService productTagService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IWebHelper webHelper,
        IWorkContext workContext,
        LocalizationSettings localizationSettings,
        //Removed: NewsSettings newsSettings,
        SitemapSettings sitemapSettings,
        SitemapXmlSettings sitemapXmlSettings)
    {
        //COMMERCE SETTINGS/SERVICES REMOVED - Phase C
        //Removed: _blogSettings = blogSettings;
        //Removed: _forumSettings = forumSettings;
        //Removed: _blogService = blogService;
        //Removed: _categoryService = categoryService;
        _customerService = customerService;
        _eventPublisher = eventPublisher;
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
        _localizationService = localizationService;
        _locker = locker;
        //Removed: _manufacturerService = manufacturerService;
        //Removed: _newsService = newsService;
        _nopFileProvider = nopFileProvider;
        _nopUrlHelper = nopUrlHelper;
        //Removed: _productService = productService;
        //Removed: _productTagService = productTagService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _webHelper = webHelper;
        _workContext = workContext;
        _localizationSettings = localizationSettings;
        //Removed: _newsSettings = newsSettings;
        _sitemapSettings = sitemapSettings;
        _sitemapXmlSettings = sitemapXmlSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get HTTP protocol
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the protocol name as string
    /// </returns>
    protected virtual async Task<string> GetHttpProtocolAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();

        return store.SslEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
    }

    /// <summary>
    /// Generate URLs for the sitemap
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of sitemap URLs
    /// </returns>
    protected virtual async Task<IList<SitemapUrlModel>> GenerateUrlsAsync()
    {
        var sitemapUrls = new List<SitemapUrlModel>
        {
            //home page
            await PrepareLocalizedSitemapUrlAsync(NopRouteNames.General.HOMEPAGE),

            //contact us
            await PrepareLocalizedSitemapUrlAsync(NopRouteNames.General.CONTACT_US)
        };

        //COMMERCE URLS REMOVED - Phase C
        //Removed: search products, news, blog, forum, categories, manufacturers, products, product tags

        //custom URLs
        if (_sitemapXmlSettings.SitemapXmlIncludeCustomUrls)
            sitemapUrls.AddRange(GetCustomUrls());

        //event notification
        await _eventPublisher.PublishAsync(new SitemapCreatedEvent(sitemapUrls));

        return sitemapUrls;
    }

    //COMMERCE METHODS REMOVED - Phase C
    //Removed: GetNewsItemUrlsAsync (news - commerce feature)
    //Removed: GetCategoryUrlsAsync (categories - commerce feature)
    //Removed: GetManufacturerUrlsAsync (manufacturers - commerce feature)
    //Removed: GetProductUrlsAsync (products - commerce feature)
    //Removed: GetProductTagUrlsAsync (product tags - commerce feature)

    //COMMERCE METHOD REMOVED - Phase C
    //Removed: GetBlogPostUrlsAsync (blog posts - commerce feature)

    /// <summary>
    /// Get custom URLs for the sitemap
    /// </summary>
    /// <returns>Sitemap URLs</returns>
    protected virtual IEnumerable<SitemapUrlModel> GetCustomUrls()
    {
        var storeLocation = _webHelper.GetStoreLocation();

        return _sitemapXmlSettings.SitemapCustomUrls.Select(customUrl =>
            new SitemapUrlModel(string.Concat(storeLocation, customUrl), new List<string>(), UpdateFrequency.Weekly, DateTime.UtcNow));
    }

    /// <summary>
    /// Write sitemap index file into the stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="sitemapNumber">The number of sitemaps</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task WriteSitemapIndexAsync(MemoryStream stream, int sitemapNumber)
    {
        await using var writer = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Async = true,
            Encoding = Encoding.UTF8,
            Indent = true,
            ConformanceLevel = ConformanceLevel.Auto
        });

        await writer.WriteStartDocumentAsync();
        await writer.WriteStartElementAsync(prefix: null, localName: "sitemapindex", ns: "http://www.sitemaps.org/schemas/sitemap/0.9");

        //write URLs of all available sitemaps
        for (var id = 1; id <= sitemapNumber; id++)
        {
            var url = _nopUrlHelper.RouteUrl(NopRouteNames.Standard.SITEMAP_INDEXED_XML, new { Id = id }, await GetHttpProtocolAsync());
            var location = await XmlHelper.XmlEncodeAsync(url);

            await writer.WriteStartElementAsync(null, "sitemap", null);
            await writer.WriteElementStringAsync(null, "loc", null, location);
            await writer.WriteElementStringAsync(null, "lastmod", null, DateTime.UtcNow.ToString(NopSeoDefaults.SitemapDateFormat));
            await writer.WriteEndElementAsync();
        }

        await writer.WriteEndElementAsync();
    }

    /// <summary>
    /// Write sitemap file into the stream
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="sitemapUrls">List of sitemap URLs</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task WriteSitemapAsync(MemoryStream stream, IList<SitemapUrlModel> sitemapUrls)
    {
        await using var writer = XmlWriter.Create(stream, new XmlWriterSettings
        {
            Async = true,
            Encoding = Encoding.UTF8,
            Indent = true,
            ConformanceLevel = ConformanceLevel.Auto
        });

        await writer.WriteStartDocumentAsync();
        await writer.WriteStartElementAsync(prefix: null, localName: "urlset", ns: "http://www.sitemaps.org/schemas/sitemap/0.9");
        await writer.WriteAttributeStringAsync(prefix: "xsi", "schemaLocation",
            "http://www.w3.org/2001/XMLSchema-instance",
            "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd http://www.w3.org/1999/xhtml http://www.w3.org/2002/08/xhtml/xhtml1-strict.xsd");
        await writer.WriteAttributeStringAsync(prefix: "xmlns", "xhtml", null, "http://www.w3.org/1999/xhtml");

        //write URLs from list to the sitemap
        foreach (var sitemapUrl in sitemapUrls)
        {
            //write base url
            await WriteSitemapUrlAsync(writer, sitemapUrl);

            //write all alternate url if exists
            foreach (var alternate in sitemapUrl.AlternateLocations
                         .Where(p => !p.Equals(sitemapUrl.Location, StringComparison.InvariantCultureIgnoreCase)))
            {
                await WriteSitemapUrlAsync(writer, new SitemapUrlModel(alternate, sitemapUrl));
            }
        }

        await writer.WriteEndElementAsync();
    }

    /// <summary>
    /// Write sitemap
    /// </summary>
    /// <param name="writer">XML stream writer</param>
    /// <param name="sitemapUrl">Sitemap URL</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task WriteSitemapUrlAsync(XmlWriter writer, SitemapUrlModel sitemapUrl)
    {
        if (string.IsNullOrEmpty(sitemapUrl.Location))
            return;

        await writer.WriteStartElementAsync(null, "url", null);

        var loc = await XmlHelper.XmlEncodeAsync(sitemapUrl.Location);
        await writer.WriteElementStringAsync(null, "loc", null, loc);

        //write all related url
        foreach (var alternate in sitemapUrl.AlternateLocations)
        {
            if (string.IsNullOrEmpty(alternate))
                continue;

            //extract seo code
            var altLoc = await XmlHelper.XmlEncodeAsync(alternate);
            var altLocPath = new Uri(altLoc).PathAndQuery;
            var (_, lang) = await altLocPath.IsLocalizedUrlAsync(_httpContextAccessor.HttpContext.Request.PathBase, true);

            if (string.IsNullOrEmpty(lang?.UniqueSeoCode))
                continue;

            await writer.WriteStartElementAsync("xhtml", "link", null);
            await writer.WriteAttributeStringAsync(null, "rel", null, "alternate");
            await writer.WriteAttributeStringAsync(null, "hreflang", null, lang.UniqueSeoCode);
            await writer.WriteAttributeStringAsync(null, "href", null, altLoc);
            await writer.WriteEndElementAsync();
        }

        await writer.WriteElementStringAsync(null, "changefreq", null, sitemapUrl.UpdateFrequency.ToString().ToLowerInvariant());
        await writer.WriteElementStringAsync(null, "lastmod", null, sitemapUrl.UpdatedOn.ToString(NopSeoDefaults.SitemapDateFormat, CultureInfo.InvariantCulture));
        await writer.WriteEndElementAsync();
    }

    /// <summary>
    /// This will build an XML sitemap for better index with search engines.
    /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
    /// </summary>
    /// <param name="fullPath">The path and name of the sitemap file</param>
    /// <param name="id">Sitemap identifier</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task GenerateAsync(string fullPath, int id = 0)
    {
        //generate all URLs for the sitemap
        var sitemapUrls = await GenerateUrlsAsync();

        //split URLs into separate lists based on the max size
        var numberOfParts = (int)Math.Ceiling((decimal)sitemapUrls.Sum(x => (x.AlternateLocations?.Count ?? 0) + 1) / NopSeoDefaults.SitemapMaxUrlNumber);

        var sitemaps = sitemapUrls
            .Select((url, index) => new { Index = index, Value = url })
            .GroupBy(group => group.Index % numberOfParts)
            .Select(group => group
                .Select(url => url.Value)
                .ToList()).ToList();

        if (!sitemaps.Any())
            return;

        await using var stream = new MemoryStream();

        if (id > 0)
        {
            //requested sitemap does not exist
            if (id > sitemaps.Count)
                return;

            //otherwise write a certain numbered sitemap file into the stream
            await WriteSitemapAsync(stream, sitemaps.ElementAt(id - 1));
        }
        else
        {
            //URLs more than the maximum allowable, so generate a sitemap index file
            if (numberOfParts > 1)
            {
                //write a sitemap index file into the stream
                await WriteSitemapIndexAsync(stream, sitemaps.Count);
            }
            else
            {
                //otherwise generate a standard sitemap
                await WriteSitemapAsync(stream, sitemaps.First());
            }
        }

        if (_nopFileProvider.FileExists(fullPath))
            _nopFileProvider.DeleteFile(fullPath);


        using var fileStream = _nopFileProvider.GetOrCreateFile(fullPath);
        stream.Position = 0;
        await stream.CopyToAsync(fileStream, 81920);
    }

    /// <summary>
    /// Gets localized URL with SEO code
    /// </summary>
    /// <param name="currentUrl">URL to add SEO code</param>
    /// <param name="lang">Language for localization</param>
    /// <returns>Localized URL with SEO code</returns>
    protected virtual string GetLocalizedUrl(string currentUrl, Language lang)
    {
        if (string.IsNullOrEmpty(currentUrl))
            return null;

        var pathBase = _httpContextAccessor.HttpContext.Request.PathBase;

        //Extract server and path from url
        var scheme = new Uri(currentUrl).GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
        var path = new Uri(currentUrl).PathAndQuery;

        //Replace seo code
        var localizedPath = path
            .RemoveLanguageSeoCodeFromUrl(pathBase, true)
            .AddLanguageSeoCodeToUrl(pathBase, true, lang);

        return new Uri(new Uri(scheme), localizedPath).ToString();
    }

    /// <summary>
    /// Return localized urls
    /// </summary>
    /// <param name="entity">An entity which supports slug</param>
    /// <param name="dateTimeUpdatedOn">A time when URL was updated last time</param>
    /// <param name="updateFreq">How often to update url</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<SitemapUrlModel> PrepareLocalizedSitemapUrlAsync<TEntity>(TEntity entity,
        DateTime? dateTimeUpdatedOn = null,
        UpdateFrequency updateFreq = UpdateFrequency.Weekly) where TEntity : BaseEntity, ISlugSupported
    {
        var url = await _nopUrlHelper
            .RouteGenericUrlAsync(entity, await GetHttpProtocolAsync());

        var store = await _storeContext.GetCurrentStoreAsync();

        var updatedOn = dateTimeUpdatedOn ?? DateTime.UtcNow;
        var languages = _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled
            ? await _languageService.GetAllLanguagesAsync(storeId: store.Id)
            : null;

        if (languages == null || languages.Count == 1)
            return new SitemapUrlModel(url, new List<string>(), updateFreq, updatedOn);

        //return list of localized urls
        var localizedUrls = await languages
            .SelectAwait(async lang =>
            {
                var currentUrl = await _nopUrlHelper.RouteGenericUrlAsync(entity, await GetHttpProtocolAsync(), languageId: lang.Id);
                return GetLocalizedUrl(currentUrl, lang);
            })
            .Where(value => !string.IsNullOrEmpty(value))
            .ToListAsync();

        return new SitemapUrlModel(url, localizedUrls, updateFreq, updatedOn);
    }

    /// <summary>
    /// Return localized urls
    /// </summary>
    /// <param name="routeName">Route name</param>
    /// <param name="dateTimeUpdatedOn">A time when URL was updated last time</param>
    /// <param name="updateFreq">How often to update url</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<SitemapUrlModel> PrepareLocalizedSitemapUrlAsync(string routeName,
        DateTime? dateTimeUpdatedOn = null,
        UpdateFrequency updateFreq = UpdateFrequency.Weekly)
    {
        var url = _nopUrlHelper.RouteUrl(routeName, null, await GetHttpProtocolAsync());

        var store = await _storeContext.GetCurrentStoreAsync();

        var updatedOn = dateTimeUpdatedOn ?? DateTime.UtcNow;
        var languages = _localizationSettings.SeoFriendlyUrlsForLanguagesEnabled
            ? await _languageService.GetAllLanguagesAsync(storeId: store.Id)
            : null;

        if (languages == null || languages.Count == 1)
            return new SitemapUrlModel(url, new List<string>(), updateFreq, updatedOn);

        //return list of localized urls
        var localizedUrls = await languages
            .Select(lang => GetLocalizedUrl(url, lang))
            .Where(value => !string.IsNullOrEmpty(value))
            .ToListAsync();

        return new SitemapUrlModel(url, localizedUrls, updateFreq, updatedOn);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the sitemap model
    /// </summary>
    /// <param name="pageModel">Sitemap page model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap model
    /// </returns>
    public virtual async Task<SitemapModel> PrepareSitemapModelAsync(SitemapPageModel pageModel)
    {
        ArgumentNullException.ThrowIfNull(pageModel);

        var language = await _workContext.GetWorkingLanguageAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var customerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer);
        var store = await _storeContext.GetCurrentStoreAsync();
        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.SitemapPageModelKey,
            language, customerRoleIds, store);

        var cachedModel = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var model = new SitemapModel();

            //prepare common items
            var commonGroupTitle = await _localizationService.GetResourceAsync("Sitemap.General");

            //home page
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
                GroupTitle = commonGroupTitle,
                Name = await _localizationService.GetResourceAsync("Homepage"),
                Url = _nopUrlHelper.RouteUrl(NopRouteNames.General.HOMEPAGE)
            });

            //COMMERCE URLS REMOVED - Phase C
            //Removed: search, news, blog, forums

            //contact us
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
                GroupTitle = commonGroupTitle,
                Name = await _localizationService.GetResourceAsync("ContactUs"),
                Url = _nopUrlHelper.RouteUrl(NopRouteNames.General.CONTACT_US)
            });

            //customer info
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
                GroupTitle = commonGroupTitle,
                Name = await _localizationService.GetResourceAsync("Account.MyAccount"),
                Url = _nopUrlHelper.RouteUrl(NopRouteNames.General.CUSTOMER_INFO)
            });

            //COMMERCE ITEMS REMOVED - Phase C
            //Removed: blog posts, news, categories, manufacturers, products, product tags

            return model;
        });

        //prepare model with pagination
        pageModel.PageSize = Math.Max(pageModel.PageSize, _sitemapSettings.SitemapPageSize);
        pageModel.PageNumber = Math.Max(pageModel.PageNumber, 1);

        var pagedItems = new PagedList<SitemapModel.SitemapItemModel>(cachedModel.Items, pageModel.PageNumber - 1, pageModel.PageSize);
        var sitemapModel = new SitemapModel { Items = pagedItems };
        sitemapModel.PageModel.LoadPagedList(pagedItems);

        return sitemapModel;
    }

    /// <summary>
    /// Prepare sitemap model.
    /// This will build an XML sitemap for better index with search engines.
    /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
    /// </summary>
    /// <param name="id">Sitemap identifier</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the sitemap model with sitemap.xml as string
    /// </returns>
    public virtual async Task<SitemapXmlModel> PrepareSitemapXmlModelAsync(int id = 0)
    {
        var language = await _workContext.GetWorkingLanguageAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        var fileName = string.Format(NopSeoDefaults.SitemapXmlFilePattern, store.Id, language.Id, id);
        var fullPath = _nopFileProvider.GetAbsolutePath(NopSeoDefaults.SitemapXmlDirectory, fileName);

        if (_nopFileProvider.FileExists(fullPath) && _nopFileProvider.GetLastWriteTimeUtc(fullPath) > DateTime.UtcNow.AddHours(-_sitemapXmlSettings.RebuildSitemapXmlAfterHours))
        {
            return new SitemapXmlModel { SitemapXmlPath = fullPath };
        }

        //execute task with lock
        if (!await _locker.PerformActionWithLockAsync(
                fullPath,
                TimeSpan.FromSeconds(_sitemapXmlSettings.SitemapBuildOperationDelay),
                async () => await GenerateAsync(fullPath, id)))
        {
            throw new InvalidOperationException();
        }

        return new SitemapXmlModel { SitemapXmlPath = fullPath };
    }

    #endregion
}