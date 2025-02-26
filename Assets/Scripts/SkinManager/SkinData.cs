using UnityEngine;

[System.Serializable]
public class SkinData
{
    public string name;
    public Sprite textureSprite;
    public Sprite icon;

    public enum CurrencyType { BrainCoin, CoinCoin }
    public CurrencyType priceType;

    public int brainCoinPrice;
    public int coinCoinPrice;

    public bool isPurchased;
}
