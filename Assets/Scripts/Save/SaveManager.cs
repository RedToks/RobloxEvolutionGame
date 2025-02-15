using UnityEngine;

public static class SaveManager
{
    private static readonly string savePath = Application.persistentDataPath + "/saveData.json";

    public static void SaveGame(GameData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            using (var writer = System.IO.File.CreateText(savePath))
            {
                writer.Write(json);
                writer.Flush(); // Принудительное сохранение данных на диск
            }
            Debug.Log($"Данные сохранены: {savePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка при сохранении данных: {ex.Message}");
        }
    }

    public static GameData LoadGame()
    {
        try
        {
            if (System.IO.File.Exists(savePath))
            {
                string json = System.IO.File.ReadAllText(savePath);
                GameData data = JsonUtility.FromJson<GameData>(json);

                if (data == null || data.inventoryItems == null || data.collectedItems == null)
                {
                    Debug.LogError("Загруженные данные повреждены. Создаем новый объект данных.");
                    return new GameData();
                }

                return data;
            }
            else
            {
                Debug.LogWarning("Файл сохранения не найден. Создаем новый.");
                return new GameData();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка при загрузке данных: {ex.Message}");
            return new GameData();
        }
    }
}
