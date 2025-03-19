using UnityEngine;
using System.Collections.Generic;

public class PetOrbitManager : MonoBehaviour
{
    public Transform playerTransform; // Игрок
    public float orbitRadius = 3f; // Радиус вращения
    public float rotationSpeed = 50f; // Скорость вращения
    public float orbitHeight = 1f; // Высота вращения питомцев
    public float moveToPlayerSpeed = 10f;

    private List<GameObject> activePets = new List<GameObject>();

    void Update()
    {
        UpdatePetPositions();
    }

    public void UpdateActivePets(List<GameObject> newActivePets)
    {
        // Вместо удаления - просто скрываем старых питомцев
        foreach (var pet in activePets)
        {
            if (pet != null)
                pet.SetActive(false);
        }
        activePets.Clear();

        // Добавляем новых питомцев
        foreach (var pet in newActivePets)
        {
            if (pet != null)
            {
                pet.SetActive(true); // Включаем питомца
                AddPet(pet);
            }
        }
    }

    public void AddPet(GameObject pet)
    {
        if (!activePets.Contains(pet))
        {
            activePets.Add(pet);
            pet.SetActive(true); // Делаем питомца видимым
            UpdatePetPositions();
        }
    }

    public void RemovePet(GameObject pet)
    {
        if (activePets.Contains(pet))
        {
            activePets.Remove(pet);
            pet.SetActive(false); // Вместо удаления - просто скрываем
            UpdatePetPositions();
        }
    }

    public void ClearAllPets()
    {
        foreach (var pet in activePets)
        {
            if (pet != null)
                pet.SetActive(false);
        }
        activePets.Clear();
    }

    private void UpdatePetPositions()
    {
        int petCount = activePets.Count;
        if (petCount == 0) return;

        float angleStep = 360f / petCount;
        Vector3 playerForward = playerTransform.forward;

        Rigidbody playerRb = playerTransform.GetComponent<Rigidbody>();
        if (playerRb != null && playerRb.velocity.magnitude > 0.1f)
        {
            playerForward = playerRb.velocity.normalized;
        }

        for (int i = 0; i < petCount; i++)
        {
            GameObject pet = activePets[i];
            if (pet == null) continue;

            float angle = i * angleStep + Time.time * rotationSpeed;
            angle %= 360f;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * orbitRadius;
            offset.y = orbitHeight;
            pet.transform.position = Vector3.Lerp(pet.transform.position, playerTransform.position + offset, Time.deltaTime * moveToPlayerSpeed);

            Vector3 moveDirection = playerRb.velocity.magnitude > 0.1f ? playerRb.velocity.normalized : playerTransform.forward;

            // Убедимся, что направление не учитывает наклон по Y
            moveDirection.y = 0;

            if (moveDirection.magnitude > 0.1f)
            {
                pet.transform.rotation = Quaternion.LookRotation(moveDirection) * Quaternion.Euler(0, -90, 0);
            }
        }
    }
}
