using UnityEngine;

[System.Serializable]
public class Hair
{
    public enum CurrencyType { BrainCoin, CoinCoin } // Enum валют

    public Sprite icon;
    public int brainCoinPrice;
    public int coinCoinPrice;
    public bool isPurchased;
    public GameObject hairPrefab;
    public CurrencyType priceType; // Выбранный тип валюты
}
