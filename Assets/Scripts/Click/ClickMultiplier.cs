using System;
using UnityEngine;

public class ClickMultiplier : MonoBehaviour
{
    public static ClickMultiplier Instance;

    private float clickMultiplier = 1f; // ��������� �� ������
    private float petMultiplier = 1f;  // ��������� �� ��������
    private float otherMultiplier = 1f; // �������������� ��������� (���� �������� � �������)

    public float TotalMultiplier => clickMultiplier * petMultiplier * otherMultiplier; // ����� �������

    public event Action<float> OnMultiplierChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetClickMultiplier(float multiplier)
    {
        clickMultiplier = multiplier;
        ApplyMultiplier();
    }

    public void SetPetMultiplier(float multiplier)
    {
        petMultiplier = multiplier;
        ApplyMultiplier();
    }

    public void SetOtherMultiplier(float multiplier)
    {
        otherMultiplier = multiplier;
        ApplyMultiplier();
    }

    private void ApplyMultiplier()
    {
        OnMultiplierChanged?.Invoke(TotalMultiplier);
    }

    public float ApplyClickMultiplier(float baseClickValue)
    {
        return baseClickValue * TotalMultiplier;
    }
}
