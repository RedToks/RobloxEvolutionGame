using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoppraGames
{
    public class QuestManager : MonoBehaviour
    {
        private float timeInGame = 0f;
        private int lastCheckedDay = -1;

        public NotificationIcon notificationIcon; // 🔹 Уведомление о доступной награде

        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            UpdateQuestNotification();
        }

        private void Update()
        {
            timeInGame += Time.deltaTime;

            if (timeInGame >= 1f)
            {
                OnAchieveQuestGoal(QuestGoals.STAY_15_MINUTES);
                timeInGame = 0f;
            }
        }

        public enum QuestGoals
        {
            CLICK_1000_TIMES,
            OBTAIN_10_PETS,
            STAY_15_MINUTES,
            COLLECT_DAILY_REWARDS
        }

        [System.Serializable]
        public class Quest
        {
            public int index;
            public QuestGoals goal;
            public int maxValue;
            public string description;
            public RewardItem rewards;
        }

        [System.Serializable]
        public class RewardItem
        {
            public enum Type
            {
                BrainCoins,
                CoinCoins,
                Pets
            }

            public Type type;
            public Sprite icon;
            public int count { get; private set; }

            public void CalculateReward()
            {
                switch (type)
                {
                    case Type.BrainCoins:
                        count = Mathf.FloorToInt(BrainCurrency.Instance.brainCurrency * 0.1f);
                        break;
                    case Type.CoinCoins:
                        count = Mathf.FloorToInt(NeuroCurrency.Instance.coinCurrency * 0.1f);
                        break;
                    case Type.Pets:
                        count = 1;
                        break;
                }
            }
        }

        public static QuestManager instance;
        public Quest[] quests;

        public int GetQuestValue(int index)
        {
            return PlayerPrefs.GetInt("quest_value_" + index, 0);
        }

        public void SetQuestValue(int index, int value)
        {
            PlayerPrefs.SetInt("quest_value_" + index, value);
        }

        public bool IsQuestClaimed(int index)
        {
            return PlayerPrefs.GetInt("quest_claimed_" + index, 0) == 1;
        }

        public void ClaimQuest(int index, bool isTrue)
        {
            PlayerPrefs.SetInt("quest_claimed_" + index, isTrue ? 1 : 0);
        }

        public void OnAchieveQuestGoal(QuestGoals goal)
        {
            foreach (Quest quest in quests)
            {
                if (quest.goal == goal)
                {
                    int currentVal = GetQuestValue(quest.index);
                    currentVal++;
                    currentVal = Mathf.Clamp(currentVal, 0, quest.maxValue);
                    SetQuestValue(quest.index, currentVal);

                    // Проверяем, активен ли QuestWindow перед обновлением
                    if (QuestWindow.instance != null && QuestWindow.instance.gameObject.activeInHierarchy)
                    {
                        QuestWindow.instance.Refresh();
                    }
                }
            }

            // 🔹 Проверяем, есть ли доступные награды
            UpdateQuestNotification();
        }

        public void ResetAllDailyQuests()
        {
            foreach (Quest quest in quests)
            {
                SetQuestValue(quest.index, 0);
                ClaimQuest(quest.index, false);
                quest.rewards.CalculateReward();
            }

            // 🔹 Проверяем уведомление после сброса
            UpdateQuestNotification();
        }

        // 🔹 Проверка наличия доступных наград
        private void UpdateQuestNotification()
        {
            bool hasUnclaimedReward = false;

            foreach (Quest quest in quests)
            {
                int currentVal = GetQuestValue(quest.index);
                if (currentVal >= quest.maxValue && !IsQuestClaimed(quest.index))
                {
                    hasUnclaimedReward = true;
                    break;
                }
            }

            notificationIcon.SetNotification(hasUnclaimedReward);
        }
    }
}
