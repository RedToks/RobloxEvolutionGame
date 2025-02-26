using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class RewardUIController : MonoBehaviour
{
    public TextMeshProUGUI rewardText; // Текст с количеством награды
    public Image rewardIcon; // Иконка награды

    public void ShowReward(int amount, Sprite icon)
    {
        gameObject.SetActive(true); // Включаем UI
        rewardText.text = "+" + CurrencyFormatter.FormatCurrency(amount).ToString(); // Устанавливаем количество награды
        rewardIcon.sprite = icon; // Устанавливаем иконку

        // Анимация появления
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        // Запускаем таймер на скрытие
        StartCoroutine(HideRewardAfterDelay(2f));
    }

    private IEnumerator HideRewardAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Анимация исчезновения
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
