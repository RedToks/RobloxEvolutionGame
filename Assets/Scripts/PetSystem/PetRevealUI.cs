using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using static Pet;

public class PetRevealUI : MonoBehaviour
{
    public static PetRevealUI Instance;

    public Image petImage; // �������� �������
    public Image lightIcon; // ���������� ������
    public TextMeshProUGUI rarityText; // ����� ��������
    public TextMeshProUGUI titleText; // �������� �������
    public CanvasGroup canvasGroup; // ��� �������� ������������
    public RectTransform panelTransform; // ��� UI-������
    public float rotationSpeed = 50f; // �������� ��������

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Assign instance if null
        }
        else
        {
            Debug.LogError("Multiple PetRevealUI instances found! Destroying duplicate.");
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    private void Update()
    {
        if (lightIcon.gameObject.activeSelf)
        {
            lightIcon.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }

    public void ShowPet(Sprite petSprite, PetRarity rarity)
    {
        petImage.sprite = petSprite;
        rarityText.text = rarity.ToString();
        rarityText.color = GetRarityColor(rarity);
        titleText.color = GetRarityColor(rarity); // ������������ ���� ��������

        // ������������� ���� ��������
        if (lightIcon != null)
        {
            lightIcon.color = GetRarityColor(rarity);
            lightIcon.gameObject.SetActive(true);
        }

        gameObject.SetActive(true);

        // ��������� ���������
        canvasGroup.alpha = 0;
        panelTransform.localScale = Vector3.zero;

        // ��������� �������� (������� ��������� � ����������)
        canvasGroup.DOFade(1f, 0.5f);
        panelTransform.DOScale(1.2f, 0.5f).SetEase(Ease.OutBack)
            .OnComplete(() => StartCoroutine(HideAfterDelay()));
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        // ������������ (������������ � ����������)
        canvasGroup.DOFade(0f, 0.5f);
        panelTransform.DOScale(0f, 0.5f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                lightIcon.gameObject.SetActive(false); // ��������� ��������
            });
    }

    private Color GetRarityColor(PetRarity rarity)
    {
        switch (rarity)
        {
            case PetRarity.Common:
                return Color.white;
            case PetRarity.Rare:
                return Color.blue;
            case PetRarity.Mythic:
                return Color.red;
            case PetRarity.Special:
                return Color.yellow;
            default:
                return Color.white;
        }
    }
}
