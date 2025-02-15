using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

namespace CoppraGames
{
    public class SpinWheelController : MonoBehaviour
    {
        [System.Serializable]
        public class RewardItem
        {
            public Sprite icon;
            public int count;
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

        public void TurnWheel()
        {
            if (_isStarted || !_canSpin)
                return;

            SpinWheelArrowCollider.gameObject.SetActive(true);
            SpinWheelArrowCollider.enabled = true;

            _isStarted = true;
            _startAngle = Wheel.localEulerAngles.z;
            int totalSlots = rewards.Length;
            _randomRewardIndex = Random.Range(0, totalSlots);

            int rotationCount = Random.Range(10, 15);
            _endAngle = -(rotationCount * 360 + _randomRewardIndex * 360 / totalSlots);

            _currentRotationTime = 0.0f;
            _maxRotationTime = Random.Range(5.0f, 9.0f);

            PlayerPrefs.SetString(LAST_SPIN_TIME_KEY, DateTime.UtcNow.ToString());
            PlayerPrefs.Save();

            // Запускаем таймер на заданное в инспекторе время
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
            ShowResult(_randomRewardIndex);
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
            StartCoroutine(_ShowResult(resultIndex));
        }

        private IEnumerator _ShowResult(int resultIndex)
        {
            if (ResultPanel)
            {
                ResultPanel.SetActive(true);
                int actualRewardIndex = rewards.Length - resultIndex; // because wheel rounds in clockwise

                if (rewards.Length > actualRewardIndex)
                {
                    ResultIcon.sprite = rewards[actualRewardIndex].icon;
                    ResultCount.text = "x" + rewards[actualRewardIndex].count.ToString();
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
            UpdateCooldownText();
        }

        private void UpdateCooldown()
        {
            if (!_canSpin)
            {
                string lastSpinTimeString = PlayerPrefs.GetString(LAST_SPIN_TIME_KEY, "");
                if (!string.IsNullOrEmpty(lastSpinTimeString))
                {
                    DateTime lastSpinTime = DateTime.Parse(lastSpinTimeString);
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
            string lastSpinTimeString = PlayerPrefs.GetString(LAST_SPIN_TIME_KEY, "");
            if (!string.IsNullOrEmpty(lastSpinTimeString))
            {
                DateTime lastSpinTime = DateTime.Parse(lastSpinTimeString);
                TimeSpan timePassed = DateTime.UtcNow - lastSpinTime;
                _timeRemaining = SpinCooldown - (float)timePassed.TotalSeconds;

                if (_timeRemaining <= 0)
                {
                    _canSpin = true;
                    _timeRemaining = 0;
                }
                else
                {
                    _canSpin = false;
                }
            }
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
