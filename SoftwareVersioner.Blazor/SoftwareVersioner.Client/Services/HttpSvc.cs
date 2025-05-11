using JinnDev.Utilities.Monad;
using System.Net.Http.Json;
using System.Text.Json;

namespace SoftwareVersioner.Client.Services;

public class HttpSvc(HttpClient HttpClient) : IHttpSvc
{
    public async Task<Maybe<string>> ProcessNameAsync(string? name)
    {
        string errMsg;
        try
        {
            HttpResponseMessage response = await HttpClient.PostAsJsonAsync("/api/name", name);

            if (response.IsSuccessStatusCode)
            {
                string? processedName = await response.Content.ReadAsStringAsync();
                return processedName;
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

        return Maybe.None<string>(errMsg);
    }
}