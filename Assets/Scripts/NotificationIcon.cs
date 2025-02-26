using UnityEngine;
using UnityEngine.UI;

public class NotificationIcon : MonoBehaviour
{
    public GameObject exclamationMark; // ������ �� ������ "!"
    private bool hasNotification = false;

    // �������� ��� ��������� ������
    public void SetNotification(bool active)
    {
        hasNotification = active;
        if (exclamationMark)
        {
            exclamationMark.SetActive(active);
        }
    }

    // ����� ������������ ��� ������� ����������
    public bool HasNotification()
    {
        return hasNotification;
    }
}