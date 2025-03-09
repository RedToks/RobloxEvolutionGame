using UnityEngine;
using TMPro;

public class BrainCurrency : MonoBehaviour
{
    public static BrainCurrency Instance;
    public long brainCurrency { get; private set; }
    public TextMeshProUGUI brainCurrencyText;

    public event System.Action OnCurrencyChanged;

    private const string BrainCurrencyKey = "BrainCurrency"; // Ключ для сохранения

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCurrency(); // Загружаем сохранённые данные
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddBrainCurrency(int amount) => AddBrainCurrency((long)amount);
    public void SpendBrainCurrency(int amount) => SpendBrainCurrency((long)amount);

    public void AddBrainCurrency(long amount)
    {
        // Проверяем, активирован ли x2 доход
        if (BuyController.isDoubleEarningsActive)
        {
            amount *= 2; // Удваиваем доход
        }

        brainCurrency += amount;

        // Обновляем общий заработок BrainCoins
        long totalEarnedBrainCoins = PlayerPrefs.GetInt("total_brain_coins", 0);
        totalEarnedBrainCoins += amount;
        PlayerPrefs.SetInt("total_brain_coins", (int)totalEarnedBrainCoins);
        PlayerPrefs.Save();

        SaveCurrency();
        UpdateUI();
        OnCurrencyChanged?.Invoke();
    }

    public void SpendBrainCurrency(long amount)
    {
        if (brainCurrency >= amount)
        {
            brainCurrency -= amount;
            SaveCurrency(); // Сохраняем после изменения
            UpdateUI();
            OnCurrencyChanged?.Invoke();
        }
        else
        {
            Debug.Log("Not enough BrainCurrency!");
        }
    }

    private void UpdateUI()
    {
        if (brainCurrencyText != null)
        {
            brainCurrencyText.text = CurrencyFormatter.FormatCurrency(brainCurrency);
        }
    }

    private void SaveCurrency()
    {
        PlayerPrefs.SetString(BrainCurrencyKey, brainCurrency.ToString());
        PlayerPrefs.Save();
    }

    private void LoadCurrency()
    {
        if (PlayerPrefs.HasKey(BrainCurrencyKey))
        {
            if (long.TryParse(PlayerPrefs.GetString(BrainCurrencyKey), out long savedCurrency))
            {
                brainCurrency = savedCurrency;
            }
        }
        UpdateUI();
    }
}
