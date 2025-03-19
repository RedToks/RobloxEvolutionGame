using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using YG;

public class MultiplierButton : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI multiplierText;
    public Button activateButton;
    public GameObject lockPanel;
    public TextMeshProUGUI costText;
    public GameObject checkmarkImage;

    public float multiplier { get; private set; }
    private long unlockCost;
    private bool isActivated = false;


    public void Setup(Sprite newIcon, float newMultiplier, long newCost)
    {
        icon.sprite = newIcon;
        multiplier = newMultiplier;
        unlockCost = newCost;

        multiplierText.text = $"x{multiplier}";
        costText.text = CurrencyFormatter.FormatCurrency(unlockCost);

        LoadState();
        UpdateState();
    }

    private void OnEnable()
    {
        if (BrainCurrency.Instance != null)
        {
            BrainCurrency.Instance.OnCurrencyChanged += UpdateState;
        }
        UpdateState();
    }

    private void OnDisable()
    {
        if (BrainCurrency.Instance != null)
        {
            BrainCurrency.Instance.OnCurrencyChanged -= UpdateState;
        }
    }

    public void Activate()
    {
        ClickMultiplier.Instance.SetClickMultiplier(multiplier);
        isActivated = true;
        YG2.saves.selectedMultiplier = multiplier; // ✅ Запоминаем выбранный множитель

        UpdateState();
    }

    public void Deactivate()
    {
        isActivated = false;
        UpdateState();
    }

    private void UpdateState()
    {
        bool isUnlocked = YG2.saves.unlockedMultipliers.Contains(multiplier);
        multiplierText.gameObject.SetActive(isUnlocked);
        lockPanel.SetActive(!isUnlocked);
        activateButton.interactable = isUnlocked;
        activateButton.gameObject.SetActive(!isActivated);
        checkmarkImage.SetActive(isActivated);
    }

    private void LoadState()
    {
        bool isUnlocked = YG2.saves.unlockedMultipliers.Contains(multiplier);
        float savedMultiplier = YG2.saves.selectedMultiplier;

        if (isUnlocked && multiplier == savedMultiplier)
        {
            isActivated = true;
            ClickMultiplier.Instance.SetClickMultiplier(multiplier);
        }
    }

    public void UnlockMultiplier()
    {
        if (!YG2.saves.unlockedMultipliers.Contains(multiplier))
        {
            YG2.saves.unlockedMultipliers.Add(multiplier);
        }
        UpdateState();
    }
}
