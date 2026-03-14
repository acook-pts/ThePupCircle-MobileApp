namespace ThePupCircle.MobileApp;

public partial class MainPage : ContentPage
{
	private static string BaseUrl => AppConstants.BaseUrl;

	public MainPage()
	{
		InitializeComponent();
		System.Diagnostics.Debug.WriteLine($"=== MainPage Constructor ===");
		System.Diagnostics.Debug.WriteLine($"BaseUrl: {BaseUrl}");
		LoadWebsite();
	}

	private void LoadWebsite()
	{
		try
		{
			System.Diagnostics.Debug.WriteLine($"LoadWebsite() called");
			System.Diagnostics.Debug.WriteLine($"Loading URL: {BaseUrl}");

			// Set the source but keep loading indicator visible
			// It will hide when OnWebViewNavigated fires
			webView.Source = BaseUrl;

			System.Diagnostics.Debug.WriteLine($"WebView.Source set successfully");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"LoadWebsite error: {ex.Message}");
			System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

			loadingIndicator.IsVisible = false;
			DisplayAlert("Connection Error",
				$"Failed to load: {BaseUrl}\n\nError: {ex.Message}",
				"OK");
		}
	}

	private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
	{
		System.Diagnostics.Debug.WriteLine($"=== WebView Navigating ===");
		System.Diagnostics.Debug.WriteLine($"URL: {e.Url}");
		System.Diagnostics.Debug.WriteLine($"NavigationEvent: {e.NavigationEvent}");
	}

	private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
	{
		System.Diagnostics.Debug.WriteLine($"=== WebView Navigated ===");
		System.Diagnostics.Debug.WriteLine($"URL: {e.Url}");
		System.Diagnostics.Debug.WriteLine($"Result: {e.Result}");

		if (e.Result == WebNavigationResult.Success)
		{
			System.Diagnostics.Debug.WriteLine("Navigation successful - showing WebView");
			loadingIndicator.IsVisible = false;
			webView.IsVisible = true;
		}
		else
		{
			System.Diagnostics.Debug.WriteLine($"Navigation failed: {e.Result}");
			loadingIndicator.IsVisible = false;
			DisplayAlert("Load Failed",
				$"Failed to load page.\n\nURL: {e.Url}\nResult: {e.Result}\n\nCheck the Output window for details.",
				"OK");
		}
	}
}
