using UnityEngine;
using YG;
using TMPro; // Для работы с текстом

public class BuyController : MonoBehaviour
{
    public static bool isDoubleEarningsActive = false; // Флаг x2 BrainCoins
    public static bool isDoubleNeuroEarningsActive = false; // Флаг x2 NeuroCoins

    [SerializeField] private GameObject adsGameObject; // 🔹 Объект, который нужно выключить при покупке

    private void OnEnable()
    {
        YandexGame.PurchaseSuccessEvent += AddCurrency;
    }

    private void OnDisable()
    {
        YandexGame.PurchaseSuccessEvent -= AddCurrency;
    }

    private void AddCurrency(string key)
    {
        if (BrainCurrency.Instance == null || NeuroCurrency.Instance == null)
        {
            Debug.LogError("BrainCurrency or NeuroCurrency instance is not found!");
            return;
        }

        switch (key)
        {
            case "1":
                if (adsGameObject != null)
                {
                    adsGameObject.SetActive(false); // 🔹 Выключаем объект
                    PlayerPrefs.SetInt("AdsDisabledObject", 1);
                    PlayerPrefs.Save();
                    Debug.Log("Объект рекламы отключен!");
                }
                break;
            case "2":
                isDoubleEarningsActive = true; // Включаем x2 BrainCoins
                PlayerPrefs.SetInt("DoubleEarnings", 1);
                PlayerPrefs.Save();
                break;
            case "3":
                isDoubleNeuroEarningsActive = true; // Включаем x2 NeuroCoins
                PlayerPrefs.SetInt("DoubleNeuroEarnings", 1);
                PlayerPrefs.Save();
                break;
            default:
                Debug.LogWarning("Unknown purchase key: " + key);
                break;
        }
    }

    private void Start()
    {
        isDoubleEarningsActive = PlayerPrefs.GetInt("DoubleEarnings", 0) == 1;
        isDoubleNeuroEarningsActive = PlayerPrefs.GetInt("DoubleNeuroEarnings", 0) == 1;

        // 🔹 Проверяем, был ли объект уже отключен
        if (PlayerPrefs.GetInt("AdsDisabledObject", 0) == 1 && adsGameObject != null)
        {
            adsGameObject.SetActive(false);
            Debug.Log("Объект рекламы восстановлен в отключенном состоянии.");
        }
    }
}
