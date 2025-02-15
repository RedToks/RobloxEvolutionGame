using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HairButton : MonoBehaviour
{
    public Image hairIcon;
    public TextMeshProUGUI priceText;
    public Button buyButton;
    public GameObject checkmark; // Галочка ✅

    private int hairIndex;
    private HairManager hairManager;

    public void Setup(Hair hair, int index, HairManager manager, bool isSelected)
    {
        hairManager = manager;
        hairIndex = index;

        hairIcon.sprite = hair.icon;
        priceText.text = hair.isPurchased ? "Выбрать" : hair.price + " монет";
        buyButton.onClick.AddListener(() => hairManager.BuyHair(hairIndex));

        checkmark.SetActive(isSelected); // Галочка на активной прическе
    }
}
