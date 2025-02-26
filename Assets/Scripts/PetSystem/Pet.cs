using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class Pet
{
    public enum PetRarity
    {
        Common,
        Rare,
        Mythic,
        Special
    }

    public string Name; // ��� �������
    public Sprite Icon; // ������ �������
    public GameObject Prefab; // ������ �������
    public float Power; // ���� �������
    public PetRarity Rarity;

    public Pet(string name, Sprite icon, GameObject prefab, float power, PetRarity rarity)
    {
        Name = name;
        Icon = icon;
        Prefab = prefab;
        Power = power;
        Rarity = rarity;
    }
}