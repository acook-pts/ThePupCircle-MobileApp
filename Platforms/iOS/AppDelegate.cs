using Foundation;
using Plugin.Firebase.CloudMessaging;
using UIKit;
using UserNotifications;

namespace ThePupCircle.MobileApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
	{
		var result = base.FinishedLaunching(application, launchOptions);

		// Request push notification authorisation
		UNUserNotificationCenter.Current.RequestAuthorization(
			UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge,
			(granted, error) =>
			{
				System.Diagnostics.Debug.WriteLine($"=== Push permission granted: {granted} ===");
				if (granted)
					InvokeOnMainThread(() => UIApplication.SharedApplication.RegisterForRemoteNotifications());
			});

		return result;
	}

	// Called by iOS when APNs registration succeeds — Firebase forwards this to FCM
	public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
	{
		System.Diagnostics.Debug.WriteLine("=== APNs token received, forwarding to Firebase ===");
		CrossFirebaseCloudMessaging.Current.OnApnsTokenReceived(deviceToken.ToString());
	}

	public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
	{
		System.Diagnostics.Debug.WriteLine($"=== APNs registration failed: {error.LocalizedDescription} ===");
	}

	// Required for background/killed-state notifications via FCM
	[Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
	public void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
	{
		System.Diagnostics.Debug.WriteLine("=== Remote notification received (background) ===");
		CrossFirebaseCloudMessaging.Current.HandleNotification(userInfo, completionHandler);
	}
}
