using UnityEngine;

public class TeleportToSpawn : MonoBehaviour
{
    [Header("��������� ������������")]
    [SerializeField] private Transform spawnPoint; // ����� ������
    [SerializeField] private float minYPosition = -10f; // ����������� ������, ����� ������� ����� ���������������

    private Transform playerTransform;

    private void Start()
    {
        // ���� ������ �� ���� "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("����� � ����� 'Player' �� ������! ���������, ��� � ������ ���������� ���������� ���.");
        }

        // ���������, ��������� �� ����� ������
        if (spawnPoint == null)
        {
            Debug.LogError("����� ������ �� ���������! ��������� � � ����������.");
        }
    }

    private void Update()
    {
        // ��������� ������� ������
        if (playerTransform != null && playerTransform.position.y < minYPosition)
        {
            TeleportToSpawnPoint();
        }
    }

    public void TeleportToSpawnPoint()
    {
        if (spawnPoint != null)
        {
            playerTransform.position = spawnPoint.position;
            playerTransform.rotation = spawnPoint.rotation; // ��������� ���������� ������
            Debug.Log("����� �������������� �� �����!");
        }
    }
}
