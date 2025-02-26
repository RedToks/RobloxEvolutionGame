using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CoppraGames
{
    public class QuestWindow : MonoBehaviour
    {

        public static QuestWindow instance;

        /* prefabs */
        public QuestItem QuestItemPrefab;

        /* component references */
        public DataList DataList;

        public GameObject RewardClaimingPanel;
        public Image RewardClaimingIcon;
        public TextMeshProUGUI RewardClaimingCount;


        /* private variables */

        private void Awake()
        {
            instance = this;
            HideRewardClaiming();     
            
        }

        public void Start()
        {
            Init();
        }

        public void Init()
        {
            this.Refresh();
        }


        public void Refresh()
        {
            _LoadItemsList();
        }


        private void _LoadItemsList()
        {
            if (DataList != null)
            {
                DataList.Clear();
                DataListOptions options = new DataListOptions();
                List<UIBehaviourOptions> list = new List<UIBehaviourOptions>();

                foreach(QuestManager.Quest quest in QuestManager.instance.quests)
                {
                    //if (!QuestManager.instance.IsQuestClaimed(quest.index))
                    //{
                        UIBehaviourOptions option = new UIBehaviourOptions();
                        option.index = quest.index;
                        list.Add(option);
                    //}
                }

                if (QuestItemPrefab != null)
                {
                    options.prefab = QuestItemPrefab;
                }

                options.list = list;
                DataList.SetData(options);
            }

        }

        public void Close()
        {
            Main.instance.ShowQuestWindow(false);
            //Destroy(this.gameObject);
        }

        public void ClaimQuest(QuestItem questItem)
        {
            QuestManager.Quest quest = questItem.GetQuest();
            if(quest != null)
            {
                QuestManager.instance.ClaimQuest(quest.index, true);

                quest.rewards.CalculateReward();
                GiveReward(quest.rewards);

                StartCoroutine(_ShowRewardClaiming(quest.rewards));
                this.Refresh();
            }
            
        }


        private void GiveReward(QuestManager.RewardItem reward)
{
    if (reward == null) return;

    switch (reward.type)
    {
        case QuestManager.RewardItem.Type.BrainCoins:
            BrainCurrency.Instance.AddBrainCurrency(reward.count);
            break;
        case QuestManager.RewardItem.Type.CoinCoins:
            NeuroCurrency.Instance.AddCoinCurrency(reward.count);
            break;
        case QuestManager.RewardItem.Type.Pets:
            Debug.Log("Добавляем питомца!"); // Здесь можешь добавить логику получения питомца
            break;
    }
}

        private IEnumerator _ShowRewardClaiming(QuestManager.RewardItem reward)
        {
            if (RewardClaimingPanel && reward != null)
            {
                RewardClaimingPanel.SetActive(true);

                if (reward != null)
                {
                    RewardClaimingIcon.sprite = reward.icon;
                    RewardClaimingCount.text = "x" + CurrencyFormatter.FormatCurrency(reward.count);
                }

                RewardClaimingPanel.GetComponent<Animator>().Play("clip");
            }
            yield return new WaitForSeconds(3.3f);
            HideRewardClaiming();

            Debug.Log("Reward claimed. You can do your own logic here!");
        }


        public void HideRewardClaiming()
        {
            if (RewardClaimingPanel)
            {
                RewardClaimingPanel.SetActive(false);
            }
        }


    }
}
