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
    public GameObject petPanel;

    public NotificationIcon notificationIcon;

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
        selectedCounter.text = $"{selectedPets.Count}/{maxSelectedPets}";

        // Очищаем контейнеры перед обновлением
        foreach (Transform child in selectedPetsContainer) Destroy(child.gameObject);
        foreach (Transform child in allPetsContainer) Destroy(child.gameObject);

        // Сортируем всех питомцев по множителю (от большего к меньшему)
        allPets.Sort((a, b) => b.Power.CompareTo(a.Power));

        // Заполняем список выбранных питомцев
        foreach (var pet in selectedPets)
        {
            var petUI = Instantiate(petUIPrefab, selectedPetsContainer);
            var petButton = petUI.GetComponent<PetButton>();
            petButton.SetPet(pet);

            petUI.GetComponent<Button>().onClick.AddListener(() => DeselectPet(pet));
        }

        // Заполняем список всех питомцев (уже отсортированный)
        foreach (var pet in allPets)
        {
            var petUI = Instantiate(petUIPrefab, allPetsContainer);
            var petButton = petUI.GetComponent<PetButton>();
            petButton.SetPet(pet);

            petUI.GetComponent<Button>().onClick.AddListener(() => SelectPet(pet));
        }
    }

    public void AddPet(Pet pet)
    {
        allPets.Add(pet);
        notificationIcon.SetNotification(true);
        UpdateUI();
    }

    public void SelectPet(Pet pet)
    {
        if (selectedPets.Count < maxSelectedPets && !selectedPets.Contains(pet))
        {
            selectedPets.Add(pet);
            allPets.Remove(pet);
            SpawnPetPrefab(pet);
            UpdateUI();
            UpdatePetMultiplier(); // Обновляем множитель питомцев
        }
    }

    public void DeselectPet(Pet pet)
    {
        if (selectedPets.Contains(pet))
        {
            selectedPets.Remove(pet);
            allPets.Add(pet);
            RemovePetPrefab(pet);
            UpdateUI();
            UpdatePetMultiplier(); // Обновляем множитель питомцев
        }
    }
    private void UpdatePetMultiplier()
    {
        float totalPetMultiplier = 1f; // Начальный множитель

        foreach (var pet in selectedPets)
        {
            totalPetMultiplier += pet.Power; // Суммируем множители всех выбранных питомцев
        }

        ClickMultiplier.Instance.SetPetMultiplier(totalPetMultiplier);
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

    public void OpenPetPanel()
    {
        petPanel.gameObject.SetActive(true); // Открываем панель
        notificationIcon.SetNotification(false); // Убираем значок уведомления
        UpdateUI(); // Обновляем интерфейс
    }
}
