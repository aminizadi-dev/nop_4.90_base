using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common;

public partial record CommonStatisticsModel : BaseNopModel
{
    //COMMERCE STATISTICS REMOVED - Phase B
    //Removed: NumberOfOrders, NumberOfPendingReturnRequests, NumberOfLowStockProducts

    public int NumberOfCustomers { get; set; }
}