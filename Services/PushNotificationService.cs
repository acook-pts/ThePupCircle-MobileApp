using Plugin.Firebase.CloudMessaging;

namespace ThePupCircle.MobileApp.Services;

public class PushNotificationService : IPushNotificationService
{
    public event EventHandler<string>? TokenReceived;

    private string? _token;

    public string? GetToken() => _token;

    public async Task InitializeAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("=== PushNotificationService.InitializeAsync ===");

            // Subscribe to token refresh so we always have the latest
            CrossFirebaseCloudMessaging.Current.TokenChanged += OnTokenChanged;
            CrossFirebaseCloudMessaging.Current.NotificationTapped += OnNotificationTapped;

            // Request permission and get the initial token
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();

            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _token = token;
                System.Diagnostics.Debug.WriteLine($"=== FCM Token obtained (first {Math.Min(20, token.Length)} chars): {token[..Math.Min(20, token.Length)]}... ===");
                TokenReceived?.Invoke(this, token);
            }
        }
        catch (Exception ex)
        {
            // Firebase not yet configured (placeholder google-services.json / GoogleService-Info.plist).
            // The app will still work normally; push notifications will activate once real Firebase
            // config files are in place.
            System.Diagnostics.Debug.WriteLine($"=== PushNotificationService: Firebase not configured — {ex.Message} ===");
        }
    }

    private void OnTokenChanged(object? sender, FCMTokenChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Token)) return;

        _token = e.Token;
        System.Diagnostics.Debug.WriteLine($"=== FCM Token refreshed ===");
        TokenReceived?.Invoke(this, e.Token);
    }

    private void OnNotificationTapped(object? sender, FCMNotificationTappedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"=== Push notification tapped ===");
        // If the notification carries a deep-link URL, navigate the WebView to it
        if (e.Notification.Data.TryGetValue("url", out var url) && !string.IsNullOrEmpty(url))
        {
            System.Diagnostics.Debug.WriteLine($"Notification URL: {url}");
            NotificationUrlReceived?.Invoke(this, url);
        }
    }

    /// <summary>Fired when a tapped notification contains a 'url' data payload.</summary>
    public event EventHandler<string>? NotificationUrlReceived;
}
