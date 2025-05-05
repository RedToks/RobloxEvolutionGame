using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace CoppraGames
{
    public class QuestManager : MonoBehaviour
    {
        private float timeInGame = 0f;
        private int lastCheckedDay = -1;

        public NotificationIcon notificationIcon; // 🔹 Уведомление о доступной награде
        private PetPanelUI petPanelUI;

        void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            petPanelUI = FindObjectOfType<PetPanelUI>();
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
            public GameObject petPrefab;
            public int count { get; private set; }

            public void CalculateReward()
            {
                int baseReward = 100; // Базовая награда

                switch (type)
                {
                    case Type.BrainCoins:
                        count = baseReward + Mathf.FloorToInt(BrainCurrency.Instance.brainCurrency * 0.1f);
                        break;
                    case Type.CoinCoins:
                        count = baseReward + Mathf.FloorToInt(NeuroCurrency.Instance.coinCurrency * 0.1f);
                        break;
                    case Type.Pets:
                        count = 1; // Для питомцев логика не меняется
                        break;
                }
            }
        }

        public static QuestManager instance;
        public Quest[] quests;

        public int GetQuestValue(int index)
        {
            return YG2.saves.questValues.ContainsKey(index) ? YG2.saves.questValues[index] : 0;
        }

        public void SetQuestValue(int index, int value)
        {
            YG2.saves.questValues[index] = value;
        }

        public bool IsQuestClaimed(int index)
        {
            return YG2.saves.questClaimed.ContainsKey(index) && YG2.saves.questClaimed[index];
        }

        public void ClaimQuest(int index, bool isTrue)
        {
            YG2.saves.questClaimed[index] = isTrue;
            Quest quest = System.Array.Find(quests, q => q.index == index);

            if (quest != null && isTrue)
            {
                GiveReward(quest.rewards);
            }

            YG2.SaveProgress(); // Сохраняем прогресс
        }

        void GiveReward(RewardItem reward)
        {
            switch (reward.type)
            {
                case RewardItem.Type.BrainCoins:
                    BrainCurrency.Instance.AddBrainCurrency(reward.count);
                    Debug.Log($"Игрок получил {reward.count} BrainCoins!");
                    break;

                case RewardItem.Type.CoinCoins:
                    NeuroCurrency.Instance.AddCoinCurrency(reward.count);
                    Debug.Log($"Игрок получил {reward.count} CoinCoins!");
                    break;

                case RewardItem.Type.Pets:
                    GivePet(reward);
                    break;
            }
        }

        void GivePet(RewardItem reward)
        {
            if (reward.petPrefab != null)
            {
                Pet newPet = new Pet(reward.icon, reward.petPrefab, 300f, Pet.PetRarity.Special);
                petPanelUI.AddPet(newPet);
                Debug.Log($"Игрок получил питомца: {reward.petPrefab.name} (Сила: 300, Редкость: Special)");
            }
            else
            {
                Debug.LogWarning("Ошибка: Префаб питомца не задан в награде!");
            }
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
