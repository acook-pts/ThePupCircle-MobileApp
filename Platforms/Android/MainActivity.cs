using Android.App;
using Android.Content.PM;
using Android.OS;

namespace ThePupCircle.MobileApp;

[Activity(
	Theme = "@style/Maui.SplashTheme", 
	MainLauncher = true, 
	ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		
		// Enable hardware acceleration for smooth scrolling
		Window?.SetFlags(
			Android.Views.WindowManagerFlags.HardwareAccelerated,
			Android.Views.WindowManagerFlags.HardwareAccelerated);
	}
}
