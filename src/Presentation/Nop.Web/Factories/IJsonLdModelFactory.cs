//COMMERCE INTERFACE REMOVED - Phase C
//All methods in IJsonLdModelFactory are commerce-related (category, product, forum)
//Removed: using Nop.Core.Domain.Forums;
//Removed: using Nop.Web.Models.Boards;
//Removed: using Nop.Web.Models.Catalog;
using Nop.Web.Models.JsonLD;

namespace Nop.Web.Factories;

/// <summary>
/// Represents JSON-LD model factory
/// </summary>
public partial interface IJsonLdModelFactory
{
    //COMMERCE METHODS REMOVED - Phase C
    //All JSON-LD methods are commerce-related and have been removed
    //Removed: PrepareJsonLdCategoryBreadcrumbAsync
    //Removed: PrepareJsonLdProductBreadcrumbAsync
    //Removed: PrepareJsonLdProductAsync
    //Removed: PrepareJsonLdForumTopicAsync
}