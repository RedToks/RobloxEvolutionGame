using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;

public class TimeRewardButton : MonoBehaviour
{
    public Image rewardIcon;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI timerText;
    public Button claimButton;

    private TimedRewardsManager.Reward rewardData;
    private TimedRewardsManager manager;

    public TimedRewardsManager.Reward RewardData => rewardData;

    public void Setup(TimedRewardsManager.Reward reward, TimedRewardsManager manager)
    {
        this.rewardData = reward;
        this.manager = manager;

        rewardIcon.sprite = reward.icon;
        rewardText.text = CurrencyFormatter.FormatCurrency(reward.count).ToString();
        claimButton.onClick.AddListener(() => manager.ClaimReward(reward));

        UpdateState(manager.playTime);
    }

    public void UpdateState(float playTime)
    {
        float timeLeft = rewardData.requiredTime - playTime;
        string lang = YG2.lang; // Определяем текущий язык

        if (rewardData.isClaimed)
        {
            claimButton.interactable = false;
            timerText.text = lang == "ru" ? "Получено!" : "Claimed!";
            return;
        }

        if (timeLeft <= 0)
        {
            claimButton.interactable = true;
            timerText.text = lang == "ru" ? "Доступно!" : "Available!";
        }
        else
        {
            claimButton.interactable = false;
            int minutes = Mathf.FloorToInt(timeLeft / 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);
            timerText.text = lang == "ru" ? $"{minutes:D2}:{seconds:D2}" : $"{minutes:D2}:{seconds:D2}";
        }
    }
}

