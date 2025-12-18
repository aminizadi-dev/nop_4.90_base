using System.Globalization;
using System.Text.Encodings.Web;
using Nop.Core;
//COMMERCE DOMAIN REMOVED - Phase C
//Removed: using Nop.Core.Domain.Catalog;
//Removed: using Nop.Core.Domain.Forums;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Services.Customers;
//COMMERCE SERVICE REMOVED - Phase C
//Removed: using Nop.Services.Forums;
using Nop.Services.Helpers;
using Nop.Web.Framework.Mvc.Routing;
//COMMERCE MODELS REMOVED - Phase C
//Removed: using Nop.Web.Models.Boards;
//Removed: using Nop.Web.Models.Catalog;
using Nop.Web.Models.JsonLD;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the JSON-LD model factory implementation
/// </summary>
public partial class JsonLdModelFactory : IJsonLdModelFactory
{
    #region Fields

    //COMMERCE SETTINGS REMOVED - Phase C
    //Removed: protected readonly ForumSettings _forumSettings;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IEventPublisher _eventPublisher;
    //COMMERCE SERVICE REMOVED - Phase C
    //Removed: protected readonly IForumService _forumService;
    protected readonly INopUrlHelper _nopUrlHelper;
    protected readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public JsonLdModelFactory(
        //COMMERCE SETTINGS REMOVED - Phase C
        //Removed: ForumSettings forumSettings,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IEventPublisher eventPublisher,
        //COMMERCE SERVICE REMOVED - Phase C
        //Removed: IForumService forumService,
        INopUrlHelper nopUrlHelper,
        IWebHelper webHelper)
    {
        //Removed: _forumSettings = forumSettings;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _eventPublisher = eventPublisher;
        //Removed: _forumService = forumService;
        _nopUrlHelper = nopUrlHelper;
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Convert datetime to an ISO 8601 string
    /// </summary>
    /// <param name="dateTime">Datetime object to convert</param>
    /// <returns>Datetime string in the ISO 8601 format</returns>
    protected virtual string ConvertDateTimeToIso8601String(DateTime? dateTime)
    {
        return dateTime == null ? null : new DateTimeOffset(dateTime.Value).ToString("O", CultureInfo.InvariantCulture);
    }

    //COMMERCE METHOD REMOVED - Phase C
    //Removed: PrepareJsonLdBreadcrumbListAsync (category breadcrumb - commerce feature)

    #endregion

    #region Methods

    //COMMERCE METHODS REMOVED - Phase C
    //All JSON-LD methods are commerce-related and have been removed
    //Removed: PrepareJsonLdCategoryBreadcrumbAsync
    //Removed: PrepareJsonLdProductBreadcrumbAsync
    //Removed: PrepareJsonLdProductAsync
    //Removed: PrepareJsonLdForumTopicAsync

    #endregion
}