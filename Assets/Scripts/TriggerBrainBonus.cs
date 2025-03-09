using UnityEngine;
using TMPro;
using System.Collections;
using System;
using KinematicCharacterController.Examples;

public class TriggerBrainBonus : MonoBehaviour
{
    [Header("Настройки таймера")]
    public TextMeshPro timerText; // Текст для отображения таймера
    public float cooldownTime = 600f; // 10 минут (600 секунд)

    [Header("Настройки бонуса")]
    [Range(0f, 1f)] public float bonusMultiplier = 0.1f; // Множитель бонуса (по умолчанию 10%)

    [Header("Цвета таймера")]
    public Color activeColor = Color.red;  // Цвет во время таймера
    public Color readyColor = Color.green; // Цвет, когда бонус доступен

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
        SaveTimerState(); // Сохранение сразу после получения бонуса
    }

    private IEnumerator StartCooldown()
    {
        while (timerRemaining > 0)
        {
            UpdateTimerUI();
            yield return new WaitForSeconds(1f);
            timerRemaining--;
            SaveTimerState(); // Сохранение каждую секунду
        }

        canReceiveBonus = true;
        timerText.text = "0:00";
        timerText.color = readyColor; // Цвет когда бонус доступен
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timerRemaining / 60);
        int seconds = Mathf.FloorToInt(timerRemaining % 60);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
        timerText.color = activeColor; // Цвет во время таймера
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
