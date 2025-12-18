using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Http;
using Nop.Data;
using Nop.Services.Security;

namespace Nop.Web.Framework.Mvc.Filters;

/// <summary>
/// Represents a filter attribute that confirms access to a closed store
/// </summary>
public sealed class CheckAccessClosedStoreAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public CheckAccessClosedStoreAttribute(bool ignore = false) : base(typeof(CheckAccessClosedStoreFilter))
    {
        IgnoreFilter = ignore;
        Arguments = [ignore];
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to ignore the execution of filter actions
    /// </summary>
    public bool IgnoreFilter { get; }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents a filter that confirms access to closed store
    /// </summary>
    private class CheckAccessClosedStoreFilter : IAsyncActionFilter
    {
        #region Fields

        protected readonly bool _ignoreFilter;
        protected readonly IPermissionService _permissionService;
        protected readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public CheckAccessClosedStoreFilter(bool ignoreFilter,
            IPermissionService permissionService,
            IStoreContext storeContext)
        {
            _ignoreFilter = ignoreFilter;
            _permissionService = permissionService;
            _storeContext = storeContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task CheckAccessClosedStoreAsync(ActionExecutingContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<CheckAccessClosedStoreAttribute>()
                .FirstOrDefault();

            //ignore filter (the action is available even if a store is closed)
            if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                return;

            //get action and controller names
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor?.ActionName;
            var controllerName = actionDescriptor?.ControllerName;

            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
                return;            

            //store is closed and no access, so redirect to 'StoreClosed' page
            context.Result = new RedirectToRouteResult(NopRouteNames.Standard.STORE_CLOSED, null);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await CheckAccessClosedStoreAsync(context);
            if (context.Result == null)
                await next();
        }

        #endregion
    }

    #endregion
}