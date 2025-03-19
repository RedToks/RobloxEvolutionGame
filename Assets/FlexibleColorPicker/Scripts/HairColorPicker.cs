using UnityEngine;

public class HairColorPicker : MonoBehaviour
{
    public HairManager hairManager;
    public FlexibleColorPicker colorPicker;
    private Renderer currentHairRenderer;

    private void Start()
    {
        int selectedHairIndex = PlayerPrefs.GetInt("SelectedHair", 0);
        SetCurrentHairRenderer(hairManager.hairs[selectedHairIndex].hairPrefab);

        // Загружаем и применяем сохранённый цвет
        Color savedColor = LoadHairColor();
        colorPicker.color = savedColor;
        ApplyHairColor(savedColor);

        // Добавляем обработчик события изменения цвета
        colorPicker.onColorChange.AddListener(OnColorChange);
    }

    private void OnDestroy()
    {
        colorPicker.onColorChange.RemoveListener(OnColorChange);
    }

    public void SetCurrentHairRenderer(GameObject hairPrefab)
    {
        if (hairPrefab != null)
        {
            currentHairRenderer = hairPrefab.GetComponentInChildren<Renderer>();

            // Применяем цвет сразу после установки нового Renderer
            Color savedColor = LoadHairColor();
            ApplyHairColor(savedColor);
        }
    }

    private void OnColorChange(Color newColor)
    {
        ApplyHairColor(newColor);
        SaveHairColor(newColor);
    }

    private void ApplyHairColor(Color color)
    {
        if (currentHairRenderer != null)
        {
            if (currentHairRenderer.material.HasProperty("_BaseColor"))
            {
                currentHairRenderer.material.SetColor("_BaseColor", color);
            }
            else
            {
                currentHairRenderer.material.color = color;
            }
        }
    }

    private void SaveHairColor(Color color)
    {
        PlayerPrefs.SetFloat("HairColor_R", color.r);
        PlayerPrefs.SetFloat("HairColor_G", color.g);
        PlayerPrefs.SetFloat("HairColor_B", color.b);
        PlayerPrefs.SetFloat("HairColor_A", color.a);
        PlayerPrefs.Save();
    }

    private Color LoadHairColor()
    {
        return new Color(
            PlayerPrefs.GetFloat("HairColor_R", 1f),
            PlayerPrefs.GetFloat("HairColor_G", 1f),
            PlayerPrefs.GetFloat("HairColor_B", 1f),
            PlayerPrefs.GetFloat("HairColor_A", 1f)
        );
    }
}
