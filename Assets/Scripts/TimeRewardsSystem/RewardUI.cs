using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardUI : MonoBehaviour
{
    public Image rewardIcon;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI timerText;
    public Button claimButton;

    private TimedRewardsManager.Reward rewardData;
    private TimedRewardsManager manager;

    public void Setup(TimedRewardsManager.Reward reward, TimedRewardsManager manager)
    {
        this.rewardData = reward;
        this.manager = manager;

        rewardIcon.sprite = reward.icon;
        rewardText.text = reward.rewardText;
        claimButton.onClick.AddListener(() => manager.ClaimReward(reward));

        UpdateState(manager.playTime);
    }

    public void UpdateState(float playTime)
    {
        float timeLeft = rewardData.requiredTime - playTime;

        if (rewardData.isClaimed)
        {
            claimButton.interactable = false;
            return;
        }

        if (timeLeft <= 0)
        {
            claimButton.interactable = true;
            timerText.text = "0:00";
        }
        else
        {
            claimButton.interactable = false;
            int minutes = Mathf.FloorToInt(timeLeft / 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);
            timerText.text = $"{minutes:D2}:{seconds:D2}";
        }
    }
}
