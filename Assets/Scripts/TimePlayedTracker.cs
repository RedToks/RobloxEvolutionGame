using UnityEngine;
using YG;
using System;

public class TimePlayedTracker : MonoBehaviour
{
    private int saveCooldown = 20; // 🔹 Частота сохранения (раз в 20 секунд)
    private int timeSinceLastSave = 0; // 🔹 Время с последнего сохранения

    private void Start()
    {
        Time.timeScale = 1f;
        if (YG2.saves != null)
        {
            YG2.saves.playedTime = Math.Max(YG2.saves.playedTime, 0);
        }
        else
        {
            Debug.LogError("YG2.saves не инициализирован!");
        }

        // Запускаем обновление каждую секунду
        InvokeRepeating(nameof(UpdateTimePlayed), 1f, 1f);
    }

    private void UpdateTimePlayed()
    {
        YG2.saves.playedTime += 1; // Увеличиваем на 1 секунду
        timeSinceLastSave += 1; // 🔹 Отслеживаем время с последнего сохранения

        if (timeSinceLastSave >= saveCooldown) // 🔹 Проверяем, прошло ли 20 секунд
        {
            SaveTimePlayed();
            timeSinceLastSave = 0; // Сбрасываем таймер
        }
    }

    private void SaveTimePlayed()
    {
        YG2.SaveProgress();
        YG2.SetLeaderboard("score", YG2.saves.playedTime);
    }
}
