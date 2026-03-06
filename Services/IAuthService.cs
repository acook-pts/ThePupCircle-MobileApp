namespace ThePupCircle.MobileApp.Services;

public interface IAuthService
{
    Task<string?> GetStoredTokenAsync();
    Task StoreTokenAsync(string token);
    Task ClearTokenAsync();
    Task<bool> ValidateTokenAsync(string token);
    Task<string?> RefreshTokenAsync(string token);
}
