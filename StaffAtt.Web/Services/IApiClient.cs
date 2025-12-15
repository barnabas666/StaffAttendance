using StaffAtt.Web.Models;

namespace StaffAtt.Web.Services;

public interface IApiClient
{
    Task<Result<T>> GetAsync<T>(string endpoint);
    Task<Result<T>> PostAsync<T>(string endpoint, T data);
    Task<Result<T>> PutAsync<T>(string endpoint, T data);
    Task<Result<bool>> DeleteAsync(string endpoint);
}
