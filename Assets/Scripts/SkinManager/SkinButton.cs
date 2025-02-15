using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinButton : MonoBehaviour
{
    public Image skinIcon; // Иконка скина
    public TextMeshProUGUI priceText; // Цена или "Активировать"
    public Button buyButton; // Кнопка покупки
    public GameObject checkmark; // Галочка ✅

    private int skinIndex;
    private SkinManager skinManager;

    public void Setup(SkinData skin, int index, SkinManager manager, bool isSelected)
    {
        skinManager = manager;
        skinIndex = index;

        skinIcon.sprite = skin.icon;
        priceText.text = skin.isPurchased ? "Выбрать" : skin.price + " монет";
        buyButton.onClick.AddListener(() => skinManager.BuySkin(skinIndex));

        // Показываем галочку, если этот скин активен
        checkmark.SetActive(isSelected);
    }
}
