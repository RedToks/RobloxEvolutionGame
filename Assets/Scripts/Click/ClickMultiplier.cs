using System;
using UnityEngine;
using YG;

public class ClickMultiplier : MonoBehaviour
{
    public static ClickMultiplier Instance;

    private float clickMultiplier = 1f; // Множитель от кнопки
    private float petMultiplier = 1f;   // Множитель от питомцев
    private float otherMultiplier = 1f; // Дополнительные множители (если появятся в будущем)

    public float TotalMultiplier => clickMultiplier * petMultiplier * otherMultiplier; // Общая формула

    public event Action<float> OnMultiplierChanged;

    private void Start()
    {
        LoadMultipliers();
        InvokeRepeating(nameof(SaveMultipliers), 10f, 10f); // 🔹 Автосохранение раз в 10 секунд
    }

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

    private void SaveMultipliers()
    {
        YG2.saves.petMultiplier = Mathf.RoundToInt(petMultiplier * 1000);
        YG2.saves.clickMultiplier = Mathf.RoundToInt(clickMultiplier * 1000);

        YG2.saves.isDoubleEarningsActive = BuyController.isDoubleEarningsActive;
    }

    private void LoadMultipliers()
    {
        // 🔹 Загружаем множители из YG2.saves
        int petMultInt = YG2.saves.petMultiplier;
        int clickMultInt = YG2.saves.clickMultiplier;

        petMultiplier = petMultInt > 0 ? petMultInt / 1000f : 1f; // 🔹 Делим обратно на 1000
        clickMultiplier = clickMultInt > 0 ? clickMultInt / 1000f : 1f; // 🔹 Делим обратно на 1000

        // 🔹 Загружаем x2 множитель из saves
        BuyController.isDoubleEarningsActive = YG2.saves.isDoubleEarningsActive;
        otherMultiplier = BuyController.isDoubleEarningsActive ? 2f : 1f;

        ApplyMultiplier();
    }

}
