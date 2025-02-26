using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CoppraGames
{
    public class DailyRewardItem : MonoBehaviour
    {
        public static DailyRewardItem selectedItem;

        public Image Icon;
        public TextMeshProUGUI CountText;
        public Image DayPanel;
        public GameObject GreenPanel;
        public GameObject Glow;
        public GameObject TickMark;

        [HideInInspector]
        public int day;

        public void SetData(DailyRewardsWindow.RewardData reward, bool isActive)
        {
            this.Icon.sprite = reward.icon;
            this.CountText.text = CurrencyFormatter.FormatCurrency(reward.count);
            this.day = reward.day;

            bool isReadyToCollect = DailyRewardsWindow.instance.IsDailyRewardReadyToCollect(day);
            bool isClaimed = DailyRewardsWindow.instance.IsDailyRewardClaimed(day);

            GreenPanel.SetActive(isActive);
            Glow.SetActive(isActive && isReadyToCollect && !isClaimed);
            DayPanel.color = isActive && !isClaimed ? Color.green : Color.white;
            TickMark.SetActive(isClaimed);

            if (isActive && isReadyToCollect && !isClaimed)
                SetSelected(true);
        }

        public void SetSelected(bool isTrue)
        {
            if (this != selectedItem && selectedItem != null)
                selectedItem.SetSelected(false);

            Glow.SetActive(isTrue);

            if (isTrue)
                selectedItem = this;
            else if (DailyRewardsWindow.instance.IsDailyRewardClaimed(day))
                selectedItem = null;
        }

        public int GetDay()
        {
            return day;
        }
    }
}
