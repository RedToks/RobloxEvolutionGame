using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HairButton : MonoBehaviour
{
    public Image hairIcon;
    public TextMeshProUGUI priceText;
    public Button buyButton;
    public GameObject checkmark;

    public TMP_SpriteAsset brainCoinSpriteAsset;
    public TMP_SpriteAsset coinCoinSpriteAsset;

    private int hairIndex;
    private HairManager hairManager;
    private Hair.CurrencyType priceType;

    public Color affordableColor = Color.green;
    public Color unaffordableColor = Color.red;

    private void OnEnable()
    {
        if (BrainCurrency.Instance != null)
        {
            BrainCurrency.Instance.OnCurrencyChanged += UpdatePriceColor;
        }
        if (NeuroCurrency.Instance != null)
        {
            NeuroCurrency.Instance.OnCurrencyChanged += UpdatePriceColor;
        }

        UpdatePriceColor(); // ✅ Сразу обновляем цвет при включении кнопки
    }

    private void OnDisable()
    {
        if (BrainCurrency.Instance != null)
        {
            BrainCurrency.Instance.OnCurrencyChanged -= UpdatePriceColor;
        }
        if (NeuroCurrency.Instance != null)
        {
            NeuroCurrency.Instance.OnCurrencyChanged -= UpdatePriceColor;
        }
    }

    public void Setup(Hair hair, int index, HairManager manager, bool isSelected)
    {
        hairManager = manager;
        hairIndex = index;
        priceType = hair.priceType;

        hairIcon.sprite = hair.icon;
        buyButton.onClick.RemoveAllListeners(); // Удаляем старые слушатели

        if (hair.isPurchased)
        {
            priceText.text = ""; // Убираем цену
            buyButton.onClick.AddListener(() => hairManager.ActivateHair(hairIndex));
        }
        else
        {
            priceText.text = FormatPriceWithIcon(hair);
            UpdatePriceColor();
            buyButton.onClick.AddListener(() => hairManager.BuyHair(hairIndex, priceType));
        }

        checkmark.SetActive(isSelected);
    }

    private void UpdatePriceColor()
    {
        long playerCurrency = (priceType == Hair.CurrencyType.BrainCoin)
            ? BrainCurrency.Instance.brainCurrency
            : NeuroCurrency.Instance.coinCurrency;

        long price = (priceType == Hair.CurrencyType.BrainCoin)
            ? hairManager.hairs[hairIndex].brainCoinPrice
            : hairManager.hairs[hairIndex].coinCoinPrice;

        priceText.color = (playerCurrency >= price) ? affordableColor : unaffordableColor;
    }


    private string FormatPriceWithIcon(Hair hair)
    {
        string spriteTag = (hair.priceType == Hair.CurrencyType.BrainCoin) ? "<sprite name=Brain>" : "<sprite name=Neuro>";
        long price = (hair.priceType == Hair.CurrencyType.BrainCoin) ? hair.brainCoinPrice : hair.coinCoinPrice;
        return $"{spriteTag}{CurrencyFormatter.FormatCurrency(price)}";
    }
}
