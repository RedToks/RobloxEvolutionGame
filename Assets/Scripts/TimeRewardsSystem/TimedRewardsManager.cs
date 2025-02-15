using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static TimedRewardsManager;

public class TimedRewardsManager : MonoBehaviour
{
    [System.Serializable]
    public class Reward
    {
        public string id; // Уникальный ID подарка
        public float requiredTime; // Время в секундах (например, 300 секунд = 5 минут)
        public Sprite icon; // Иконка награды
        public string rewardText; // Текст награды (например, "x50K")
        public bool isClaimed; // Получен ли подарок
    }

    public List<Reward> rewards = new List<Reward>();
    public Transform rewardPanelParent; // Контейнер для кнопок подарков
    public GameObject rewardPrefab; // Префаб кнопки подарка

    public float playTime; // Общее время в игре
    private const string PLAY_TIME_KEY = "TotalPlayTime";

    private void Awake()
    {
        LoadPlayTime();
        StartCoroutine(UpdatePlayTime());
        CreateRewardButtons();
    }

    private IEnumerator UpdatePlayTime()
    {
        while (true)
        {
            playTime += 1;
            PlayerPrefs.SetFloat(PLAY_TIME_KEY, playTime);
            PlayerPrefs.Save();
            UpdateRewardButtons();
            yield return new WaitForSeconds(1);
        }
    }

    private void LoadPlayTime()
    {
        playTime = PlayerPrefs.GetFloat(PLAY_TIME_KEY, 0);
    }

    private void CreateRewardButtons()
    {
        foreach (Reward reward in rewards)
        {
            GameObject rewardObj = Instantiate(rewardPrefab, rewardPanelParent);
            RewardUI rewardUI = rewardObj.GetComponent<RewardUI>();
            rewardUI.Setup(reward, this);
        }
    }

    public void ClaimReward(Reward reward)
    {
        if (playTime >= reward.requiredTime && !reward.isClaimed)
        {
            reward.isClaimed = true;
            PlayerPrefs.SetInt(reward.id, 1); // Сохраняем, что награда получена
            PlayerPrefs.Save();
            UpdateRewardButtons();
            Debug.Log($"Получена награда: {reward.rewardText}");
        }
    }

    private void UpdateRewardButtons()
    {
        foreach (Transform child in rewardPanelParent)
        {
            RewardUI rewardUI = child.GetComponent<RewardUI>();
            if (rewardUI != null)
            {
                rewardUI.UpdateState(playTime);
            }
        }
    }

    private void OnApplicationQuit()
    {
        // Сбрасываем playTime при выходе из игры
        PlayerPrefs.SetFloat(PLAY_TIME_KEY, 0);
        PlayerPrefs.Save();
    }
}
