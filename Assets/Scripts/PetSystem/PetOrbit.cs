using UnityEngine;
using System.Collections.Generic;

public class PetOrbitManager : MonoBehaviour
{
    public Transform playerTransform; // Игрок
    public float orbitRadius = 3f; // Радиус вращения
    public float rotationSpeed = 50f; // Скорость вращения
    public float orbitHeight = 1f; // Высота вращения питомцев

    private List<GameObject> activePets = new List<GameObject>();

    void Update()
    {
        UpdatePetPositions();
    }

    public void AddPet(GameObject pet)
    {
        if (!activePets.Contains(pet))
        {
            activePets.Add(pet);
            UpdatePetPositions(); // Обновляем позиции после добавления питомца
        }
    }

    public void RemovePet(GameObject pet)
    {
        if (activePets.Contains(pet))
        {
            activePets.Remove(pet);
            Destroy(pet); // Удаляем питомца из сцены
            UpdatePetPositions(); // Обновляем позиции оставшихся питомцев
        }
    }

    private void UpdatePetPositions()
    {
        int petCount = activePets.Count;
        if (petCount == 0) return;

        // Угол между каждым питомцем
        float angleStep = 360f / petCount;

        for (int i = 0; i < petCount; i++)
        {
            GameObject pet = activePets[i];
            if (pet == null) continue;

            // Расчёт текущего угла питомца с учетом времени (для плавного вращения)
            float angle = i * angleStep + Time.time * rotationSpeed;
            angle %= 360f;

            // Конвертируем угол в радианы
            float radians = angle * Mathf.Deg2Rad;

            // Рассчитываем позицию питомца на окружности
            Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * orbitRadius;
            offset.y = orbitHeight; // Устанавливаем высоту орбиты
            pet.transform.position = playerTransform.position + offset;

            // Питомец всегда смотрит на игрока, но остаётся вертикальным
            Vector3 lookDirection = playerTransform.position - pet.transform.position;
            lookDirection.y = 0; // Убираем влияние вертикальной оси
            pet.transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
