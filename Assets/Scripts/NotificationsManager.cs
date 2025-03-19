using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class NotificationsManager : MonoBehaviour
{
    [SerializeField] private AndroidNotifications androidNotifications;

    void Start()
    {
        androidNotifications.RequestAuthorization();
        androidNotifications.RegisterNotificationChannel();
        if (PlayerPrefs.GetInt("FirstPlay", 0) == 0)
        {
            PlayerPrefs.SetInt("FirstPlay", 1);
            ScheduleNotifications();
        }
    }

    private void ScheduleNotifications()
    {
        androidNotifications.SendNotification("Daily notifications", "Oh!😯 Our customers are waiting for you!✨", 1 * 60);
        androidNotifications.SendNotification("Daily notifications", "Dress, shoes, styling, makeup - all you need for a perfect look!💋", 2 * 60);
        androidNotifications.SendNotification("Daily notifications", "Look! There is new bonus set of clothes!👚 👖", 3 * 60);
        androidNotifications.SendNotification("Daily notifications", "Find the best hairstyle for our couples!💇 ✨", 4 * 60);
        androidNotifications.SendNotification("Daily notifications", "Can you make perfect outfit with new bonus items?💎 💕", 5 * 60);
        androidNotifications.SendNotification("Daily notifications", "Check our reward today! 😉", 6 * 60);
        androidNotifications.SendNotification("Daily notifications", "Comeback! Our couples need you!", 13 * 60);
    }
}
