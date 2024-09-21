using Minigun.Models;

namespace Minigun.Services;

public interface IRaygunApiService
{
    Task<List<Application>?> ListApplicationsAsync(int? count = null, int? offset = null, string[]? orderby = null);
    Task<List<ErrorGroup>?> ListErrorGroupsAsync(string applicationId, int? count = null, int? offset = null, string[]? orderby = null);
}