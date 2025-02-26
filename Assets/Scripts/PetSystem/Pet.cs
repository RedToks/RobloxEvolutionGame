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

    public string Name; // Имя питомца
    public Sprite Icon; // Иконка питомца
    public GameObject Prefab; // Префаб питомца
    public float Power; // Сила питомца
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