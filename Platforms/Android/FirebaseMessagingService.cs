using Android.App;
using Firebase.Messaging;
using Plugin.Firebase.CloudMessaging;

namespace ThePupCircle.MobileApp.Platforms.Android;

[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class PupCircleFirebaseMessagingService : FirebaseMessagingService
{
    public override void OnMessageReceived(RemoteMessage message)
    {
        System.Diagnostics.Debug.WriteLine($"=== FCM Message Received ===");
        System.Diagnostics.Debug.WriteLine($"From: {message.From}");

        CrossFirebaseCloudMessaging.Current.OnMessageReceived(message);
    }

    public override void OnNewToken(string token)
    {
        System.Diagnostics.Debug.WriteLine($"=== FCM Token Refreshed ===");
        System.Diagnostics.Debug.WriteLine($"New token: {token[..Math.Min(20, token.Length)]}...");

        CrossFirebaseCloudMessaging.Current.OnNewToken(token);
    }
}
