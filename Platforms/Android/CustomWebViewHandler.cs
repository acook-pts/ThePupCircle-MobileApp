using Android.Webkit;
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

        // Enable app cache
        webView.Settings.SetAppCacheEnabled(true);
        webView.Settings.CacheMode = CacheModes.Default;

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