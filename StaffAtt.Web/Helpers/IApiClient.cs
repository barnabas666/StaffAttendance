namespace StaffAtt.Web.Helpers;

public interface IApiClient
{
    Task<Result<T>> GetAsync<T>(string endpoint);
    Task<Result<T>> PostAsync<T>(string endpoint, T data);
    // Add Put/Delete as needed
}
