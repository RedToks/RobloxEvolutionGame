using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseAfterAd : MonoBehaviour
{
    public GameObject pausePanel; // Сюда назначь панель
    public Button continueButton; // Назначь кнопку "Продолжить"

    private void Start()
    {
        pausePanel.SetActive(false); // Панель скрыта при старте
        continueButton.onClick.AddListener(ResumeGame);
    }

    public void ShowPauseAfterTimer()
    {
        PauseAndShowPanel();
    }

    private void PauseAndShowPanel()
    {
        Time.timeScale = 0; // Ставим игру на паузу
        pausePanel.SetActive(true); // Показываем панель
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // Снимаем игру с паузы
        pausePanel.SetActive(false); // Скрываем панель
    }
}
