using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using YG;

namespace CoppraGames
{
    public class SpinWheelController : MonoBehaviour
    {
        public enum RewardType
        {
            Pet,
            BrainCoins,
            CoinCoins
        }

        [System.Serializable]
        public class RewardItem
        {
            public RewardType type;
            public Sprite icon;
            public int count;

            public GameObject petPrefab;
            public string petName;
            public float petStrength;
            public Sprite petIcon;
        }

        public RewardItem[] rewards;
        public RewardItemComponent[] rewardItemComponents;

        public Transform Wheel;
        public AnimationCurve Curve;

        public Collider2D SpinWheelArrowCollider;

        public GameObject ResultPanel;
        public Image ResultIcon;
        public TextMeshProUGUI ResultCount;
        public Button SpinButton; // Кнопка для кручения
        public TextMeshProUGUI SpinCooldownText; // Текстовый UI для таймера
        public NotificationIcon notificationIcon;

        private bool _isStarted;
        private float _startAngle;
        private float _endAngle;
        private int _randomRewardIndex = 0;
        private float _currentRotationTime;
        private float _maxRotationTime;

        private bool _canSpin = true;
        private float _timeRemaining = 0f;
        [SerializeField] private float SpinCooldown = 300f; // Время ожидания в секундах (по умолчанию 5 минут)
        private const string LAST_SPIN_TIME_KEY = "LastSpinTime";

        private void Awake()
        {
            HideResult();
            LoadCooldown();
            UpdateSpinButtonState();
            Init();
        }

        public void Init()
        {
            ApplyValues();
            UpdateSpinButtonState();
        }

        private void TurnWheel()
        {
            if (_isStarted || !_canSpin)
                return;

            _isStarted = true;
            _startAngle = Wheel.localEulerAngles.z;
            int totalSlots = rewards.Length;
            _randomRewardIndex = Random.Range(0, totalSlots);

            Debug.Log($"Выпал индекс награды: {_randomRewardIndex} - {rewards[_randomRewardIndex].type}");

            int rotationCount = Random.Range(10, 15);
            _endAngle = -(rotationCount * 360 + _randomRewardIndex * 360 / totalSlots);

            _currentRotationTime = 0.0f;
            _maxRotationTime = Random.Range(5.0f, 9.0f);

            // Сохраняем время спина в YG2.saves
            YG2.saves.lastSpinTime = DateTime.UtcNow.ToString();
            YG2.SaveProgress();

            _canSpin = false;
            _timeRemaining = SpinCooldown;
            UpdateSpinButtonState();
        }


        void Update()
        {
            UpdateCooldown();
            if (_isStarted)
            {
                float t = _currentRotationTime / _maxRotationTime;
                t = Curve.Evaluate(t);
                float angle = Mathf.Lerp(_startAngle, _endAngle, t);
                Wheel.localRotation = Quaternion.Euler(0, 0, angle);

                if (angle <= _endAngle)
                {
                    _isStarted = false;
                    SettleWheel();
                }

                _currentRotationTime += Time.deltaTime;
            }

            // Таймер на ожидание спина
            if (!_canSpin)
            {
                _timeRemaining -= Time.deltaTime;
                if (_timeRemaining <= 0)
                {
                    _canSpin = true;
                    _timeRemaining = 0;
                    UpdateSpinButtonState();
                }
                UpdateCooldownText();
            }
        }

        void SettleWheel()
        {
            SpinWheelArrowCollider.enabled = false;

            int actualRewardIndex = GetActualRewardIndex();

            Debug.Log($"Фактический индекс награды: {actualRewardIndex} - {rewards[actualRewardIndex].type}");
            Debug.Log($"Запланированный индекс награды: {_randomRewardIndex} - {rewards[_randomRewardIndex].type}");

            if (_randomRewardIndex != actualRewardIndex)
            {
                Debug.LogWarning($"Несовпадение индексов! Ожидалось {_randomRewardIndex}, но фактически выпало {actualRewardIndex}");
            }

            ShowResult(actualRewardIndex);
        }

        int GetActualRewardIndex()
{
    float currentAngle = Wheel.localEulerAngles.z;
    int totalSlots = rewards.Length;

    if (currentAngle < 0) currentAngle += 360; // Приводим угол к диапазону 0-360

    float slotSize = 360f / totalSlots;
    int index = Mathf.RoundToInt(currentAngle / slotSize) % totalSlots;

    return index;
}

        public void ApplyValues()
        {
            int index = 0;
            foreach (var r in rewards)
            {
                if (rewardItemComponents.Length > index)
                {
                    rewardItemComponents[index].SetData(r);
                }
                index++;
            }
        }

        public void ShowResult(int resultIndex)
        {
            GiveReward(resultIndex);
            StartCoroutine(_ShowResult(resultIndex));
        }

        private IEnumerator _ShowResult(int resultIndex)
        {
            if (ResultPanel)
            {
                ResultPanel.SetActive(true);
                int actualRewardIndex = resultIndex;

                if (rewards.Length > actualRewardIndex)
                {
                    ResultIcon.sprite = rewards[actualRewardIndex].icon;
                    ResultCount.text = "x" + CurrencyFormatter.FormatCurrency(rewards[actualRewardIndex].count);
                }

                ResultPanel.GetComponent<Animator>().Play("clip");
            }
            yield return new WaitForSeconds(3.3f);
            HideResult();
        }

        public void HideResult()
        {
            if (ResultPanel)
            {
                ResultPanel.SetActive(false);
            }
        }

        void GiveReward(int rewardIndex)
        {
            Debug.Log($"Выдаём награду: {rewardIndex} - {rewards[rewardIndex].type}");

            RewardItem reward = rewards[rewardIndex];
            switch (reward.type)
            {
                case RewardType.Pet:
                    GivePet(reward);
                    break;
                case RewardType.BrainCoins:
                    BrainCurrency.Instance.AddBrainCurrency(reward.count);
                    Debug.Log($"Игрок получил {reward.count} BrainCoins!");
                    break;
                case RewardType.CoinCoins:
                    NeuroCurrency.Instance.AddCoinCurrency(reward.count);
                    Debug.Log($"Игрок получил {reward.count} CoinCoins!");
                    break;
            }
        }

        void GivePet(RewardItem reward)
        {
            if (reward.petPrefab != null)
            {
                // **Создаем питомца с характеристиками из награды**
                Pet newPet = new Pet(reward.petIcon, reward.petPrefab, reward.petStrength, Pet.PetRarity.Special);

                // **Добавляем питомца в панель питомцев**
                FindObjectOfType<PetPanelUI>().AddPet(newPet);
            }
        }



        public void Close()
        {
            Main.instance.ShowSpinWheelWindow(false);
        }

        private void UpdateSpinButtonState()
        {
            if (SpinButton != null)
            {
                SpinButton.interactable = _canSpin;
            }
            if (notificationIcon != null)
            {
                notificationIcon.SetNotification(_canSpin);
            }
            UpdateCooldownText();
        }

        private void UpdateCooldown()
        {
            if (!_canSpin)
            {
                if (!string.IsNullOrEmpty(YG2.saves.lastSpinTime))
                {
                    DateTime lastSpinTime = DateTime.Parse(YG2.saves.lastSpinTime);
                    TimeSpan timePassed = DateTime.UtcNow - lastSpinTime;
                    _timeRemaining = SpinCooldown - (float)timePassed.TotalSeconds;

                    if (_timeRemaining <= 0)
                    {
                        _canSpin = true;
                        _timeRemaining = 0;
                        UpdateSpinButtonState();
                    }
                }
                UpdateCooldownText();
            }
        }

        private void LoadCooldown()
        {
            if (!string.IsNullOrEmpty(YG2.saves.lastSpinTime))
            {
                DateTime lastSpinTime = DateTime.Parse(YG2.saves.lastSpinTime);
                TimeSpan timePassed = DateTime.UtcNow - lastSpinTime;
                _timeRemaining = SpinCooldown - (float)timePassed.TotalSeconds;

                _canSpin = _timeRemaining <= 0;
                if (_canSpin)
                {
                    _timeRemaining = 0;
                }
            }
            else
            {
                // Если данных нет, устанавливаем начальное время
                _canSpin = false;
                _timeRemaining = SpinCooldown;
                YG2.saves.lastSpinTime = DateTime.UtcNow.ToString();
                YG2.SaveProgress();
            }

            UpdateSpinButtonState();
        }


        private void UpdateCooldownText()
        {
            if (SpinCooldownText != null)
            {
                if (_canSpin)
                {
                    SpinCooldownText.gameObject.SetActive(false); // Скрываем текст, если можно крутить
                }
                else
                {
                    SpinCooldownText.gameObject.SetActive(true);
                    int minutes = Mathf.FloorToInt(_timeRemaining / 60);
                    int seconds = Mathf.FloorToInt(_timeRemaining % 60);
                    SpinCooldownText.text = $"{minutes:D2}:{seconds:D2}";
                }
            }
        }
    }
}
