using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PetPanelUI : MonoBehaviour
{
    public Transform selectedPetsContainer; // Верхняя сетка (выбранные питомцы)
    public Transform allPetsContainer; // Нижняя сетка (все питомцы)
    public GameObject petUIPrefab; // Префаб элемента UI для питомца
    public TextMeshProUGUI selectedCounter; // Текст для счетчика 4/3
    public Transform playerTransform; // Ссылка на игрока
    public Button equipBestButton; // Кнопка "Оснастить лучших"

    [SerializeField] private List<Pet> allPets = new List<Pet>(); // Все питомцы
    private List<Pet> selectedPets = new List<Pet>(); // Выбранные питомцы
    private Dictionary<Pet, GameObject> activePets = new Dictionary<Pet, GameObject>(); // Активные питомцы в сцене
    private const int maxSelectedPets = 3; // Максимум выбранных питомцев
    private PetOrbitManager petOrbitManager;

    void Start()
    {
        petOrbitManager = FindObjectOfType<PetOrbitManager>();

        // Привязываем кнопку "Оснастить лучших" к методу
        equipBestButton.onClick.AddListener(EquipBestPets);

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Обновляем счетчик выбранных питомцев
        selectedCounter.text = $"{selectedPets.Count}/{maxSelectedPets}";

        // Удаляем старые элементы UI
        foreach (Transform child in selectedPetsContainer) Destroy(child.gameObject);
        foreach (Transform child in allPetsContainer) Destroy(child.gameObject);

        // Обновляем выбранных питомцев (верхняя сетка)
        foreach (var pet in selectedPets)
        {
            var petUI = Instantiate(petUIPrefab, selectedPetsContainer);
            petUI.GetComponentInChildren<TextMeshProUGUI>().text = $"x{pet.Power}";
            petUI.GetComponentInChildren<Image>().sprite = pet.Icon;

            // Добавляем кнопку для снятия выбора
            var button = petUI.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => DeselectPet(pet));
        }

        // Обновляем всех питомцев (нижняя сетка)
        foreach (var pet in allPets)
        {
            var petUI = Instantiate(petUIPrefab, allPetsContainer);
            petUI.GetComponentInChildren<TextMeshProUGUI>().text = $"x{pet.Power}";
            petUI.GetComponentInChildren<Image>().sprite = pet.Icon;

            // Добавляем кнопку для выбора питомца
            var button = petUI.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => SelectPet(pet));
        }
    }

    public void AddPet(Pet pet)
    {
        allPets.Add(pet);
        UpdateUI();
    }

    public void SelectPet(Pet pet)
    {
        if (selectedPets.Count < maxSelectedPets && !selectedPets.Contains(pet))
        {
            selectedPets.Add(pet);
            allPets.Remove(pet);
            SpawnPetPrefab(pet); // Создаем питомца в мире
            UpdateUI();
        }
    }

    public void DeselectPet(Pet pet)
    {
        if (selectedPets.Contains(pet))
        {
            selectedPets.Remove(pet);
            allPets.Add(pet);
            RemovePetPrefab(pet); // Убираем питомца из мира
            UpdateUI();
        }
    }

    private void SpawnPetPrefab(Pet pet)
    {
        if (pet.Prefab != null)
        {
            // Создаем экземпляр питомца
            GameObject spawnedPet = Instantiate(pet.Prefab);
            petOrbitManager.AddPet(spawnedPet); // Добавляем в менеджер вращения
            activePets[pet] = spawnedPet;
        }
    }

    private void RemovePetPrefab(Pet pet)
    {
        if (activePets.ContainsKey(pet))
        {
            petOrbitManager.RemovePet(activePets[pet]); // Убираем из менеджера вращения
            activePets.Remove(pet);
        }
    }

    public void EquipBestPets()
    {
        // Снимаем всех текущих питомцев
        foreach (var pet in new List<Pet>(selectedPets))
        {
            DeselectPet(pet);
        }

        // Сортируем питомцев по силе
        var bestPets = new List<Pet>(allPets);
        bestPets.Sort((a, b) => b.Power.CompareTo(a.Power)); // Сортировка по убыванию силы

        // Выбираем максимум доступных питомцев
        for (int i = 0; i < Mathf.Min(maxSelectedPets, bestPets.Count); i++)
        {
            SelectPet(bestPets[i]);
        }
    }
}
