using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimedRewardsManager : MonoBehaviour
{
    [System.Serializable]
    public class Reward
    {
        public RewardType type;
        public string id;
        public float requiredTime; // Время в секундах
        public Sprite icon;
        public int count;
        public bool isClaimed;

        public GameObject petPrefab;
        public string petName;
        public float petStrength;
        public Sprite petIcon;
    }

    public enum RewardType
    {
        Pet,
        BrainCoins,
        CoinCoins
    }

    public List<Reward> rewards = new List<Reward>();
    public Transform rewardPanelParent;
    public GameObject rewardPrefab;
    public NotificationIcon notificationIcon;

    public float playTime;
    private const string LAST_LOGIN_KEY = "LastLoginTime";

    private void Awake()
    {
        LoadPlayTime();
        StartCoroutine(UpdatePlayTime());
        CreateRewardButtons();
    }

    private void LoadPlayTime()
    {
        // Загружаем последнее время выхода
        string lastLoginString = PlayerPrefs.GetString(LAST_LOGIN_KEY, "");
        if (!string.IsNullOrEmpty(lastLoginString))
        {
            DateTime lastLoginTime = DateTime.Parse(lastLoginString);
            TimeSpan timeSinceLastLogin = DateTime.UtcNow - lastLoginTime;

            // Добавляем прошедшее время в playTime
            playTime += (float)timeSinceLastLogin.TotalSeconds;
        }
    }

    private IEnumerator UpdatePlayTime()
    {
        while (true)
        {
            playTime += 1;
            PlayerPrefs.SetString(LAST_LOGIN_KEY, DateTime.UtcNow.ToString());
            PlayerPrefs.Save();
            UpdateRewardButtons();
            yield return new WaitForSeconds(1);
        }
    }

    private void CreateRewardButtons()
    {
        foreach (Reward reward in rewards)
        {
            GameObject rewardObj = Instantiate(rewardPrefab, rewardPanelParent);
            TimeRewardButton rewardUI = rewardObj.GetComponent<TimeRewardButton>();
            rewardUI.Setup(reward, this);
        }
    }

    public void ClaimReward(Reward reward)
    {
        if (playTime >= reward.requiredTime && !reward.isClaimed)
        {
            reward.isClaimed = true;
            PlayerPrefs.SetInt(reward.id, 1);
            PlayerPrefs.Save();

            GiveReward(reward);
            UpdateRewardButtons();
        }
    }

    void GiveReward(Reward reward)
    {
        switch (reward.type)
        {
            case RewardType.Pet:
                GivePet(reward);
                break;
            case RewardType.BrainCoins:
                BrainCurrency.Instance.AddBrainCurrency(reward.count);
                Debug.Log($"Игрок получил {reward.count} BrainCoins!");
                break;
            case RewardType.CoinCoins:
                NeuroCurrency.Instance.AddCoinCurrency(reward.count);
                Debug.Log($"Игрок получил {reward.count} CoinCoins!");
                break;
        }
    }

    void GivePet(Reward reward)
    {
        if (reward.petPrefab != null)
        {
            Pet newPet = new Pet(reward.petName, reward.petIcon, reward.petPrefab, reward.petStrength, Pet.PetRarity.Special);

            FindObjectOfType<PetPanelUI>().AddPet(newPet);
        }
    }


    private void UpdateRewardButtons()
    {
        bool hasAvailableReward = false;

        foreach (Transform child in rewardPanelParent)
        {
            TimeRewardButton rewardUI = child.GetComponent<TimeRewardButton>();
            if (rewardUI != null)
            {
                rewardUI.UpdateState(playTime);
                if (rewardUI.RewardData.requiredTime <= playTime && !rewardUI.RewardData.isClaimed)
                {
                    hasAvailableReward = true;
                }
            }
        }

        if (notificationIcon != null)
        {
            notificationIcon.SetNotification(hasAvailableReward);
        }
    }
}
