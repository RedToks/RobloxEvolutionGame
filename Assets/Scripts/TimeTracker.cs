using UnityEngine;
using System.Collections;
using YG;

public class TimeTracker : MonoBehaviour
{
    private float gameStartTime;
    private float totalPlayTime;

    void Start()
    {
        // Загружаем ранее сохраненное время
        totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0);
        gameStartTime = Time.time;
    }

    void OnApplicationQuit()
    {
        float sessionTime = Time.time - gameStartTime;
        totalPlayTime += sessionTime;
        PlayerPrefs.SetFloat("TotalPlayTime", totalPlayTime);
        PlayerPrefs.Save();

        // Сохранение времени в Yandex Leaderboard (после выхода)
        SubmitPlayTimeToLeaderboard(totalPlayTime);
    }

    private void SubmitPlayTimeToLeaderboard(float playTime)
    {
        // Преобразуем время в секунды в long
        long playTimeInSeconds = Mathf.FloorToInt(playTime);

        // Отправка результата в Yandex Leaderboard
        YandexGame.NewLeaderboardScores("PlayedTime", playTimeInSeconds);
    }
}
