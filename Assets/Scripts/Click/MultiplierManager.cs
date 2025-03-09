using UnityEngine;
using UnityEngine.UI;

public class MultiplierManager : MonoBehaviour
{
    public GameObject multiplierButtonPrefab;
    public Transform buttonParent;
    public MultiplierData[] multipliers;
    public NotificationIcon notificationIcon;
    public GameObject clickPanel;
    public Image bottomMultiplierIcon;

    private MultiplierButton activeMultiplierButton;
    private bool hasNewUnlockedMultiplier = false; // 🔹 Флаг нового множителя

    private void Start()
    {
        foreach (MultiplierData data in multipliers)
        {
            data.ParseUnlockCost();

            GameObject newButton = Instantiate(multiplierButtonPrefab, buttonParent);
            MultiplierButton buttonScript = newButton.GetComponent<MultiplierButton>();

            buttonScript.Setup(data.icon, data.multiplier, data.unlockCost);
            buttonScript.activateButton.onClick.AddListener(() => ActivateMultiplier(buttonScript));
        }

        // 🔹 Проверяем множители при старте
        CheckForNewMultipliers();

        // 🔹 Подписка на обновление валюты
        if (BrainCurrency.Instance != null)
        {
            BrainCurrency.Instance.OnCurrencyChanged += CheckForNewMultipliers;
        }
    }

    private void OnDestroy()
    {
        if (BrainCurrency.Instance != null)
        {
            BrainCurrency.Instance.OnCurrencyChanged -= CheckForNewMultipliers;
        }
    }

    private void ActivateMultiplier(MultiplierButton newButton)
    {
        if (activeMultiplierButton != null)
        {
            activeMultiplierButton.Deactivate();
        }

        activeMultiplierButton = newButton;
        activeMultiplierButton.Activate();

        bottomMultiplierIcon.sprite = newButton.icon.sprite;

        // 🔹 Сбрасываем уведомление при активации множителя
        hasNewUnlockedMultiplier = false;
        notificationIcon.SetNotification(false);
    }

    // 🔹 Проверяем, появился ли новый множитель
    private void CheckForNewMultipliers()
    {
        bool foundNewMultiplier = false;

        foreach (MultiplierData data in multipliers)
        {
            bool isUnlocked = BrainCurrency.Instance.brainCurrency >= data.unlockCost;
            bool wasLockedBefore = PlayerPrefs.GetInt($"Multiplier_{data.multiplier}_Unlocked", 0) == 0;

            if (isUnlocked && wasLockedBefore)
            {
                foundNewMultiplier = true;
                PlayerPrefs.SetInt($"Multiplier_{data.multiplier}_Unlocked", 1); // Сохраняем, что множитель разблокирован
            }
        }

        if (foundNewMultiplier)
        {
            hasNewUnlockedMultiplier = true;
            notificationIcon.SetNotification(true); // Включаем уведомление
        }
    }

    // 🔹 Вызываем этот метод, когда игрок открывает панель множителей
    public void OnMultiplierPanelOpened()
    {
        hasNewUnlockedMultiplier = false;
        notificationIcon.SetNotification(false);
    }

    public void OpenClickPanel()
    {
        OnMultiplierPanelOpened();
        clickPanel.SetActive(true);
    }
}
