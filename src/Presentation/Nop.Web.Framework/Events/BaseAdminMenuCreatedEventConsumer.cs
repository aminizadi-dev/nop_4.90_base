using Nop.Services.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Base admin menu created event consumer
/// NOTE: Plugin infrastructure removed - this class simplified to remove plugin dependencies
/// </summary>
public abstract class BaseAdminMenuCreatedEventConsumer : IConsumer<AdminMenuCreatedEvent>
{
    #region Utilities

    /// <summary>
    /// Checks is the current customer has rights to access this menu item
    /// By default, always return true
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the true if access is granted, otherwise false
    /// </returns>
    protected virtual Task<bool> CheckAccessAsync()
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// Gets the menu item to insert
    /// Override this method in derived classes to provide custom menu items
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the instance of <see cref="AdminMenuItem"/> or null
    /// </returns>
    protected virtual Task<AdminMenuItem> GetAdminMenuItemAsync()
    {
        return Task.FromResult<AdminMenuItem>(null);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        if (!await CheckAccessAsync())
            return;

        var newItem = await GetAdminMenuItemAsync();

        if (newItem == null)
            return;

        switch (InsertType)
        {
            case MenuItemInsertType.After:
                eventMessage.RootMenuItem.InsertAfter(AfterMenuSystemName, newItem);
                break;
            case MenuItemInsertType.Before:
                eventMessage.RootMenuItem.InsertBefore(BeforeMenuSystemName, newItem);
                break;
            case MenuItemInsertType.TryAfterThanBefore:
                if (!eventMessage.RootMenuItem.InsertAfter(AfterMenuSystemName, newItem))
                    eventMessage.RootMenuItem.InsertBefore(BeforeMenuSystemName, newItem);
                break;
            case MenuItemInsertType.TryBeforeThanAfter:
                if (!eventMessage.RootMenuItem.InsertBefore(BeforeMenuSystemName, newItem))
                    eventMessage.RootMenuItem.InsertAfter(AfterMenuSystemName, newItem);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Menu item insertion type (by default: <see cref="MenuItemInsertType.Before"/>)
    /// </summary>
    protected virtual MenuItemInsertType InsertType => MenuItemInsertType.Before;

    /// <summary>
    /// The system name of the menu item after with need to insert the current one
    /// </summary>
    protected virtual string AfterMenuSystemName => string.Empty;

    /// <summary>
    /// The system name of the menu item before with need to insert the current one
    /// </summary>
    protected virtual string BeforeMenuSystemName => string.Empty;

    #endregion
}