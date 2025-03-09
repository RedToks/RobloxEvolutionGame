using UnityEngine;

[System.Serializable]
public class MultiplierData
{
    public Sprite icon;         // Иконка множителя
    public float multiplier;    // Значение множителя (например, x200)
    [HideInInspector]
    public long unlockCost; // Скрытый long
    [SerializeField]
    private string unlockCostString = "0"; // Отображаемая строка

    public void ParseUnlockCost()
    {
        if (!long.TryParse(unlockCostString, out unlockCost))
        {
            Debug.LogError($"Ошибка: Некорректное число в unlockCostString ({unlockCostString})");
        }
    }
}
