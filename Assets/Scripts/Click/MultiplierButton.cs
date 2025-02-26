using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiplierButton : MonoBehaviour
{
    public Image icon; // Иконка множителя
    public TextMeshProUGUI multiplierText; // Текст множителя (например, x200)
    public Button activateButton; // Кнопка "Активировать"
    public GameObject lockPanel; // Панель с замком
    public TextMeshProUGUI costText; // Текст стоимости
    public GameObject checkmarkImage; // ✅ Галочка

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

        UpdateState();
    }

    private void OnEnable()
    {
        if (BrainCurrency.Instance != null)
        {
            BrainCurrency.Instance.OnCurrencyChanged += UpdateState;
        }
        UpdateState(); // Чтобы сразу проверить состояние при старте
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
        Debug.Log($"Активирован множитель: x{multiplier}");

        isActivated = true;
        UpdateState();
    }

    public void Deactivate()
    {
        isActivated = false;
        UpdateState();
    }

    private void UpdateState()
    {
        bool isUnlocked = BrainCurrency.Instance.brainCurrency >= unlockCost;
        // Отображение множителя, если он разблокирован
        multiplierText.gameObject.SetActive(isUnlocked);

        // Управляем замком и доступностью кнопки
        lockPanel.SetActive(!isUnlocked);
        activateButton.interactable = isUnlocked;

        // Управляем отображением кнопки и галочки
        activateButton.gameObject.SetActive(!isActivated);
        checkmarkImage.SetActive(isActivated);
    }


}
