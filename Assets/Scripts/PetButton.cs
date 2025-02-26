using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PetButton : MonoBehaviour
{
    public Image petImage; // ������ �� ����������� �������
    public TextMeshProUGUI petMultiplierText; // ������ �� ����� ���������
    private Button button; // ������


    // ����� ��� ��������� ������ �������
    public void SetPet(Pet pet)
    {
        petImage.sprite = pet.Icon;
        petMultiplierText.text = $"x{pet.Power}";
        button = GetComponent<Button>();
        button.image.color = GetRarityColor(pet.Rarity);
    }


    // ���������� ���� ������ � ����������� �� ��������
    private Color GetRarityColor(Pet.PetRarity rarity)
    {
        switch (rarity)
        {
            case Pet.PetRarity.Common: return Color.white;
            case Pet.PetRarity.Rare: return Color.blue;
            case Pet.PetRarity.Mythic: return Color.red;
            case Pet.PetRarity.Special: return Color.yellow;
            default: return Color.white;
        }
    }
}
