using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using CoppraGames;
using YG;

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
    public Button autoClickButton; // Кнопка автокликера

    [Header("Настройки цветов кнопки автокликера")]
    public Color activeColor = Color.green;  // Цвет при включенном автокликере
    public Color inactiveColor = Color.red;  // Цвет при выключенном автокликере

    private float lastClickTime = 0f; // Время последнего клика
    private bool isAutoClickerActive = false; // Флаг активации автокликера
    private Coroutine autoClickCoroutine; // Ссылка на корутину автокликера

    private void OnEnable()
    {
        if (ClickMultiplier.Instance != null)
        {
            ClickMultiplier.Instance.OnMultiplierChanged += UpdateBrainCoinText;
            UpdateBrainCoinText(ClickMultiplier.Instance.TotalMultiplier); // 🔹 Обновляем при включении Canvas
        }
    }

    private void OnDisable()
    {
        if (ClickMultiplier.Instance != null)
        {
            ClickMultiplier.Instance.OnMultiplierChanged -= UpdateBrainCoinText;
        }
    }

    private void Start()
    {
        if (uiCamera == null)
            uiCamera = Camera.main;

        ClickMultiplier.Instance.OnMultiplierChanged += UpdateBrainCoinText;
        UpdateBrainCoinText(ClickMultiplier.Instance.TotalMultiplier);
        UpdateAutoClickButtonColor(); // Обновляем цвет кнопки при старте
    }

    private void OnDestroy()
    {
        ClickMultiplier.Instance.OnMultiplierChanged -= UpdateBrainCoinText;
    }

    private void Update()
    {
        if (Time.timeScale == 0) { return; }

        if (Input.GetMouseButtonDown(0) && !isAutoClickerActive)
        {
            AttemptClick();
        }
    }

    private void UpdateBrainCoinText(float newMultiplier)
    {
        if (brainCoinPerClick != null)
        {
            int totalCoins = Mathf.RoundToInt(baseBrainCoinsPerClick * ClickMultiplier.Instance.TotalMultiplier);

            string lang = YG2.lang;

            string text = lang == "ru" ? $"+{CurrencyFormatter.FormatCurrency(totalCoins)} / клик"
                                       : $"+{CurrencyFormatter.FormatCurrency(totalCoins)} / click";

            brainCoinPerClick.text = text;
        }
    }

    private void AttemptClick()
    {
        if (Time.time - lastClickTime < clickCooldown)
        {
            return;
        }

        lastClickTime = Time.time;
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
            textComponent.text = $"<sprite name=\"Brain\">+{CurrencyFormatter.FormatCurrency(coins)}";
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

        UpdateAutoClickButtonColor(); // Меняем цвет кнопки при включении/выключении
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
            yield return new WaitForSeconds(autoClickCooldown);
        }
    }

    private void UpdateAutoClickButtonColor()
    {
        if (autoClickButton != null)
        {
            autoClickButton.image.color = isAutoClickerActive ? activeColor : inactiveColor;
        }
    }
}
