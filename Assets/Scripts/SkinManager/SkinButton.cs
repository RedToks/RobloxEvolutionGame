using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinButton : MonoBehaviour
{
    public Image skinIcon;
    public TextMeshProUGUI priceText;
    public Button buyButton;
    public GameObject checkmark;

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
        buyButton.onClick.RemoveAllListeners();

        if (skin.isPurchased)
        {
            Debug.Log($"✅ Скрываем цену для купленного скина {skin.name} (индекс {index})");
            priceText.gameObject.SetActive(false);  // Теперь точно скрываем цену!
            buyButton.onClick.AddListener(() => skinManager.ActivateSkin(skinIndex));
        }
        else
        {
            priceText.gameObject.SetActive(true);
            UpdatePriceColor(skin);
            buyButton.onClick.AddListener(() => skinManager.BuySkin(skinIndex));
        }

        checkmark.SetActive(isSelected);
    }

    public void UpdateState(SkinData skin, bool isSelected)
    {
        skinIcon.sprite = skin.icon;

        if (skin.isPurchased)
        {
            priceText.text = "";
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => skinManager.ActivateSkin(skinIndex));
        }

        checkmark.SetActive(isSelected);
    }

    public void UpdatePriceColor(SkinData skin)
    {
        long playerCurrency = (priceType == SkinData.CurrencyType.BrainCoin) ?
            BrainCurrency.Instance.brainCurrency : NeuroCurrency.Instance.coinCurrency;
        long price = (priceType == SkinData.CurrencyType.BrainCoin) ?
            skin.brainCoinPrice : skin.coinCoinPrice;

        if (!skin.isPurchased)
        {
            priceText.text = FormatPriceWithIcon(price, priceType);
            priceText.color = (playerCurrency >= price) ? affordableColor : unaffordableColor;
        }
    }

    private string FormatPriceWithIcon(long price, SkinData.CurrencyType currencyType)
    {
        string spriteTag = (currencyType == SkinData.CurrencyType.BrainCoin) ? "<sprite name=Brain>" : "<sprite name=Neuro>";
        return $"{spriteTag}{CurrencyFormatter.FormatCurrency(price)}";
    }

    public void SetSelected(bool isSelected)
    {
        checkmark.SetActive(isSelected);
    }
}
