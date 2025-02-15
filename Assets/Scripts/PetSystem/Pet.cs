using UnityEngine;

[System.Serializable]
public class Pet
{
    public string Name; // ��� �������
    public Sprite Icon; // ������ �������
    public GameObject Prefab; // ������ �������
    public float Power; // ���� �������

    public Pet(string name, Sprite icon, GameObject prefab, float power)
    {
        Name = name;
        Icon = icon;
        Prefab = prefab;
        Power = power;
    }
}