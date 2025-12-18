using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Models.DataTables;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the home models factory implementation
/// </summary>
public partial class HomeModelFactory : IHomeModelFactory
{
    #region Fields

    protected readonly AdminAreaSettings _adminAreaSettings;
    protected readonly ICommonModelFactory _commonModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly ISettingService _settingService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IWorkContext _workContext;
    protected readonly NopHttpClient _nopHttpClient;

    #endregion

    #region Ctor

    public HomeModelFactory(AdminAreaSettings adminAreaSettings,
        ICommonModelFactory commonModelFactory,
        ILocalizationService localizationService,
        ILogger logger,
        ISettingService settingService,
        IStaticCacheManager staticCacheManager,
        IWorkContext workContext,
        NopHttpClient nopHttpClient)
    {
        _adminAreaSettings = adminAreaSettings;
        _commonModelFactory = commonModelFactory;
        _localizationService = localizationService;
        _logger = logger;
        _settingService = settingService;
        _staticCacheManager = staticCacheManager;
        _workContext = workContext;
        _nopHttpClient = nopHttpClient;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare dashboard model
    /// </summary>
    /// <param name="model">Dashboard model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    public virtual async Task<DashboardModel> PrepareDashboardModelAsync(DashboardModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return model;
    }

    /// <summary>
    /// Prepare popular search term report model
    /// </summary>
    /// <param name="model">DataTables model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the dashboard model
    /// </returns>
    public virtual async Task<DataTablesModel> PreparePopularSearchTermReportModelAsync(DataTablesModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var searchModel = new PopularSearchTermSearchModel();
        searchModel = await _commonModelFactory.PreparePopularSearchTermSearchModelAsync(searchModel);

        model.Name = "search-term-report-grid";
        model.UrlRead = new DataUrl("PopularSearchTermsReport", "Common", null);
        model.Length = searchModel.Length;
        model.LengthMenu = searchModel.AvailablePageSizes;
        model.RefreshButton = false;
        model.ColumnCollection = new List<ColumnProperty>
        {
            new(nameof(PopularSearchTermModel.Keyword))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SearchTermReport.Keyword")
            },
            new(nameof(PopularSearchTermModel.Count))
            {
                Title = await _localizationService.GetResourceAsync("Admin.SearchTermReport.Count")
            }
        };

        return model;
    }

    /// <summary>
    /// Prepare nopCommerce news model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the nopCommerce news model
    /// </returns>
    public virtual async Task<NopCommerceNewsModel> PrepareNopCommerceNewsModelAsync()
    {
        var model = new NopCommerceNewsModel
        {
            HideAdvertisements = _adminAreaSettings.HideAdvertisementsOnAdminArea
        };

        try
        {
            //try to get news RSS feed
            var rssData = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.OfficialNewsModelKey), async () =>
            {
                try
                {
                    return await _nopHttpClient.GetNewsRssAsync();
                }
                catch (AggregateException exception)
                {
                    //rethrow actual excepion
                    throw exception.InnerException;
                }
            });

            for (var i = 0; i < rssData.Items.Count; i++)
            {
                var item = rssData.Items.ElementAt(i);
                var newsItem = new NopCommerceNewsDetailsModel
                {
                    Title = item.TitleText,
                    Summary = XmlHelper.XmlDecode(item.Content?.Value ?? string.Empty),
                    Url = item.Url.OriginalString,
                    PublishDate = item.PublishDate
                };
                model.Items.Add(newsItem);

                //has new items?
                if (i != 0)
                    continue;

                var firstRequest = string.IsNullOrEmpty(_adminAreaSettings.LastNewsTitleAdminArea);
                if (_adminAreaSettings.LastNewsTitleAdminArea == newsItem.Title)
                    continue;

                _adminAreaSettings.LastNewsTitleAdminArea = newsItem.Title;
                await _settingService.SaveSettingAsync(_adminAreaSettings);

                //new item
                if (!firstRequest)
                    model.HasNewItems = true;
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync("No access to the news. Website www.nopcommerce.com is not available.", ex);
        }

        return model;
    }

    #endregion
}