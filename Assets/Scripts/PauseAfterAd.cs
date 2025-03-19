using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseAfterAd : MonoBehaviour
{
    public GameObject pausePanel; // ���� ������� ������
    public Button continueButton; // ������� ������ "����������"

    private void Start()
    {
        pausePanel.SetActive(false); // ������ ������ ��� ������
        continueButton.onClick.AddListener(ResumeGame);
    }

    public void ShowPauseAfterTimer()
    {
        PauseAndShowPanel();
    }

    private void PauseAndShowPanel()
    {
        Time.timeScale = 0; // ������ ���� �� �����
        pausePanel.SetActive(true); // ���������� ������
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // ������� ���� � �����
        pausePanel.SetActive(false); // �������� ������
    }
}
