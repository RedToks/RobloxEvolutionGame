using UnityEngine;

[System.Serializable]
public class Pet
{
    public string Name; // Имя питомца
    public Sprite Icon; // Иконка питомца
    public GameObject Prefab; // Префаб питомца
    public float Power; // Сила питомца

    public Pet(string name, Sprite icon, GameObject prefab, float power)
    {
        Name = name;
        Icon = icon;
        Prefab = prefab;
        Power = power;
    }
}