﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using KinematicCharacterController.Examples;
using KinematicCharacterController;
using DG.Tweening;
using TMPro;

public class ResourceMining : MonoBehaviour
{
    public Slider progressBar; // Слайдер прогресса

    public Canvas CanvasUI; // Основной интерфейс
    public GameObject miningUI; // Интерфейс добычи (Canvas)
    public Camera mainCamera; // Основная камера
    public Camera miningCamera; // Камера для добычи
    public GameObject player;
    public ExamplePlayer playerController;
    public GameObject interactPrompt; // Подсказка (нажмите "E")
    public Transform startPosition; // Точка телепортации игрока в начале игры
    public Animator playerAnimator; // Аниматор игрока
    public GameObject pickAxe;
    public Transform miningRock; // Объект камня
    public TextMeshProUGUI playerBrainCoinsText;
    public TextMeshProUGUI enemyBrainCoinsText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI loseText;

    [HideInInspector] public long requiredBrainCoins = 0;

    public TextMeshProUGUI countdownText3;
    public TextMeshProUGUI countdownText2;
    public TextMeshProUGUI countdownText1;


    public GameObject rewardPrefab; // Префаб награды
    public Transform rewardSpawnPoint; // Точка появления награды
    public Sprite neuroCurrencyIcon; // Иконка валюты
    public int rewardAmount = 0; // Количество награды

    private int hitCount = 0; // Счётчик ударов
    public ParticleSystem rockParticles;
    public ParticleSystem rockBreakEffect;



    public float decreaseRate = 0.5f; // Скорость уменьшения прогресса
    public float increaseAmount = 0.1f; // Насколько увеличивается шкала за клик

    private Vector3 originalPosition;
    private bool isMining = false;
    private bool canStartMining = false; // Флаг, можно ли начать добычу
    private bool isVictory;
    private Coroutine miningCoroutine;

    [SerializeField]
    private string requiredBrainCoinsString = "0"; // Показываем строку в инспекторе

    public Button startMiningButton;
    private static ResourceMining activeMiningInstance;

    public AudioSource audioSource;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip pickaxeHitSound;


    private void Start()
    {
        if (!long.TryParse(requiredBrainCoinsString, out requiredBrainCoins))
        {
            Debug.LogError($"Ошибка преобразования {requiredBrainCoinsString} в long.");
            requiredBrainCoins = 0; // Значение по умолчанию, если парсинг не удался
        }



        startMiningButton.onClick.AddListener(() =>
        {
            if (activeMiningInstance == this) // Запускаем только если кнопка относится к текущему объекту
            {
                StartMining();
            }
        });

        loseText.gameObject.SetActive(false);
        originalPosition = miningRock.position;
        pickAxe.gameObject.SetActive(false);

        ResetMiningUI();
        interactPrompt.SetActive(false);
        countdownText3.gameObject.SetActive(false);
        countdownText2.gameObject.SetActive(false);
        countdownText1.gameObject.SetActive(false);
        UpdateBrainCoinsUI();
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out ExampleCharacterController player) && !isMining)
        {
            activeMiningInstance = this; // Сохраняем текущий объект добычи
            canStartMining = true;
            interactPrompt.SetActive(true);
            startMiningButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ExampleCharacterController player))
        {
            if (activeMiningInstance == this) // Сбрасываем только если это был активный объект
            {
                activeMiningInstance = null;
                canStartMining = false;
                interactPrompt.SetActive(false);
                startMiningButton.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        AdjustMiningSpeed();

        if (canStartMining && !isMining && (Input.GetKeyDown(KeyCode.E)))
        {
            StartMining();
        }

        if (isMining && Input.GetMouseButtonDown(0))
        {
            progressBar.value += increaseAmount;
            playerAnimator.SetTrigger("mining");
            ShakeRock();

            if (pickaxeHitSound != null)
            {
                audioSource.PlayOneShot(pickaxeHitSound);
            }

            hitCount++;

            if (hitCount % 10 == 0)
            {
                PlayRockEffect();
            }

            if (progressBar.value >= 1)
            {
                isVictory = true;
                EndMining();
            }
        }
        else if (isMining && Input.GetMouseButtonUp(0))
        {
            playerAnimator.SetBool("mining", false);
        }
    }

    private void UpdateBrainCoinsUI()
    {
        int displayedReward = rewardAmount;

        if (BuyController.isDoubleNeuroEarningsActive)
        {
            displayedReward *= 2; // Удваиваем, если бонус активен
        }

        rewardText.text = "<sprite name=\"Neuro\">+" + CurrencyFormatter.FormatCurrency(displayedReward);
        playerBrainCoinsText.text = "<sprite name=\"Brain\">" + CurrencyFormatter.FormatCurrency(BrainCurrency.Instance.brainCurrency).ToString();
        enemyBrainCoinsText.text = "<sprite name=\"Brain\">" + CurrencyFormatter.FormatCurrency(requiredBrainCoins).ToString();
    }

    private void AdjustMiningSpeed()
    {
        long playerBrainCoins = BrainCurrency.Instance.brainCurrency;

        float ratio = (float)playerBrainCoins / requiredBrainCoins; // Насколько игрок приближен к 10к

        ratio = Mathf.Clamp(ratio, 0.1f, 2f); // Ограничиваем от 0.1 до 2 (минимальная и максимальная скорость)

        increaseAmount = 0.1f * ratio; // Чем больше BrainCoins, тем быстрее заполняется
        decreaseRate = 0.09f / ratio;   // Чем больше BrainCoins, тем медленнее убывает
    }

    private void ShakeRock()
    {
        if (miningRock != null)
        {
            miningRock.DOKill(); // Останавливаем предыдущую анимацию, если она еще идет
            miningRock.position = originalPosition; // Гарантированно возвращаем на место

            miningRock.DOShakePosition(0.2f, 0.1f, 10, 90, false, true)
                .OnComplete(() => miningRock.position = originalPosition); // Восстанавливаем позицию после тряски
        }
    }

    private void ShowLoseText()
    {
        loseText.gameObject.SetActive(true);
        loseText.alpha = 0f;
        loseText.DOFade(1f, 0.5f) // Плавное появление
            .OnComplete(() =>
                loseText.DOFade(0f, 0.5f).SetDelay(1f) // Исчезновение через 1 сек
            );
    }

    private void PlayRockEffect()
    {
        if (rockParticles != null)
        {
            rockParticles.Play();
        }
    }

    private IEnumerator DecreaseProgressOverTime()
    {
        while (isMining) // Пока игрок добывает
        {
            progressBar.value -= decreaseRate * Time.deltaTime;

            if (progressBar.value <= 0)
            {
                isVictory = false;
                EndMining();
            }

            yield return null;
        }
    }

    private void StartMining()
    {
        UpdateBrainCoinsUI();
        StartMiningProcess();
    }

    private IEnumerator StartCountdownCoroutine()
    {
        countdownText3.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdownText3.gameObject.SetActive(false);

        countdownText2.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdownText2.gameObject.SetActive(false);

        countdownText1.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countdownText1.gameObject.SetActive(false);

        isMining = true;

        if (miningCoroutine != null) StopCoroutine(miningCoroutine);
        miningCoroutine = StartCoroutine(DecreaseProgressOverTime());
    }


    private void StartMiningProcess()
    {
        canStartMining = false;

        FindObjectOfType<KinematicCharacterMotor>().enabled = false;
        playerAnimator.Rebind();
        playerController.enabled = false;
        player.transform.position = startPosition.position;
        player.transform.rotation = startPosition.rotation;

        interactPrompt.SetActive(false);

        pickAxe.gameObject.SetActive(true);
        CanvasUI.gameObject.SetActive(false);
        miningUI.SetActive(true);

        mainCamera.gameObject.SetActive(false);
        miningCamera.gameObject.SetActive(true);

        progressBar.value = 0.5f;

        isVictory = false;
        StartCoroutine(StartCountdownCoroutine());
    }

    private void EndMining()
    {
        StartCoroutine(EndMiningCoroutine());
    }

    private IEnumerator EndMiningCoroutine()
    {
        isMining = false;
        playerAnimator.SetBool("mining", false);
        pickAxe.gameObject.SetActive(false);

        if (miningCoroutine != null)
        {
            StopCoroutine(miningCoroutine);
            miningCoroutine = null;
        }

        if (isVictory)
        {
            ShowReward();
            PlayRockBreakEffect();
            if (winSound != null)
            {
                audioSource.PlayOneShot(winSound);
            }
        }

        if (!isVictory)
        {
            ShowLoseText();
            if (loseSound != null)
            {
                audioSource.PlayOneShot(loseSound);
            }
        }

        yield return new WaitForSeconds(3f);

        ResetMiningUI();
        miningUI.SetActive(false);
        CanvasUI.gameObject.SetActive(true);

        FindObjectOfType<KinematicCharacterMotor>().enabled = true;
        playerController.enabled = true;
        mainCamera.gameObject.SetActive(true);
        miningCamera.gameObject.SetActive(false);
        miningRock.gameObject.SetActive(true);
    }

    private void PlayRockBreakEffect()
    {
        miningRock.gameObject.SetActive(false); // Отключаем объект камня
        rockBreakEffect.Play(); // Запускаем частицы
    }

    private void ShowReward()
    {
        int finalRewardAmount = rewardAmount;

        // Проверяем, активирован ли x2 NeuroCoins
        if (BuyController.isDoubleNeuroEarningsActive)
        {
            finalRewardAmount *= 2; // Удваиваем награду
        }

        // Отображаем награду в UI
        GameObject rewardInstance = Instantiate(rewardPrefab, rewardSpawnPoint);
        rewardInstance.GetComponent<RewardUIController>().ShowReward(finalRewardAmount, neuroCurrencyIcon);

        // Начисляем валюту
        NeuroCurrency.Instance.AddCoinCurrency(finalRewardAmount);

        // Обновляем текст награды в UI
        rewardText.text = "<sprite name=\"Neuro\">+" + CurrencyFormatter.FormatCurrency(finalRewardAmount);
    }

    private void ResetMiningUI()
    {
        miningUI.SetActive(false);
        progressBar.value = 0.5f;
    }
}
