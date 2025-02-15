using UnityEngine;

public class HairColorPicker : MonoBehaviour
{
    public HairManager hairManager; // Ссылка на HairManager
    public FlexibleColorPicker colorPicker; // Ссылка на FlexibleColorPicker
    private Renderer currentHairRenderer;

    private void Start()
    {
        SetCurrentHairRenderer(hairManager.hairs[PlayerPrefs.GetInt("SelectedHair", 0)].hairPrefab);
    }

    private void Update()
    {
        UpdateHairColor(colorPicker.color); // Постоянно обновляем цвет волос
    }

    public void SetCurrentHairRenderer(GameObject hairPrefab)
    {
        if (hairPrefab != null)
        {
            currentHairRenderer = hairPrefab.GetComponentInChildren<Renderer>();
            UpdateHairColor(colorPicker.color); // Применяем текущий цвет сразу
        }
    }

    private void UpdateHairColor(Color newColor)
    {
        if (currentHairRenderer != null)
        {
            currentHairRenderer.material.SetColor("_BaseColor", newColor);
        }
    }
}
