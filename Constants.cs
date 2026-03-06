namespace ThePupCircle.MobileApp;

public static class AppConstants
{
    /// <summary>
    /// Base URL for the website - using production for both DEBUG and RELEASE
    /// </summary>
    public static string BaseUrl => "https://thepupcircle.com";

    // Note: To test against localhost during development, you can temporarily change
    // the BaseUrl above to point to your local development server:
    // - Android Emulator: "http://10.0.2.2:5011" (use HTTP to avoid cert issues)
    // - iOS Simulator: "http://localhost:5011"
    // - Physical Device: "http://192.168.1.XXX:5011" (use your machine's IP)
}
