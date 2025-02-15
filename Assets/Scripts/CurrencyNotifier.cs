using System.Collections;
using TMPro;
using UnityEngine;

public class CurrencyNotifier : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI notificationText; // Ссылка на текстовый элемент
    [SerializeField] private float displayDuration = 2f; // Общее время отображения
    [SerializeField] private float animationSpeed = 2f; // Скорость анимации увеличения/уменьшения
    [SerializeField] private Vector3 maxScale = new Vector3(1.2f, 1.2f, 1f); // Максимальный размер увеличения

    private Coroutine currentNotificationCoroutine;

    public void ShowNotification(string message)
    {
        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine); // Останавливаем текущую анимацию
        }

        currentNotificationCoroutine = StartCoroutine(AnimateNotification(message));
    }

    private IEnumerator AnimateNotification(string message)
    {
        // Устанавливаем начальные параметры
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        notificationText.transform.localScale = Vector3.one;
        Color originalColor = notificationText.color;
        notificationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);

        // Фаза увеличения
        float elapsedTime = 0f;
        while (elapsedTime < displayDuration / 3f) // Первая треть времени
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (displayDuration / 3f);
            notificationText.transform.localScale = Vector3.Lerp(Vector3.one, maxScale, progress);
            yield return null;
        }

        // Фаза уменьшения
        elapsedTime = 0f;
        while (elapsedTime < displayDuration / 3f) // Вторая треть времени
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (displayDuration / 3f);
            notificationText.transform.localScale = Vector3.Lerp(maxScale, Vector3.one, progress);
            yield return null;
        }

        // Фаза исчезновения
        elapsedTime = 0f;
        while (elapsedTime < displayDuration / 3f) // Последняя треть времени
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / (displayDuration / 3f);
            notificationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - progress);
            yield return null;
        }

        // Скрываем текст
        notificationText.gameObject.SetActive(false);
    }
}
