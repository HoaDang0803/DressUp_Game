using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class AndroidNotifications : MonoBehaviour
{
    public void RequestAuthorization()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATION"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATION");
        }
    }

    public void RegisterNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Vlinder Princess",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void SendNotification(string title, string text, int seconds)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.FireTime = System.DateTime.Now.AddSeconds(seconds);
        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";
        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }
}
