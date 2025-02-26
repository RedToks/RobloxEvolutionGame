using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace CoppraGames
{
    public class DailyRewardsWindow : MonoBehaviour
    {
        public enum RewardType
        {
            BrainCoin,
            CoinCoin,
            Pet
        }
        [System.Serializable]
        public class RewardData
        {
            public Sprite icon;
            public int count;
            public int day;
            public RewardType rewardType; // 🔥 Добавляем тип награды
        }

        public GameObject ResultPanel;
        public Image ResultIcon;
        public TextMeshProUGUI ResultCount;
        public Button ClaimButton;
        public RewardData[] rewards;
        public DailyRewardItem[] rewardItemComponents;
        public TextMeshProUGUI timerText;
        public NotificationIcon notificationIcon; // 🔹 Уведомление (восклицательный знак)

        public GameObject specialPetPrefab;

        public static DailyRewardsWindow instance;

        private PetPanelUI petPanelUI;
        private DateTime lastUpdateTime;
        private int currentDay;
        private const int resetDay = 7; // День, после которого сбрасывается прогресс
        private const int updateInterval = 10; // Интервал обновления в секундах

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            petPanelUI = FindObjectOfType<PetPanelUI>();
            LoadData();
            UpdateRewards();
            StartCoroutine(UpdateTimerCoroutine());
        }

        private void LoadData()
        {
            lastUpdateTime = DateTime.Parse(PlayerPrefs.GetString("last_update_time", DateTime.Now.ToString()));
            currentDay = PlayerPrefs.GetInt("current_day", 1);
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
            DateTime nextUpdateTime = lastUpdateTime.AddSeconds(updateInterval);
            TimeSpan timeLeft = nextUpdateTime - DateTime.Now;

            if (timeLeft.TotalSeconds > 0)
            {
                timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeLeft.Minutes, timeLeft.Seconds, timeLeft.Milliseconds / 10);
            }
            else
            {
                NextDay();
            }
        }

        private void NextDay()
        {
            lastUpdateTime = DateTime.Now;
            currentDay++;
            QuestManager.instance.ResetAllDailyQuests();

            if (currentDay > resetDay)
            {
                currentDay = 1; // Сбросить на день 1
                ResetAllRewards(); // Сброс всех наград
            }

            PlayerPrefs.SetString("last_update_time", lastUpdateTime.ToString());
            PlayerPrefs.SetInt("current_day", currentDay);
            PlayerPrefs.Save();

            UpdateRewards();
        }

        private void ResetAllRewards()
        {
            for (int i = 1; i <= resetDay; i++)
            {
                PlayerPrefs.DeleteKey("reward_claimed_" + i);
            }
            PlayerPrefs.Save();
        }

        private void UpdateRewards()
        {
            bool hasUnclaimedReward = false;

            for (int i = 0; i < rewards.Length; i++)
            {
                bool isCurrentDay = rewards[i].day == currentDay;
                bool isClaimed = IsDailyRewardClaimed(rewards[i].day);

                rewardItemComponents[i].SetData(rewards[i], isCurrentDay);

                // 🔹 Проверяем, есть ли хотя бы одна незабранная награда
                if (isCurrentDay && !isClaimed)
                {
                    hasUnclaimedReward = true;
                }
            }

            // 🔹 Включаем/выключаем уведомление
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
            RewardData reward = rewards[day - 1]; // Получаем данные о награде

            switch (reward.rewardType)
            {
                case RewardType.BrainCoin:
                    BrainCurrency.Instance.AddBrainCurrency(reward.count);
                    break;

                case RewardType.CoinCoin:
                    NeuroCurrency.Instance.AddCoinCurrency(reward.count);
                    break;

                case RewardType.Pet:
                    GivePlayerPet(reward); // Выдаем питомца
                    break;
            }

            PlayerPrefs.SetInt("reward_claimed_" + day, 1);
            PlayerPrefs.Save();
            QuestManager.instance.OnAchieveQuestGoal(QuestManager.QuestGoals.COLLECT_DAILY_REWARDS);
        }

        private void GivePlayerPet(RewardData reward)
        {
            Pet newPet = new Pet("Special Pet", reward.icon, specialPetPrefab, 500f, Pet.PetRarity.Special);
            petPanelUI.AddPet(newPet);
        }

        public bool IsDailyRewardClaimed(int day)
        {
            return PlayerPrefs.GetInt("reward_claimed_" + day, 0) == 1;
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
