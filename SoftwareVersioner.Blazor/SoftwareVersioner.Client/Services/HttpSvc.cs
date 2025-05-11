using JinnDev.Utilities.Monad;
using SoftwareVersioner.Core;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace SoftwareVersioner.Client.Services;

public class HttpSvc(HttpClient HttpClient) : IHttpSvc
{
    public async Task<Maybe<List<Software>>> GetAllSoftwareAsync()
    {
        string errMsg;
        try
        {
            HttpResponseMessage response = await HttpClient.GetAsync("api/software/all");

            if (response.IsSuccessStatusCode)
            {
                var softwareList = await response.Content.ReadFromJsonAsync<List<Software>>();
                if (softwareList != null)
                    return softwareList;
                errMsg = $"API Returned Null";
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                errMsg = $"API Error: {response.StatusCode} - {errorContent}";
            }
        }
        catch (HttpRequestException ex)
        {
            errMsg = $"Network request failed: {ex.Message}";
        }
        catch (JsonException ex)
        {
            errMsg = $"Failed to process JSON response: {ex.Message}";
        }
        catch (Exception ex)
        {
            errMsg = $"An unexpected error occurred: {ex.Message}";
        }

        return Maybe.None<List<Software>>(errMsg);
    }
}