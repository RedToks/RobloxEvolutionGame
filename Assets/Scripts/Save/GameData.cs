using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public List<string> inventoryItems = new List<string>();
    public List<string> collectedItems = new List<string>();
    public List<string> completedQuestIcons = new List<string>(); // Список имен завершенных квестов
}
