using UnityEngine;
using TMPro;
using System.Collections;
using System;
using KinematicCharacterController.Examples;

public class TriggerBrainBonus : MonoBehaviour
{
    [Header("��������� �������")]
    public TextMeshPro timerText; // ����� ��� ����������� �������
    public float cooldownTime = 600f; // 10 ����� (600 ������)

    [Header("��������� ������")]
    [Range(0f, 1f)] public float bonusMultiplier = 0.1f; // ��������� ������ (�� ��������� 10%)

    [Header("����� �������")]
    public Color activeColor = Color.red;  // ���� �� ����� �������
    public Color readyColor = Color.green; // ����, ����� ����� ��������

    private bool canReceiveBonus = true;
    private float timerRemaining = 0f;
    private const string LastExitTimeKey = "LastExitTime";
    private const string TimerRemainingKey = "TimerRemaining";

    private void Start()
    {
        LoadTimerState();
        StartCoroutine(StartCooldown());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ExampleCharacterController player) && canReceiveBonus)
        {
            GiveBrainBonus();
            StartCoroutine(StartCooldown());
        }
    }

    private void GiveBrainBonus()
    {
        long bonusAmount = Mathf.RoundToInt(BrainCurrency.Instance.brainCurrency * bonusMultiplier);
        BrainCurrency.Instance.AddBrainCurrency(bonusAmount);
        canReceiveBonus = false;
        timerRemaining = cooldownTime;
        SaveTimerState(); // ���������� ����� ����� ��������� ������
    }

    private IEnumerator StartCooldown()
    {
        while (timerRemaining > 0)
        {
            UpdateTimerUI();
            yield return new WaitForSeconds(1f);
            timerRemaining--;
            SaveTimerState(); // ���������� ������ �������
        }

        canReceiveBonus = true;
        timerText.text = "0:00";
        timerText.color = readyColor; // ���� ����� ����� ��������
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timerRemaining / 60);
        int seconds = Mathf.FloorToInt(timerRemaining % 60);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
        timerText.color = activeColor; // ���� �� ����� �������
    }

    private void SaveTimerState()
    {
        PlayerPrefs.SetFloat(TimerRemainingKey, timerRemaining);
        PlayerPrefs.SetString(LastExitTimeKey, DateTime.UtcNow.ToString());
        PlayerPrefs.Save();
    }

    private void LoadTimerState()
    {
        if (PlayerPrefs.HasKey(TimerRemainingKey) && PlayerPrefs.HasKey(LastExitTimeKey))
        {
            timerRemaining = PlayerPrefs.GetFloat(TimerRemainingKey, 0f);
            string lastExitTimeStr = PlayerPrefs.GetString(LastExitTimeKey, "");

            if (!string.IsNullOrEmpty(lastExitTimeStr))
            {
                DateTime lastExitTime = DateTime.Parse(lastExitTimeStr);
                double elapsedSeconds = (DateTime.UtcNow - lastExitTime).TotalSeconds;

                timerRemaining -= (float)elapsedSeconds;
                if (timerRemaining <= 0)
                {
                    timerRemaining = 0;
                    canReceiveBonus = true;
                }
            }
        }
    }
}
