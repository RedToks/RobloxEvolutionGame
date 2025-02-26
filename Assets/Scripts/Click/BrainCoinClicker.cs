using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using CoppraGames;

public class BrainCoinClicker : MonoBehaviour
{
    public GameObject floatingTextPrefab; // Префаб всплывающего текста
    public Canvas canvas; // Canvas (Screen Space - Camera)
    public Camera uiCamera; // Камера UI
    public int baseBrainCoinsPerClick = 10; // Базовое количество за клик
    public TextMeshProUGUI brainCoinPerClick;
    public RectTransform spawnArea; // Область спавна
    public RectTransform brainCoinTarget; // Куда летит текст
    public float clickCooldown = 0.2f; // Ограничение по времени обычного клика
    public float autoClickCooldown = 0.5f; // Ограничение по времени автоклика

    private float lastClickTime = 0f; // Время последнего клика
    private bool isAutoClickerActive = false; // Флаг активации автокликера
    private Coroutine autoClickCoroutine; // Ссылка на корутину автокликера

    private void Start()
    {
        if (uiCamera == null)
            uiCamera = Camera.main;

        ClickMultiplier.Instance.OnMultiplierChanged += UpdateBrainCoinText;
        UpdateBrainCoinText(ClickMultiplier.Instance.TotalMultiplier);
    }

    private void OnDestroy()
    {
        ClickMultiplier.Instance.OnMultiplierChanged -= UpdateBrainCoinText; // Отписка при удалении
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAutoClickerActive) // Игрок не может кликать, если автокликер активен
        {
            AttemptClick();
        }
    }

    private void UpdateBrainCoinText(float newMultiplier)
    {
        if (brainCoinPerClick != null)
        {
            int totalCoins = Mathf.RoundToInt(baseBrainCoinsPerClick * ClickMultiplier.Instance.TotalMultiplier);
            brainCoinPerClick.text = $"{totalCoins}";
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
        QuestManager.instance.OnAchieveQuestGoal(QuestManager.QuestGoals.CLICK_1000_TIMES);
    }

    private void GiveBrainCoins()
    {
        int totalCoins = Mathf.RoundToInt(baseBrainCoinsPerClick * ClickMultiplier.Instance.TotalMultiplier);
        BrainCurrency.Instance.AddBrainCurrency(totalCoins);
        CreateFloatingText(totalCoins);
    }

    private void CreateFloatingText(int coins)
    {
        if (spawnArea == null || brainCoinTarget == null)
        {
            Debug.LogError("Не назначена область спавна или цель!");
            return;
        }

        GameObject floatingText = Instantiate(floatingTextPrefab, spawnArea);
        RectTransform floatingTextRect = floatingText.GetComponent<RectTransform>();

        Vector2 randomLocalPos = new Vector2(
            Random.Range(-spawnArea.rect.width / 2, spawnArea.rect.width / 2),
            Random.Range(-spawnArea.rect.height / 2, spawnArea.rect.height / 2)
        );

        floatingTextRect.anchoredPosition = randomLocalPos;

        TextMeshProUGUI textComponent = floatingText.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = $"+{CurrencyFormatter.FormatCurrency(coins)}";
        }

        DOVirtual.DelayedCall(0.5f, () =>
        {
            floatingTextRect
                .DOAnchorPos(brainCoinTarget.anchoredPosition, 1f)
                .SetEase(Ease.InOutQuad);

            floatingTextRect
                .DOScale(Vector3.zero, 1f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => Destroy(floatingText));
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
