using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;

public class BrainCoinClicker : MonoBehaviour
{
    public GameObject floatingTextPrefab; // Префаб всплывающего текста
    public Canvas canvas; // Canvas (Screen Space - Camera)
    public Camera uiCamera; // Камера UI
    public int brainCoinsPerClick = 10; // Количество за клик
    public RectTransform spawnArea; // Область спавна
    public RectTransform brainCoinTarget; // Куда летит текст
    public float clickCooldown = 0.2f; // Ограничение по времени обычного клика
    public float autoClickCooldown = 0.5f; // Ограничение по времени автоклика (можно настроить в инспекторе)

    private float lastClickTime = 0f; // Время последнего клика
    private bool isAutoClickerActive = false; // Флаг активации автокликера
    private Coroutine autoClickCoroutine; // Ссылка на корутину автокликера

    private void Start()
    {
        if (uiCamera == null)
            uiCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAutoClickerActive) // Игрок не может кликать, если автокликер активен
        {
            AttemptClick();
        }
    }

    private void AttemptClick()
    {
        // Проверяем, прошло ли достаточно времени с последнего клика
        if (Time.time - lastClickTime < clickCooldown)
        {
            return;
        }

        lastClickTime = Time.time; // Обновляем время последнего клика
        GiveBrainCoins();
    }

    private void GiveBrainCoins()
    {
        BrainCurrency.Instance.AddBrainCurrency(brainCoinsPerClick);
        CreateFloatingText();
    }

    private void CreateFloatingText()
    {
        if (spawnArea == null || brainCoinTarget == null)
        {
            Debug.LogError("Не назначена область спавна или цель!");
            return;
        }

        // Создаём текст
        GameObject floatingText = Instantiate(floatingTextPrefab, spawnArea);
        RectTransform floatingTextRect = floatingText.GetComponent<RectTransform>();

        // Генерируем случайную позицию в области (локальные координаты)
        Vector2 randomLocalPos = new Vector2(
            Random.Range(-spawnArea.rect.width / 2, spawnArea.rect.width / 2),
            Random.Range(-spawnArea.rect.height / 2, spawnArea.rect.height / 2)
        );

        floatingTextRect.anchoredPosition = randomLocalPos; // Используем anchoredPosition

        // Устанавливаем текст
        TextMeshProUGUI textComponent = floatingText.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = $"+{CurrencyFormatter.FormatCurrency(brainCoinsPerClick)}";
        }

        // 🔵 Добавляем задержку 0.5 сек перед анимацией
        DOVirtual.DelayedCall(0.5f, () =>
        {
            // Анимация движения к BrainCoins
            floatingTextRect
                .DOAnchorPos(brainCoinTarget.anchoredPosition, 1f)
                .SetEase(Ease.InOutQuad);

            // Анимация уменьшения
            floatingTextRect
                .DOScale(Vector3.zero, 1f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => Destroy(floatingText)); // Удаляем объект после анимации
        });
    }

    // 🔥 Метод для включения/выключения автокликера
    public void ToggleAutoClicker()
    {
        if (isAutoClickerActive)
        {
            StopAutoClicker();
        }
        else
        {
            StartAutoClicker();
        }
    }

    // ✅ Запуск автокликера
    private void StartAutoClicker()
    {
        if (autoClickCoroutine == null)
        {
            autoClickCoroutine = StartCoroutine(AutoClickRoutine());
            isAutoClickerActive = true;
        }
    }

    // ⛔ Остановка автокликера
    private void StopAutoClicker()
    {
        if (autoClickCoroutine != null)
        {
            StopCoroutine(autoClickCoroutine);
            autoClickCoroutine = null;
        }
        isAutoClickerActive = false;
    }

    // 🔄 Корутин для автоматического клика (с отдельной задержкой)
    private IEnumerator AutoClickRoutine()
    {
        while (true)
        {
            GiveBrainCoins();
            yield return new WaitForSeconds(autoClickCooldown); // Автоклик с индивидуальной задержкой
        }
    }
}
