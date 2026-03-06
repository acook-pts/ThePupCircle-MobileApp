namespace ThePupCircle.MobileApp;

public partial class App : Application
{
	public App()
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("=== App Constructor START ===");
			InitializeComponent();
			System.Diagnostics.Debug.WriteLine("=== App InitializeComponent completed ===");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"!!! App Constructor FAILED: {ex}");
			throw;
		}
	}

	protected override Window CreateWindow(IActivationState activationState)
	{
		try
		{
			System.Diagnostics.Debug.WriteLine("=== CreateWindow START ===");
			// Get MainPage from DI after resources are loaded
			var mainPage = Handler.MauiContext.Services.GetRequiredService<MainPage>();
			System.Diagnostics.Debug.WriteLine("MainPage retrieved from DI");
			
			var window = new Window(mainPage);
			System.Diagnostics.Debug.WriteLine("=== CreateWindow END ===");
			return window;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"!!! CreateWindow FAILED: {ex}");
			throw;
		}
	}
}