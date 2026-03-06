using System.Text.Json;

namespace ThePupCircle.MobileApp.Services;

public class AuthService : IAuthService
{
    private const string TokenKey = "jwt_token";
    private static string BaseUrl => AppConstants.BaseUrl;

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "ThePupCircleMobile/1.0");
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<string?> GetStoredTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(TokenKey);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task StoreTokenAsync(string token)
    {
        try
        {
            await SecureStorage.SetAsync(TokenKey, token);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to store token: {ex.Message}");
        }
    }

    public async Task ClearTokenAsync()
    {
        try
        {
            SecureStorage.Remove(TokenKey);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to clear token: {ex.Message}");
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/pwaauth/validate");
            request.Headers.Add("X-Auth-Token", token);
            
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<string?> RefreshTokenAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/pwaauth/refresh");
            request.Headers.Add("X-Auth-Token", token);
            
            var response = await _httpClient.SendAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RefreshTokenResponse>(content, _jsonOptions);
                
                if (result?.Token != null)
                {
                    await StoreTokenAsync(result.Token);
                    return result.Token;
                }
            }
            
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private class RefreshTokenResponse
    {
        public string? Token { get; set; }
    }
}

