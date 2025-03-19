using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using YG;

namespace CoppraGames
{
    public class QuestItem : UIBehaviour
    {
        #region Variables
        [Header("Component Refs")]
        public Image BG;
        public GameObject ClaimedLabel;
        public TextMeshProUGUI DescriptionText;
        public TextMeshProUGUI ProgressText;
        public ProgressBar ProgressBar;
        public Image RewardIcon;
        public TextMeshProUGUI RewardCountText;
        public GameObject ClaimButton;
        public Color ClaimedColor;

        private QuestManager.Quest _quest;

        // Словарь для переводов описаний квестов
        private Dictionary<QuestManager.QuestGoals, (string ru, string en)> questDescriptions = new Dictionary<QuestManager.QuestGoals, (string ru, string en)>()
{
    { QuestManager.QuestGoals.CLICK_1000_TIMES, ("Нажмите 1000 раз", "Click 1000 times") },
    { QuestManager.QuestGoals.OBTAIN_10_PETS, ("Получите 10 питомцев", "Obtain 10 pets") },
    { QuestManager.QuestGoals.STAY_15_MINUTES, ("Оставайтесь в игре 15 минут", "Stay in the game for 15 minutes") },
    { QuestManager.QuestGoals.COLLECT_DAILY_REWARDS, ("Соберите ежедневные награды", "Collect daily rewards") }
};
        #endregion

        #region Builtin Methods
        void Start()
        {
        }

        void Update()
        {
        }
        #endregion

        #region Custom Methods
        public override void SetData(UIBehaviourOptions data)
        {
            int questIndex = data.index;
            _quest = QuestManager.instance.quests[questIndex];
            _Refresh();
        }

        public void OnClickClaimButton()
        {
            GetComponentInParent<QuestWindow>().ClaimQuest(this);
        }

        private void _Refresh()
        {
            if (_quest != null)
            {
                string lang = YG2.lang; // Определяем текущий язык

                if (DescriptionText)
                {
                    if (questDescriptions.TryGetValue(_quest.goal, out var translation))
                    {
                        DescriptionText.text = lang == "ru" ? translation.ru : translation.en;
                    }
                    else
                    {
                        DescriptionText.text = _quest.description; // Если перевода нет, оставляем оригинал
                    }
            }

            _quest.rewards.CalculateReward();

            if (RewardIcon && RewardCountText)
            {
                RewardIcon.sprite = _quest.rewards.icon;
                RewardCountText.text = CurrencyFormatter.FormatCurrency(_quest.rewards.count).ToString();
            }

            int progress = QuestManager.instance.GetQuestValue(_quest.index);
            int goalValue = _quest.maxValue;

            if (ProgressText && ProgressBar)
            {
                int percentage = (int)(((float)progress / (float)goalValue) * 100);
                ProgressText.text = $"{progress}/{goalValue}";
                ProgressBar.SetProgress(percentage);
            }

            if (ClaimButton)
            {
                ClaimButton.SetActive(progress >= goalValue);
            }

            bool isClaimed = QuestManager.instance.IsQuestClaimed(_quest.index);
            ProgressBar.gameObject.SetActive(!isClaimed);
            ClaimedLabel.SetActive(isClaimed);
            BG.color = isClaimed ? ClaimedColor : Color.white;

            ClaimButton.SetActive(ClaimButton.activeSelf && !isClaimed);

            if (ClaimedLabel && ClaimedLabel.TryGetComponent(out TextMeshProUGUI claimedText))
            {
                claimedText.text = lang == "ru" ? "Получено!" : "Claimed!";
            }

            if (ClaimButton && ClaimButton.TryGetComponent(out TextMeshProUGUI claimButtonText))
            {
                claimButtonText.text = lang == "ru" ? "Получить" : "Claim";
            }
        }
    }

    public QuestManager.Quest GetQuest()
    {
        return _quest;
    }
    #endregion
}
}