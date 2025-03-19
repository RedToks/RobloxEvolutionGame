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

    public Sprite Icon;
    public GameObject Prefab;
    public float Power;
    public PetRarity Rarity;

    public override bool Equals(object obj)
    {
        if (obj is Pet otherPet)
        {
            return this.Prefab != null && otherPet.Prefab != null && this.Prefab.name == otherPet.Prefab.name;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Prefab.name.GetHashCode();
    }

    public Pet(Sprite icon, GameObject prefab, float power, PetRarity rarity)
    {
        Icon = icon;
        Prefab = prefab;
        Power = power;
        Rarity = rarity;
    }
}
