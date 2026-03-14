namespace ThePupCircle.MobileApp.Services;

public interface IPushNotificationService
{
    /// <summary>
    /// Fired whenever a valid FCM token is available (on startup and on refresh).
    /// The website should listen for window event 'nativePushTokenReady' instead of
    /// consuming this directly.
    /// </summary>
    event EventHandler<string> TokenReceived;

    /// <summary>Returns the current FCM token, or null if not yet available.</summary>
    string? GetToken();

    /// <summary>Initialises Firebase, requests permissions, and fetches the token.</summary>
    Task InitializeAsync();
}
