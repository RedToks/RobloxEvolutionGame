using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using KinematicCharacterController.Examples;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider; // Слайдер для чувствительности

    private GameSettings currentSettings;
    private SaveLoad saveLoad;
    private ExampleCharacterCamera cameraController;

    private void Start()
    {
        cameraController = FindObjectOfType<ExampleCharacterCamera>();
        saveLoad = FindObjectOfType<SaveLoad>();
        if (saveLoad == null)
        {
            Debug.LogError("SaveLoad component not found in the scene.");
            return;
        }

        currentSettings = saveLoad.LoadSettings();
        if (currentSettings == null)
        {
            Debug.LogWarning("No saved settings found. Using default settings.");
            currentSettings = new GameSettings();
        }

        // Логируем загруженные настройки
        Debug.Log($"Loaded Settings: Volume={currentSettings.volume}, Sensitivity={currentSettings.sensitivity}, QualityLevel={currentSettings.qualityLevel}");

        // Применяем настройки громкости
        if (audioMixer != null)
        {
            audioMixer.SetFloat("volume", currentSettings.volume);
            Debug.Log($"Audio volume set to {currentSettings.volume}");
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = currentSettings.volume;
        }

        // Применяем чувствительность
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = currentSettings.sensitivity;
        }
       
        cameraController.RotationSpeed = currentSettings.sensitivity;
    }

    public void SetVolume(float volume)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("volume", volume);
            Debug.Log($"Volume changed to {volume}");
        }

        currentSettings.volume = volume;
        saveLoad.SaveSettings(currentSettings);
    }

    public void SetSensitivity(float sensitivity)
    {
        Debug.Log($"Sensitivity changed to {sensitivity}");
        currentSettings.sensitivity = sensitivity;
        saveLoad.SaveSettings(currentSettings);

        cameraController.RotationSpeed = currentSettings.sensitivity;
    }
}
