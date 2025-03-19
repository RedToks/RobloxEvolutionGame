using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using YG;

public class PetPanelUI : MonoBehaviour
{
    public Transform selectedPetsContainer;
    public Transform allPetsContainer;
    public GameObject petUIPrefab;
    public TextMeshProUGUI selectedCounter;
    public Transform playerTransform;
    public Button equipBestButton;
    public GameObject petPanel;
    public NotificationIcon notificationIcon;

    [SerializeField] private List<Pet> allPets = new List<Pet>();
    private List<Pet> selectedPets = new List<Pet>();
    private Dictionary<Pet, GameObject> activePets = new Dictionary<Pet, GameObject>();
    private const int maxSelectedPets = 3;
    private PetOrbitManager petOrbitManager;

    private void Start()
    {
        petOrbitManager = FindObjectOfType<PetOrbitManager>();
        equipBestButton.onClick.AddListener(EquipBestPets);

        LoadPets();
        UpdateUI();

        InvokeRepeating(nameof(AutoSavePets), 10f, 10f); // Автосохранение раз в 10 сек
    }

    private void SavePets()
    {
        YG2.saves.allPets = allPets.Select(pet => new PetData
        {
            IconName = pet.Icon ? pet.Icon.name : "",
            PrefabName = pet.Prefab ? pet.Prefab.name : "",
            Power = pet.Power,
            Rarity = pet.Rarity
        }).ToList();

        YG2.saves.selectedPets = selectedPets.Select(pet => new PetData
        {
            IconName = pet.Icon ? pet.Icon.name : "",
            PrefabName = pet.Prefab ? pet.Prefab.name : "",
            Power = pet.Power,
            Rarity = pet.Rarity
        }).ToList();

    }

    private void AutoSavePets()
    {
        SavePets();
    }

    private void LoadPets()
    {
        allPets.Clear();
        selectedPets.Clear();

        foreach (var pet in activePets.Values) Destroy(pet);
        activePets.Clear();

        foreach (var petData in YG2.saves.allPets)
        {
            Pet pet = CreatePetFromData(petData);
            if (pet != null) allPets.Add(pet);
        }

        foreach (var petData in YG2.saves.selectedPets)
        {
            Pet pet = CreatePetFromData(petData);
            if (pet != null)
            {
                selectedPets.Add(pet);
                SpawnPet(pet);
            }
        }

        petOrbitManager.UpdateActivePets(activePets.Values.ToList());
    }

    private void SpawnPet(Pet pet)
    {
        if (activePets.ContainsKey(pet)) return;

        if (pet.Prefab != null && playerTransform != null)
        {
            GameObject spawnedPet = Instantiate(pet.Prefab, playerTransform.position, Quaternion.identity);
            petOrbitManager.AddPet(spawnedPet);
            activePets[pet] = spawnedPet;
        }
    }

    private Pet CreatePetFromData(PetData petData)
    {
        Sprite icon = Resources.Load<Sprite>("PetIcons/" + petData.IconName);
        GameObject prefab = Resources.Load<GameObject>("PetPrefabs/" + petData.PrefabName);

        if (prefab == null) Debug.LogError($"❌ Префаб {petData.PrefabName} не найден в Resources!");
        if (icon == null) Debug.LogError($"❌ Иконка {petData.IconName} не найдена в Resources!");

        return new Pet(icon, prefab, petData.Power, petData.Rarity);
    }

    private void UpdateUI()
    {
        selectedCounter.text = $"{selectedPets.Count}/{maxSelectedPets}";

        foreach (Transform child in selectedPetsContainer) Destroy(child.gameObject);
        foreach (Transform child in allPetsContainer) Destroy(child.gameObject);

        allPets.Sort((a, b) => b.Power.CompareTo(a.Power));

        foreach (var pet in selectedPets)
        {
            var petUI = Instantiate(petUIPrefab, selectedPetsContainer);
            var petButton = petUI.GetComponent<PetButton>();
            petButton.SetPet(pet);

            petUI.GetComponent<Button>().onClick.AddListener(() => DeselectPet(pet));
        }

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
        SavePets();
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
            SavePets();
            UpdateUI();
            UpdatePetMultiplier();

            petOrbitManager.UpdateActivePets(activePets.Values.ToList());
        }
    }

    public void DeselectPet(Pet pet)
    {
        if (selectedPets.Contains(pet))
        {
            selectedPets.Remove(pet);
            allPets.Add(pet);
            RemovePetPrefab(pet);
            SavePets();
            UpdateUI();
            UpdatePetMultiplier();

            petOrbitManager.UpdateActivePets(activePets.Values.ToList());
        }
    }

    private void UpdatePetMultiplier()
    {
        float totalPetMultiplier = 1f;
        foreach (var pet in selectedPets)
        {
            totalPetMultiplier += pet.Power;
        }
        ClickMultiplier.Instance.SetPetMultiplier(totalPetMultiplier);
    }

    private void SpawnPetPrefab(Pet pet)
    {
        if (pet.Prefab != null)
        {
            GameObject spawnedPet = Instantiate(pet.Prefab);
            petOrbitManager.AddPet(spawnedPet);
            activePets[pet] = spawnedPet;
        }
    }

    private void RemovePetPrefab(Pet pet)
    {
        if (activePets.ContainsKey(pet))
        {
            GameObject petObject = activePets[pet];
            petOrbitManager.RemovePet(petObject);
            activePets.Remove(pet);
            Destroy(petObject);
        }
    }

    public void EquipBestPets()
    {
        foreach (var pet in new List<Pet>(selectedPets))
        {
            DeselectPet(pet);
        }

        var bestPets = new List<Pet>(allPets);
        bestPets.Sort((a, b) => b.Power.CompareTo(a.Power));

        for (int i = 0; i < Mathf.Min(maxSelectedPets, bestPets.Count); i++)
        {
            SelectPet(bestPets[i]);
        }

        petOrbitManager.UpdateActivePets(activePets.Values.ToList());
    }

    public void OpenPetPanel()
    {
        petPanel.gameObject.SetActive(true);
        notificationIcon.SetNotification(false);
        UpdateUI();
    }
}
