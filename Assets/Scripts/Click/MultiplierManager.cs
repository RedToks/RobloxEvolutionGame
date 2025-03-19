using UnityEngine;
using UnityEngine.UI;
using YG;
using System.Collections;

public class MultiplierManager : MonoBehaviour
{
    public GameObject multiplierButtonPrefab;
    public Transform buttonParent;
    public MultiplierData[] multipliers;
    public NotificationIcon notificationIcon;
    public GameObject clickPanel;
    public Image bottomMultiplierIcon;

    private MultiplierButton activeMultiplierButton;
    private bool hasNewUnlockedMultiplier = false;
    private static bool isSavingAllowed = true; // 🔹 Флаг разрешения сохранения

    private void Start()
    {
        StartCoroutine(AutoSaveRoutine()); // 🔹 Запуск автосохранения раз в 10 секунд

        foreach (MultiplierData data in multipliers)
        {
            data.ParseUnlockCost();

            GameObject newButton = Instantiate(multiplierButtonPrefab, buttonParent);
            MultiplierButton buttonScript = newButton.GetComponent<MultiplierButton>();

            buttonScript.Setup(data.icon, data.multiplier, data.unlockCost);
            buttonScript.activateButton.onClick.AddListener(() => ActivateMultiplier(buttonScript));

            // 🔹 Загружаем сохраненный множитель
            if (YG2.saves.selectedMultiplier == data.multiplier)
            {
                ActivateMultiplier(buttonScript, false);
            }
        }

        CheckForNewMultipliers();

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

    private void ActivateMultiplier(MultiplierButton newButton, bool save = true)
    {
        if (activeMultiplierButton != null)
        {
            activeMultiplierButton.Deactivate();
        }

        activeMultiplierButton = newButton;
        activeMultiplierButton.Activate();
        bottomMultiplierIcon.sprite = newButton.icon.sprite;

        // 🔹 Сохраняем множитель, но с отложенным сохранением
        if (save)
        {
            YG2.saves.selectedMultiplier = newButton.multiplier;
            RequestSave(); // 🔹 Отложенное сохранение
        }

        hasNewUnlockedMultiplier = false;
        notificationIcon.SetNotification(false);
    }

    private void CheckForNewMultipliers()
    {
        bool foundNewMultiplier = false;

        foreach (MultiplierData data in multipliers)
        {
            bool isUnlocked = BrainCurrency.Instance.brainCurrency >= data.unlockCost;
            bool wasLockedBefore = !YG2.saves.unlockedMultipliers.Contains(data.multiplier);

            if (isUnlocked && wasLockedBefore)
            {
                foundNewMultiplier = true;
                YG2.saves.unlockedMultipliers.Add(data.multiplier);
                RequestSave(); // 🔹 Отложенное сохранение
            }
        }

        if (foundNewMultiplier)
        {
            hasNewUnlockedMultiplier = true;
            notificationIcon.SetNotification(true);
        }
    }

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

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            if (!isSavingAllowed) continue;
            YG2.SaveProgress();
            isSavingAllowed = false;
        }
    }

    // 🔹 Метод для запроса сохранения
    private void RequestSave()
    {
        isSavingAllowed = true;
    }
}
