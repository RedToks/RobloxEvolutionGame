using UnityEngine;
using System.Collections;
using YG;

public class TimeTracker : MonoBehaviour
{
    private float gameStartTime;
    private float totalPlayTime;

    void Start()
    {
        // ��������� ����� ����������� �����
        totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0);
        gameStartTime = Time.time;
    }

    void OnApplicationQuit()
    {
        float sessionTime = Time.time - gameStartTime;
        totalPlayTime += sessionTime;
        PlayerPrefs.SetFloat("TotalPlayTime", totalPlayTime);
        PlayerPrefs.Save();

        // ���������� ������� � Yandex Leaderboard (����� ������)
        SubmitPlayTimeToLeaderboard(totalPlayTime);
    }

    private void SubmitPlayTimeToLeaderboard(float playTime)
    {
        // ����������� ����� � ������� � long
        long playTimeInSeconds = Mathf.FloorToInt(playTime);

        // �������� ���������� � Yandex Leaderboard
        YandexGame.NewLeaderboardScores("PlayedTime", playTimeInSeconds);
    }
}
