namespace Nop.Services.ExportImport;

/// <summary>
/// Import manager interface
/// </summary>
public partial interface IImportManager
{
    /// <summary>
    /// Import customers from XLSX file
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task ImportCustomersFromXlsxAsync(Stream stream);

}