using System.Collections;
using TMPro;
using UnityEngine;

public class CurrencyNotifier : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText; // ������ �� ��������� �������
    [SerializeField] private float displayDuration = 2f; // ����� ����� �����������
    [SerializeField] private float animationSpeed = 2f; // �������� �������� ����������/����������
    [SerializeField] private Vector3 maxScale = new Vector3(1.2f, 1.2f, 1f); // ������������ ������ ����������

    private Coroutine currentNotificationCoroutine;

    public void ShowNotification(string message)
    {
        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine); // ������������� ������� ��������
        }

        currentNotificationCoroutine = StartCoroutine(AnimateNotification(message));
    }

    private IEnumerator AnimateNotification(string message)
    {
        // ������������� ��������� ���������
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        notificationText.transform.localScale = Vector3.one;
        Color originalColor = notificationText.color;
        notificationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);

        // ���� ����������
        float elapsedTime = 0f;
        while (elapsedTime < displayDuration / 3f) // ������ ����� �������
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (displayDuration / 3f);
            notificationText.transform.localScale = Vector3.Lerp(Vector3.one, maxScale, progress);
            yield return null;
        }

        // ���� ����������
        elapsedTime = 0f;
        while (elapsedTime < displayDuration / 3f) // ������ ����� �������
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (displayDuration / 3f);
            notificationText.transform.localScale = Vector3.Lerp(maxScale, Vector3.one, progress);
            yield return null;
        }

        // ���� ������������
        elapsedTime = 0f;
        while (elapsedTime < displayDuration / 3f) // ��������� ����� �������
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (displayDuration / 3f);
            notificationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - progress);
            yield return null;
        }

        // �������� �����
        notificationText.gameObject.SetActive(false);
    }
}
