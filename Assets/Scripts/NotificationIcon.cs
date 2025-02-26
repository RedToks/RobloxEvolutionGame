using UnityEngine;
using UnityEngine.UI;

public class NotificationIcon : MonoBehaviour
{
    public GameObject exclamationMark; // Ссылка на значок "!"
    private bool hasNotification = false;

    // Включает или выключает значок
    public void SetNotification(bool active)
    {
        hasNotification = active;
        if (exclamationMark)
        {
            exclamationMark.SetActive(active);
        }
    }

    // Можно использовать для ручного обновления
    public bool HasNotification()
    {
        return hasNotification;
    }
}