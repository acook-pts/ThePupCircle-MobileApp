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

		ApplyBrandStatusBar();
		RequestNotificationPermission();
	}

	private void ApplyBrandStatusBar()
	{
		// Set status bar and navigation bar to brand green (#4A6741)
		Window?.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
		Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#4A6741"));
		Window?.SetNavigationBarColor(Android.Graphics.Color.ParseColor("#4A6741"));

		// Ensure status bar icons are white (not dark), since background is dark green
		if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
		{
			var decorView = Window?.DecorView;
			if (decorView != null)
			{
				decorView.SystemUiVisibility = (Android.Views.StatusBarVisibility)
					((int)decorView.SystemUiVisibility & ~(int)Android.Views.SystemUiFlags.LightStatusBar);
			}
		}
	}

	private void RequestNotificationPermission()
	{
		// Android 13+ (API 33) requires explicit POST_NOTIFICATIONS runtime permission
		if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
		{
			RequestPermissions(new[] { Android.Manifest.Permission.PostNotifications }, 0);
		}
	}
}
