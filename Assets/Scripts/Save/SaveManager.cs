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
                writer.Flush(); // �������������� ���������� ������ �� ����
            }
            Debug.Log($"������ ���������: {savePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"������ ��� ���������� ������: {ex.Message}");
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
                    Debug.LogError("����������� ������ ����������. ������� ����� ������ ������.");
                    return new GameData();
                }

                return data;
            }
            else
            {
                Debug.LogWarning("���� ���������� �� ������. ������� �����.");
                return new GameData();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"������ ��� �������� ������: {ex.Message}");
            return new GameData();
        }
    }
}
