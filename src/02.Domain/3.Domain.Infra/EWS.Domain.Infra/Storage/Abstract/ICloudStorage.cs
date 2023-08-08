namespace EWS.Domain.Infra.Storage;

public interface ICloudStorage
{
    Task<string> FileDownloadUrl(string tenantId, string fullUri, string fileName);
    Task<bool> FileDelete(string tenantId, string userId, string hashedFileName);
    Task<bool> FileExist(string tenantId, string userId, string hashedFileName);
}