using Microsoft.Maui.Handlers;
using WebKit;

namespace ThePupCircle.MobileApp.Platforms.iOS;

public class CustomWebViewHandler : WebViewHandler
{
    protected override void ConnectHandler(WKWebView platformView)
    {
        base.ConnectHandler(platformView);

        // Allow inline media playback (no fullscreen forced)
        platformView.Configuration.AllowsInlineMediaPlayback = true;

        // Allow picture-in-picture
        platformView.Configuration.AllowsPictureInPictureMediaPlayback = true;

        // Set custom user agent so the site knows it's the mobile app
        var defaultAgent = platformView.CustomUserAgent ?? string.Empty;
        platformView.CustomUserAgent = $"{defaultAgent} ThePupCircleMobile/1.0".Trim();

        System.Diagnostics.Debug.WriteLine("=== CustomWebViewHandler.ConnectHandler completed (iOS) ===");
    }
}
