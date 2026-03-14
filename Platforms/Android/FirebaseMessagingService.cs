using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Firebase.Messaging;

namespace ThePupCircle.MobileApp.Platforms.Android;

/// <summary>
/// Handles incoming FCM messages and token refresh.
/// Background/killed-app notifications are shown automatically by the Firebase SDK
/// when the message contains a "notification" payload.
/// This service handles data-only messages and token rotation.
/// </summary>
[Service(Exported = false)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class PupCircleFirebaseMessagingService : FirebaseMessagingService
{
    private const string ChannelId = "thepupcircle_notifications";

    public override void OnNewToken(string token)
    {
        // Store the new token so MainPage can re-register it with the server
        Microsoft.Maui.Storage.Preferences.Default.Set("fcm_token", token);
        Microsoft.Maui.Storage.Preferences.Default.Set("fcm_token_registered", false);
    }

    public override void OnMessageReceived(RemoteMessage message)
    {
        // Firebase auto-displays notification-payload messages when the app is in the background.
        // This handles foreground display and data-only messages.
        var title = message.GetNotification()?.Title
                    ?? message.Data.GetValueOrDefault("title", "ThePupCircle");
        var body  = message.GetNotification()?.Body
                    ?? message.Data.GetValueOrDefault("body", string.Empty);
        var url   = message.Data.GetValueOrDefault("url", string.Empty);

        if (string.IsNullOrEmpty(body)) return;

        ShowNotification(title, body, url);
    }

    private void ShowNotification(string title, string body, string url)
    {
        EnsureChannel();

        var builder = new NotificationCompat.Builder(this, ChannelId)
            .SetSmallIcon(ApplicationInfo!.Icon)
            .SetContentTitle(title)
            .SetContentText(body)
            .SetAutoCancel(true)
            .SetPriority(NotificationCompat.PriorityDefault);

        if (!string.IsNullOrEmpty(url))
        {
            var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(url));
            intent.AddFlags(ActivityFlags.NewTask);
            var pending = PendingIntent.GetActivity(
                this, 0, intent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
            builder.SetContentIntent(pending);
        }

        NotificationManagerCompat.From(this).Notify(Environment.TickCount, builder.Build());
    }

    private void EnsureChannel()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;
        var nm = (NotificationManager)GetSystemService(NotificationService)!;
        if (nm.GetNotificationChannel(ChannelId) != null) return;
        nm.CreateNotificationChannel(
            new NotificationChannel(ChannelId, "ThePupCircle", NotificationImportance.Default));
    }
}
