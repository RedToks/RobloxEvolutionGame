using UnityEngine;
using TMPro;
using System.Collections;
using System;
using YG;

public class TriggerBrainBonus : MonoBehaviour
{
    [Header("Настройки таймера")]
    public TextMeshPro timerText;
    public float cooldownTime = 600f;

    [Header("Настройки бонуса")]
    [Range(0f, 1f)] public float bonusMultiplier = 0.1f;

    [Header("Цвета таймера")]
    public Color activeColor = Color.red;
    public Color readyColor = Color.green;

    private bool canReceiveBonus = false;
    private float timerRemaining = 0f;
    private bool isSavingAllowed = false; // 🔹 Флаг разрешения сохранения

    private void Start()
    {
        LoadTimerState();

        if (timerRemaining > 0)
        {
            StartCoroutine(StartCooldown());
        }
        else
        {
            SetBonusReady();
        }

        StartCoroutine(AutoSaveRoutine()); // 🔹 Запуск автосохранения раз в 10 секунд
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canReceiveBonus)
        {
            GiveBrainBonus();
            StartCoroutine(StartCooldown());
        }
    }

    private void GiveBrainBonus()
    {
        int bonusAmount = Mathf.RoundToInt(BrainCurrency.Instance.brainCurrency * bonusMultiplier);
        BrainCurrency.Instance.AddBrainCurrency(bonusAmount);

        canReceiveBonus = false;
        timerRemaining = cooldownTime;
        RequestSave();
        StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        while (timerRemaining > 0)
        {
            UpdateTimerUI();
            yield return new WaitForSeconds(1f);
            timerRemaining--;
            RequestSave();
        }

        SetBonusReady();
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timerRemaining / 60);
        int seconds = Mathf.FloorToInt(timerRemaining % 60);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
        timerText.color = activeColor;
    }

    private void SetBonusReady()
    {
        canReceiveBonus = true;
        timerText.color = readyColor;
    }

    private void SaveTimerState()
    {
        YG2.saves.bonusTimerRemaining = timerRemaining;
        YG2.saves.lastExitTime = DateTime.UtcNow.ToString();

        isSavingAllowed = false;
    }

    private void LoadTimerState()
    {
        if (!string.IsNullOrEmpty(YG2.saves.lastExitTime))
        {
            timerRemaining = YG2.saves.bonusTimerRemaining;
            DateTime lastExitTime = DateTime.Parse(YG2.saves.lastExitTime);
            double elapsedSeconds = (DateTime.UtcNow - lastExitTime).TotalSeconds;

            timerRemaining -= (float)elapsedSeconds;
            if (timerRemaining <= 0)
            {
                timerRemaining = 0;
                SetBonusReady();
            }
        }
        else
        {
            timerRemaining = cooldownTime;
        }
    }

    // 🔹 Корутина автосохранения раз в 10 секунд
    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            if (isSavingAllowed)
            {
                SaveTimerState();
            }
        }
    }

    // 🔹 Запрос на сохранение
    private void RequestSave()
    {
        isSavingAllowed = true;
    }
}
