using UnityEngine;

public class TeleportToSpawn : MonoBehaviour
{
    [Header("Настройки телепортации")]
    [SerializeField] private Transform spawnPoint; // Точка спавна
    [SerializeField] private float minYPosition = -10f; // Минимальная высота, после которой игрок телепортируется

    private Transform playerTransform;

    private void Start()
    {
        // Ищем игрока по тегу "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Игрок с тегом 'Player' не найден! Убедитесь, что у игрока установлен правильный тег.");
        }

        // Проверяем, назначена ли точка спавна
        if (spawnPoint == null)
        {
            Debug.LogError("Точка спавна не назначена! Назначьте её в инспекторе.");
        }
    }

    private void Update()
    {
        // Проверяем позицию игрока
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
            playerTransform.rotation = spawnPoint.rotation; // Сохраняем ориентацию спавна
            Debug.Log("Игрок телепортирован на спавн!");
        }
    }
}
