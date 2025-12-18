using Nop.Core.Caching;
using Nop.Core.Domain.Configuration;
using Nop.Core.Events;
using Nop.Services.Events;

namespace Nop.Web.Areas.Admin.Infrastructure.Cache;

/// <summary>
/// Model cache event consumer (used for caching of presentation layer models)
/// </summary>
public partial class ModelCacheEventConsumer :
    //settings
    IConsumer<EntityUpdatedEvent<Setting>>
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

    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
    {
        //clear models which depend on settings (infrastructure only - removed commerce-specific cache keys)
    }

    #endregion
}