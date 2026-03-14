using Android.App;
using Android.Content;
using Android.OS;
using Android.Webkit;
using AndroidX.Core.App;
using Java.Interop;
using Microsoft.Maui.Handlers;

namespace ThePupCircle.MobileApp.Platforms.Android;

public class CustomWebViewHandler : WebViewHandler
{
    protected override void ConnectHandler(global::Android.Webkit.WebView platformView)
    {
        // Don't call base yet - we need to set client first
        ConfigureWebView(platformView);
        
        // Set our custom WebViewClient BEFORE calling base
        var customClient = new CustomWebViewClient();
        platformView.SetWebViewClient(customClient);
        
        // Now call base - it might try to set its own client, but we'll override it again
        base.ConnectHandler(platformView);
        
        // Set it again after base to make sure ours is used
        platformView.SetWebViewClient(customClient);
        
        System.Diagnostics.Debug.WriteLine("=== CustomWebViewHandler.ConnectHandler completed ===");
        System.Diagnostics.Debug.WriteLine($"WebViewClient type: {platformView.WebViewClient?.GetType().Name}");
    }

    private void ConfigureWebView(global::Android.Webkit.WebView webView)
    {
        if (webView?.Settings == null)
            return;

        // Enable JavaScript - Required for PWA and modern websites
        webView.Settings.JavaScriptEnabled = true;
        webView.Settings.JavaScriptCanOpenWindowsAutomatically = true;

        // Enable DOM storage - Required for localStorage/sessionStorage (PWA requirement)
        webView.Settings.DomStorageEnabled = true;

        // Enable database storage
        webView.Settings.DatabaseEnabled = true;

        // Enable mixed content
        webView.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;

        // Enable zoom controls
        webView.Settings.SetSupportZoom(true);
        webView.Settings.BuiltInZoomControls = true;
        webView.Settings.DisplayZoomControls = false;

        // Enable wide viewport
        webView.Settings.UseWideViewPort = true;
        webView.Settings.LoadWithOverviewMode = true;

        // Use cached resources first, falling back to network — speeds up repeat launches
        webView.Settings.CacheMode = CacheModes.CacheElseNetwork;

        // Register JS bridge — accessible as window.NativeApp in the WebView
        webView.AddJavascriptInterface(new NativeJsBridge(webView.Context!), "NativeApp");

        // Set user agent
        var userAgent = webView.Settings.UserAgentString;
        if (!string.IsNullOrEmpty(userAgent))
        {
            webView.Settings.UserAgentString = $"{userAgent} ThePupCircleMobile/1.0";
        }

        // Allow file access
        webView.Settings.AllowFileAccess = true;
        webView.Settings.AllowContentAccess = true;

        // Set WebChromeClient for console messages and alerts
        webView.SetWebChromeClient(new CustomWebChromeClient());
    }

    private class CustomWebChromeClient : WebChromeClient
    {
        public override bool OnConsoleMessage(ConsoleMessage consoleMessage)
        {
            System.Diagnostics.Debug.WriteLine(
                $"WebView Console [{consoleMessage.InvokeMessageLevel()}]: {consoleMessage.Message()} -- line {consoleMessage.LineNumber()} of {consoleMessage.SourceId()}"
            );
            return true;
        }
    }

    private class CustomWebViewClient : WebViewClient
    {
        public override void OnReceivedSslError(global::Android.Webkit.WebView view, SslErrorHandler handler, global::Android.Net.Http.SslError error)
        {
            var url = error.Url;
            System.Diagnostics.Debug.WriteLine($"*** SSL ERROR RECEIVED ***");
            System.Diagnostics.Debug.WriteLine($"URL: {url}");
            System.Diagnostics.Debug.WriteLine($"Error: {error}");
            
            // Only ignore SSL errors for localhost/development addresses
            if (url.Contains("10.0.2.2") || url.Contains("localhost") || url.Contains("127.0.0.1"))
            {
                System.Diagnostics.Debug.WriteLine($">>> PROCEEDING despite SSL error for localhost");
                handler.Proceed(); // Ignore SSL errors for localhost
            }
            else
            {
                // For production URLs, respect SSL errors
                System.Diagnostics.Debug.WriteLine($">>> CANCELING navigation due to SSL error");
                handler.Cancel();
            }
        }

        public override void OnReceivedError(global::Android.Webkit.WebView view, IWebResourceRequest request, WebResourceError error)
        {
            base.OnReceivedError(view, request, error);
            System.Diagnostics.Debug.WriteLine($"*** RESOURCE ERROR ***");
            System.Diagnostics.Debug.WriteLine($"URL: {request.Url}");
            System.Diagnostics.Debug.WriteLine($"Error Code: {error.ErrorCode}");
            System.Diagnostics.Debug.WriteLine($"Description: {error.Description}");
        }

        public override void OnPageFinished(global::Android.Webkit.WebView view, string url)
        {
            base.OnPageFinished(view, url);
            System.Diagnostics.Debug.WriteLine($"*** PAGE FINISHED: {url}");
        }
    }
}

/// <summary>
/// JavaScript bridge exposed as window.NativeApp in the WebView.
/// The website calls window.NativeApp.showNotification(title, body, url)
/// to trigger a native system notification.
/// </summary>
[JavascriptInterface]
[Android.Runtime.Register("ThePupCircleNativeBridge")]
public class NativeJsBridge : Java.Lang.Object
{
    private const string ChannelId = "thepupcircle_notifications";
    private readonly Context _context;

    public NativeJsBridge(Context context)
    {
        _context = context;
        EnsureNotificationChannel();
    }

    [JavascriptInterface]
    [Export("showNotification")]
    public void ShowNotification(string title, string body, string url)
    {
        var builder = new NotificationCompat.Builder(_context, ChannelId)
            .SetSmallIcon(_context.ApplicationInfo!.Icon)
            .SetContentTitle(title)
            .SetContentText(body)
            .SetAutoCancel(true)
            .SetPriority(NotificationCompat.PriorityDefault);

        if (!string.IsNullOrEmpty(url))
        {
            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
            intent.AddFlags(ActivityFlags.NewTask);
            var pending = PendingIntent.GetActivity(
                _context, 0, intent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            builder.SetContentIntent(pending);
        }

        NotificationManagerCompat.From(_context).Notify(Environment.TickCount, builder.Build());
    }

    private void EnsureNotificationChannel()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;

        var nm = (NotificationManager)_context.GetSystemService(Context.NotificationService)!;
        if (nm.GetNotificationChannel(ChannelId) != null) return;

        var channel = new NotificationChannel(ChannelId, "ThePupCircle", NotificationImportance.Default);
        nm.CreateNotificationChannel(channel);
    }
}