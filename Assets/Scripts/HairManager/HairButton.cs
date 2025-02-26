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

    public void Setup(Hair hair, int index, HairManager manager, bool isSelected)
    {
        hairManager = manager;
        hairIndex = index;
        priceType = hair.priceType;

        hairIcon.sprite = hair.icon;

        if (hair.isPurchased)
        {
            priceText.text = "";
            buyButton.onClick.AddListener(() => hairManager.ActivateHair(hairIndex));
        }
        else
        {
            long playerCurrency = (priceType == Hair.CurrencyType.BrainCoin) ? BrainCurrency.Instance.brainCurrency : NeuroCurrency.Instance.coinCurrency;
            long price = (priceType == Hair.CurrencyType.BrainCoin) ? hair.brainCoinPrice : hair.coinCoinPrice;

            priceText.text = FormatPriceWithIcon(price, priceType);
            priceText.color = (playerCurrency >= price) ? affordableColor : unaffordableColor;

            buyButton.onClick.AddListener(() => hairManager.BuyHair(hairIndex, priceType));
        }

        checkmark.SetActive(isSelected);
    }

    private string FormatPriceWithIcon(long price, Hair.CurrencyType currencyType)
    {
        string spriteTag = (currencyType == Hair.CurrencyType.BrainCoin) ? "<sprite name=Brain>" : "<sprite name=Neuro>";
        return $"{spriteTag}{CurrencyFormatter.FormatCurrency(price)}";
    }
}
