using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;

namespace CoppraGames
{
    public class DailyRewardsWindow : MonoBehaviour
    {
        public enum RewardType { BrainCoin, CoinCoin, Pet }

        [Serializable]
        public class RewardData
        {
            public Sprite icon;
            public int count;
            public int day;
            public RewardType rewardType;
        }

        public GameObject ResultPanel;
        public Image ResultIcon;
        public TextMeshProUGUI ResultCount;
        public Button ClaimButton;
        public RewardData[] rewards;
        public DailyRewardItem[] rewardItemComponents;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI timerText2;
        public NotificationIcon notificationIcon;
        public GameObject specialPetPrefab;

        public static DailyRewardsWindow instance;

        private PetPanelUI petPanelUI;
        private DateTime nextClaimTime;
        private int currentDay;
        private const int resetDay = 7;
        private const int rewardCooldownHours = 24;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            petPanelUI = FindObjectOfType<PetPanelUI>();
            LoadData();
            CheckForNextDay();
            UpdateRewards();
            StartCoroutine(UpdateTimerCoroutine());
        }

        private IEnumerator UpdateTimerCoroutine()
        {
            while (true)
            {
                UpdateTimerUI();
                yield return new WaitForSeconds(1);
            }
        }

        private void UpdateTimerUI()
        {
            TimeSpan timeLeft = nextClaimTime - DateTime.UtcNow;

            if (timeLeft.TotalSeconds > 0)
            {
                string timeString = string.Format("{0:D2}:{1:D2}:{2:D2}", timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
                timerText.text = timeString;
                timerText2.text = timeString;
            }
            else
            {
                NextDay();
            }
        }

        private void NextDay()
        {
            currentDay++;
            nextClaimTime = DateTime.UtcNow.AddHours(rewardCooldownHours);

            if (currentDay > resetDay)
            {
                currentDay = 1;
                ResetAllRewards();
            }

            YG2.saves.nextClaimTime = nextClaimTime.ToString();
            YG2.saves.currentDay = currentDay;
            YG2.SaveProgress();

            UpdateRewards();
        }

        private void LoadData()
        {
            if (string.IsNullOrEmpty(YG2.saves.nextClaimTime))
            {
                nextClaimTime = DateTime.UtcNow.AddHours(rewardCooldownHours);
            }
            else
            {
                nextClaimTime = DateTime.Parse(YG2.saves.nextClaimTime);
            }

            currentDay = YG2.saves.currentDay;

            Debug.Log($"Загруженный день: {currentDay}"); // Проверка в консоли

            for (int i = 1; i <= resetDay; i++)
            {
                bool claimed = YG2.saves.rewardClaimed.ContainsKey(i) && YG2.saves.rewardClaimed[i];
                Debug.Log($"День {i}: {(claimed ? "забран" : "не забран")}"); // Проверка сохраненных данных
            }
        }

        private void CheckForNextDay()
        {
            while (DateTime.UtcNow >= nextClaimTime)
            {
                currentDay++;
                nextClaimTime = nextClaimTime.AddHours(rewardCooldownHours);
            }

            if (currentDay > resetDay)
            {
                currentDay = 1;
                ResetAllRewards();
            }

            YG2.saves.nextClaimTime = nextClaimTime.ToString();
            YG2.saves.currentDay = currentDay;
            YG2.SaveProgress();
            UpdateRewards();
        }

        private void ResetAllRewards()
        {
            YG2.saves.rewardClaimed.Clear();
            for (int i = 1; i <= resetDay; i++)
            {
                YG2.saves.rewardClaimed[i] = false;
            }
            YG2.SaveProgress();
        }

        private void UpdateRewards()
        {
            bool hasUnclaimedReward = false;

            for (int i = 0; i < rewards.Length; i++)
            {
                bool isCurrentDay = rewards[i].day == currentDay;
                bool isClaimed = IsDailyRewardClaimed(rewards[i].day);

                rewardItemComponents[i].SetData(rewards[i], isCurrentDay);

                if (isCurrentDay && !isClaimed)
                {
                    hasUnclaimedReward = true;
                }
            }

            notificationIcon.SetNotification(hasUnclaimedReward);
            RefreshClaimButton();
        }

        public void OnClickClaimButton()
        {
            if (DailyRewardItem.selectedItem != null)
            {
                int day = DailyRewardItem.selectedItem.GetDay();
                if (!IsDailyRewardClaimed(day))
                {
                    ClaimDailyReward(day);
                    ShowResult(day - 1);
                    UpdateRewards();
                }
            }
        }

        private void ClaimDailyReward(int day)
        {
            RewardData reward = rewards[day - 1];

            switch (reward.rewardType)
            {
                case RewardType.BrainCoin:
                    BrainCurrency.Instance.AddBrainCurrency(reward.count);
                    break;
                case RewardType.CoinCoin:
                    NeuroCurrency.Instance.AddCoinCurrency(reward.count);
                    break;
                case RewardType.Pet:
                    GivePlayerPet(reward);
                    break;
            }

            // Добавляем в словарь вручную, если ключа нет
            if (!YG2.saves.rewardClaimed.ContainsKey(day))
            {
                YG2.saves.rewardClaimed.Add(day, true);
            }
            else
            {
                YG2.saves.rewardClaimed[day] = true;
            }

            Debug.Log($"✅ День {day} успешно сохранен как полученный!");

            YG2.SaveProgress();
        }


        private void GivePlayerPet(RewardData reward)
        {
            Pet newPet = new Pet(reward.icon, specialPetPrefab, 500f, Pet.PetRarity.Special);
            petPanelUI.AddPet(newPet);
        }

        public bool IsDailyRewardClaimed(int day)
        {
            return YG2.saves.rewardClaimed.ContainsKey(day) && YG2.saves.rewardClaimed[day];
        }

        private void RefreshClaimButton()
        {
            if (DailyRewardItem.selectedItem != null)
            {
                int day = DailyRewardItem.selectedItem.GetDay();
                ClaimButton.interactable = !IsDailyRewardClaimed(day);
            }
            else
            {
                ClaimButton.interactable = false;
            }
        }

        private void ShowResult(int resultIndex)
        {
            if (ResultPanel)
            {
                ResultPanel.SetActive(true);
                ResultIcon.sprite = rewards[resultIndex].icon;
                ResultCount.text = "x" + rewards[resultIndex].count.ToString();
                ResultPanel.GetComponent<Animator>().Play("clip");
            }
            Invoke("HideResult", 3.3f);
        }

        public bool IsDailyRewardReadyToCollect(int day)
        {
            return currentDay >= day;
        }

        private void HideResult()
        {
            if (ResultPanel)
            {
                ResultPanel.SetActive(false);
            }
        }
    }
}
