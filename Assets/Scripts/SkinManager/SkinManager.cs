using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SkinManager : MonoBehaviour
{
    public SkinnedMeshRenderer playerRenderer; // ������ ������
    public List<SkinData> skins; // ������ ������
    public GameObject skinButtonPrefab; // ������ ������
    public Transform contentPanel; // ��������� ������ (Grid Layout Group)
    private int coins;

    private void Start()
    {
        LoadSkinStates();
        GenerateSkinButtons();
        ActivateSkin(PlayerPrefs.GetInt("SelectedSkin", 0));
    }

    private void GenerateSkinButtons()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        int selectedSkin = PlayerPrefs.GetInt("SelectedSkin", 0);

        for (int i = 0; i < skins.Count; i++)
        {
            GameObject newButton = Instantiate(skinButtonPrefab, contentPanel);
            bool isSelected = (i == selectedSkin);
            newButton.GetComponent<SkinButton>().Setup(skins[i], i, this, isSelected);
        }
    }


    public void BuySkin(int index)
    {
        if (index >= 0 && index < skins.Count)
        {
            if (skins[index].isPurchased)
            {
                ActivateSkin(index);
            }
            else if (coins >= skins[index].price)
            {
                coins -= skins[index].price;
                skins[index].isPurchased = true;
                PlayerPrefs.SetInt($"Skin_{index}", 1); // ��������� �������
                PlayerPrefs.SetInt("Coins", coins);
                ActivateSkin(index);
            }
            GenerateSkinButtons(); // ��������� ������ ����� �������
        }
    }

    public void ActivateSkin(int index)
    {
        if (skins[index].isPurchased)
        {
            Material playerMaterial = playerRenderer.material;
            playerMaterial.mainTexture = skins[index].textureSprite.texture;
            PlayerPrefs.SetInt("SelectedSkin", index);
            GenerateSkinButtons(); // ����������� ������, �������� �������
        }
    }



    private void LoadSkinStates()
    {
        for (int i = 0; i < skins.Count; i++)
        {
            skins[i].isPurchased = (i == 0) || PlayerPrefs.GetInt($"Skin_{i}", 0) == 1;
        }

        // ���� ��� ������������ ���������� �����, �������� ������
        if (!PlayerPrefs.HasKey("SelectedSkin"))
        {
            PlayerPrefs.SetInt("SelectedSkin", 0);
        }
    }

}
