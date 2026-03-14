using Foundation;
using Microsoft.Maui.Handlers;
using UserNotifications;
using WebKit;

namespace ThePupCircle.MobileApp.Platforms.iOS;

public class CustomWebViewHandler : WebViewHandler
{
    private NativeScriptHandler? _scriptHandler;

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

        // Register JS bridge — callable as window.webkit.messageHandlers.nativeApp.postMessage(...)
        _scriptHandler = new NativeScriptHandler();
        platformView.Configuration.UserContentController.AddScriptMessageHandler(_scriptHandler, "nativeApp");

        // Request notification permission on first launch
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var center = UNUserNotificationCenter.Current;
            await center.RequestAuthorizationAsync(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge);
        });

        System.Diagnostics.Debug.WriteLine("=== CustomWebViewHandler.ConnectHandler completed (iOS) ===");
    }

    protected override void DisconnectHandler(WKWebView platformView)
    {
        if (_scriptHandler != null)
            platformView.Configuration.UserContentController.RemoveScriptMessageHandler("nativeApp");

        base.DisconnectHandler(platformView);
    }
}

/// <summary>
/// Receives messages from window.webkit.messageHandlers.nativeApp.postMessage({ title, body, url })
/// and shows a native local notification.
/// </summary>
public class NativeScriptHandler : NSObject, IWKScriptMessageHandler
{
    public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
    {
        if (message.Body is not NSDictionary body) return;

        var title = body["title"]?.ToString() ?? string.Empty;
        var text  = body["body"]?.ToString()  ?? string.Empty;
        var url   = body["url"]?.ToString();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var content = new UNMutableNotificationContent
            {
                Title = title,
                Body  = text,
                Sound = UNNotificationSound.Default,
            };

            if (!string.IsNullOrEmpty(url))
                content.UserInfo = NSDictionary.FromObjectAndKey(
                    NSObject.FromObject(url), NSObject.FromObject("url"));

            var request = UNNotificationRequest.FromIdentifier(
                Guid.NewGuid().ToString(), content, trigger: null);

            await UNUserNotificationCenter.Current.AddNotificationRequestAsync(request);
        });
    }
}
