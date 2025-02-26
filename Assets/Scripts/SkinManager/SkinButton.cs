using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinButton : MonoBehaviour
{
    public Image skinIcon; // Иконка скина
    public TextMeshProUGUI priceText; // Цена или "Выбрать"
    public Button buyButton; // Кнопка покупки
    public GameObject checkmark; // Галочка ✅

    private int skinIndex;
    private SkinManager skinManager;
    private SkinData.CurrencyType priceType;

    public Color affordableColor = Color.green;
    public Color unaffordableColor = Color.red;

    public void Setup(SkinData skin, int index, SkinManager manager, bool isSelected)
    {
        skinManager = manager;
        skinIndex = index;
        priceType = skin.priceType;

        skinIcon.sprite = skin.icon;

        if (skin.isPurchased)
        {
            priceText.text = "";
            buyButton.onClick.AddListener(() => skinManager.ActivateSkin(skinIndex));
        }
        else
        {
            long playerCurrency = (priceType == SkinData.CurrencyType.BrainCoin) ?
                BrainCurrency.Instance.brainCurrency : NeuroCurrency.Instance.coinCurrency;
            long price = (priceType == SkinData.CurrencyType.BrainCoin) ?
                skin.brainCoinPrice : skin.coinCoinPrice;

            priceText.text = FormatPriceWithIcon(price, priceType);
            priceText.color = (playerCurrency >= price) ? affordableColor : unaffordableColor;

            buyButton.onClick.AddListener(() => skinManager.BuySkin(skinIndex));
        }

        checkmark.SetActive(isSelected);
    }

    private string FormatPriceWithIcon(long price, SkinData.CurrencyType currencyType)
    {
        string spriteTag = (currencyType == SkinData.CurrencyType.BrainCoin) ? "<sprite name=Brain>" : "<sprite name=Neuro>";
        return $"{spriteTag}{CurrencyFormatter.FormatCurrency(price)}";
    }
}
