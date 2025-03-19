using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using YG;

public class SkinManager : MonoBehaviour
{
    public SkinnedMeshRenderer playerRenderer;
    public List<SkinData> skins;
    public GameObject skinButtonPrefab;
    public Transform contentPanel;

    private Dictionary<int, SkinButton> skinButtons = new Dictionary<int, SkinButton>();

    private void Start()
    {
        LoadSkinStates();
        GenerateSkinButtons();

        ActivateSkin(YG2.saves.selectedSkin);
    }

    private void OnEnable()
    {
        BrainCurrency.Instance.OnCurrencyChanged += UpdateSkinButtonStates;
        NeuroCurrency.Instance.OnCurrencyChanged += UpdateSkinButtonStates;
    }

    private void OnDisable()
    {
        BrainCurrency.Instance.OnCurrencyChanged -= UpdateSkinButtonStates;
        NeuroCurrency.Instance.OnCurrencyChanged -= UpdateSkinButtonStates;
    }

    private void GenerateSkinButtons()
    {
        Debug.Log("🔄 Генерация всех кнопок скинов...");
        skinButtons.Clear();

        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        int selectedSkin = YG2.saves.selectedSkin;

        for (int i = 0; i < skins.Count; i++)
        {
            GameObject newButton = Instantiate(skinButtonPrefab, contentPanel);
            SkinButton skinButton = newButton.GetComponent<SkinButton>();
            skinButton.Setup(skins[i], i, this, i == selectedSkin);
            skinButtons[i] = skinButton;
        }

        foreach (var pair in skinButtons)
        {
            pair.Value.UpdateState(skins[pair.Key], pair.Key == selectedSkin);
        }
    }

    public void BuySkin(int index)
    {
        if (index < 0 || index >= skins.Count)
        {
            Debug.LogError($"❌ Ошибка: индекс {index} вне диапазона!");
            return;
        }

        SkinData skin = skins[index];
        if (IsSkinPurchased(index))
        {
            ActivateSkin(index);
            return;
        }


        long playerCurrency = (skin.priceType == SkinData.CurrencyType.BrainCoin) ?
            BrainCurrency.Instance.brainCurrency : NeuroCurrency.Instance.coinCurrency;
        long price = (skin.priceType == SkinData.CurrencyType.BrainCoin) ?
            skin.brainCoinPrice : skin.coinCoinPrice;

        if (playerCurrency >= price)
        {
            if (skin.priceType == SkinData.CurrencyType.BrainCoin)
                BrainCurrency.Instance.SpendBrainCurrency(price);
            else
                NeuroCurrency.Instance.SpendCoinCurrency((int)price);

            MarkSkinAsPurchased(index);

            Debug.Log($"✅ Куплен скин {skin.name} (индекс {index})");
            ActivateSkin(index);

            if (skinButtons.TryGetValue(index, out SkinButton skinButton))
            {
                skinButton.UpdateState(skins[index], true); // Обновляем UI
            }
        }
        else
        {
            Debug.Log($"❌ Недостаточно валюты для покупки {skin.name}");
        }
    }

    public void ActivateSkin(int index)
    {
        if (index >= 0 && index < skins.Count && IsSkinPurchased(index))
        {
            playerRenderer.material.mainTexture = skins[index].textureSprite.texture;
            YG2.saves.selectedSkin = index;

            Debug.Log($"🎭 Активирован скин {skins[index].name} (индекс {index})");

            foreach (var pair in skinButtons)
            {
                pair.Value.SetSelected(pair.Key == index);
            }
        }
    }

    private void LoadSkinStates()
    {
        if (string.IsNullOrEmpty(YG2.saves.purchasedSkins))
        {
            YG2.saves.purchasedSkins = "1".PadRight(skins.Count, '0');
        }

        // Обновляем список купленных скинов
        char[] purchasedArray = YG2.saves.purchasedSkins.PadRight(skins.Count, '0').ToCharArray();
        for (int i = 0; i < skins.Count; i++)
        {
            skins[i].isPurchased = (purchasedArray[i] == '1');
        }
    }

    private bool IsSkinPurchased(int index)
    {
        return index == 0 || (index < YG2.saves.purchasedSkins.Length && YG2.saves.purchasedSkins[index] == '1');
    }

    private void MarkSkinAsPurchased(int index)
    {
        char[] skinDataArray = YG2.saves.purchasedSkins.PadRight(skins.Count, '0').ToCharArray();
        skinDataArray[index] = '1';
        YG2.saves.purchasedSkins = new string(skinDataArray);

        skins[index].isPurchased = true; // Обновляем флаг у объекта
    }

    private void UpdateSkinButtonStates()
    {
        Debug.Log("💰 Изменение валюты — обновляем только цены на кнопках!");

        foreach (var pair in skinButtons)
        {
            int index = pair.Key;
            SkinButton button = pair.Value;
            button.UpdatePriceColor(skins[index]);
        }
    }
}