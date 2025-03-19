using UnityEngine;
using YG;
using YG.Utils.Pay;

public class BuyController : MonoBehaviour
{
    public static bool isDoubleEarningsActive = false; // Флаг x2 BrainCoins
    public static bool isDoubleNeuroEarningsActive = false; // Флаг x2 NeuroCoins

    [SerializeField] private PetPanelUI petPanelUI; // Панель питомцев
    [SerializeField] private Pet specialPetPrefab; // Префаб специального питомца

    [SerializeField] private GameObject objectToDisable1;
    [SerializeField] private GameObject objectToDisable2;
    [SerializeField] private GameObject objectToDisableForDoubleEarnings;
    [SerializeField] private GameObject objectToDisableForDoubleNeuroEarnings;

    private void OnEnable()
    {
        YG2.onPurchaseSuccess += AddCurrency;
    }

    private void OnDisable()
    {
        YG2.onPurchaseSuccess -= AddCurrency;
    }

    private void Start()
    {
        // Загружаем сохраненные состояния
        isDoubleEarningsActive = YG2.saves.isDoubleEarningsActive;
        isDoubleNeuroEarningsActive = YG2.saves.isDoubleNeuroEarningsActive;

        if (YG2.saves.isObjectsDisabled)
        {
            DisableObjects(objectToDisable1, objectToDisable2);
        }
        if (YG2.saves.isDoubleEarningsDisabled)
        {
            DisableObjects(objectToDisableForDoubleEarnings);
        }
        if (YG2.saves.isDoubleNeuroEarningsDisabled)
        {
            DisableObjects(objectToDisableForDoubleNeuroEarnings);
        }
    }

    private void AddCurrency(string key)
    {
        switch (key)
        {
            case "1":
                YG2.saves.isObjectsDisabled = true;
                DisableObjects(objectToDisable1, objectToDisable2);
                Debug.Log("Объекты для кейса '1' отключены!");
                break;

            case "2":
                isDoubleEarningsActive = true;
                YG2.saves.isDoubleEarningsActive = true;
                YG2.saves.isDoubleEarningsDisabled = true;
                DisableObjects(objectToDisableForDoubleEarnings);
                ClickMultiplier.Instance.SetOtherMultiplier(2f);
                Debug.Log("x2 BrainCoins активированы!");
                break;

            case "3":
                isDoubleNeuroEarningsActive = true;
                YG2.saves.isDoubleNeuroEarningsActive = true;
                YG2.saves.isDoubleNeuroEarningsDisabled = true;
                DisableObjects(objectToDisableForDoubleNeuroEarnings);
                Debug.Log("x2 NeuroCoins активированы!");
                break;

            case "4":
                if (petPanelUI != null && specialPetPrefab != null)
                {
                    Pet specialPet = new Pet(specialPetPrefab.Icon, specialPetPrefab.Prefab, 1500f, Pet.PetRarity.Special);
                    petPanelUI.AddPet(specialPet);
                    Debug.Log("Добавлен специальный питомец!");
                }
                else
                {
                    Debug.LogError("Ошибка: petPanelUI или specialPetPrefab не назначены!");
                }
                break;

            case "5":
                BrainCurrency.Instance.AddBrainCurrency(100000);
                Debug.Log("Начислено 100K BrainCoins!");
                break;

            case "6":
                BrainCurrency.Instance.AddBrainCurrency(1000000);
                Debug.Log("Начислено 1M BrainCoins!");
                break;

            case "7":
                BrainCurrency.Instance.AddBrainCurrency(1000000000);
                Debug.Log("Начислено 1B BrainCoins!");
                Debug.Log($"[SaveCurrency] Перед сохранением: brainCurrency = {BrainCurrency.Instance.brainCurrency}");
                break;

            case "8":
                NeuroCurrency.Instance.AddCoinCurrency(10000);
                Debug.Log("Начислено 10K NeuroCoins!");
                break;

            case "9":
                NeuroCurrency.Instance.AddCoinCurrency(100000);
                Debug.Log("Начислено 100K NeuroCoins!");
                break;

            case "10":
                NeuroCurrency.Instance.AddCoinCurrency(1000000);
                Debug.Log("Начислено 1M NeuroCoins!");
                break;

            default:
                Debug.LogWarning("Неизвестный ключ покупки: " + key);
                break;
        }
        YG2.SaveProgress();
    }

    private void DisableObjects(params GameObject[] objects)
    {
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}
