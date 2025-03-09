using UnityEngine;
using System.Collections.Generic;

public class PetOrbitManager : MonoBehaviour
{
    public Transform playerTransform; // �����
    public float orbitRadius = 3f; // ������ ��������
    public float rotationSpeed = 50f; // �������� ��������
    public float orbitHeight = 1f; // ������ �������� ��������

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
            UpdatePetPositions(); // ��������� ������� ����� ���������� �������
        }
    }

    public void RemovePet(GameObject pet)
    {
        if (activePets.Contains(pet))
        {
            activePets.Remove(pet);
            Destroy(pet); // ������� ������� �� �����
            UpdatePetPositions(); // ��������� ������� ���������� ��������
        }
    }

    private void UpdatePetPositions()
    {
        int petCount = activePets.Count;
        if (petCount == 0) return;

        float angleStep = 360f / petCount;
        Vector3 playerForward = playerTransform.forward; // ����������� ������ �� ���������

        // ���� � ������ ���� ��������, ���������� ����������� ��������
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

            // ������������ ������� ������� ������������ ������
            Vector3 offset = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians)) * orbitRadius;
            offset.y = orbitHeight;
            pet.transform.position = playerTransform.position + offset;

            // ������� ������� � ������� �������� ���������
            if (playerForward.magnitude > 0.1f) // ���������, ��� ����� ��������
            {
                pet.transform.rotation = Quaternion.LookRotation(playerForward);
            }
        }
    }

}
