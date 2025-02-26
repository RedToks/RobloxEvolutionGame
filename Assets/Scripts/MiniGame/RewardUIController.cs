using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class RewardUIController : MonoBehaviour
{
    public TextMeshProUGUI rewardText; // ����� � ����������� �������
    public Image rewardIcon; // ������ �������

    public void ShowReward(int amount, Sprite icon)
    {
        gameObject.SetActive(true); // �������� UI
        rewardText.text = "+" + CurrencyFormatter.FormatCurrency(amount).ToString(); // ������������� ���������� �������
        rewardIcon.sprite = icon; // ������������� ������

        // �������� ���������
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        // ��������� ������ �� �������
        StartCoroutine(HideRewardAfterDelay(2f));
    }

    private IEnumerator HideRewardAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // �������� ������������
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
