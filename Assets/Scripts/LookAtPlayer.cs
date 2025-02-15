using UnityEngine;

public class LookAtPlayerFull : MonoBehaviour
{
    [SerializeField] private Transform player; // Ссылка на объект игрока
    [SerializeField] private float rotationSpeed = 5f; // Скорость поворота
    [SerializeField] private Vector3 rotationOffset = Vector3.zero; // Дополнительный сдвиг по осям (например, если объект направлен неправильно)

    private void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player transform not assigned.");
            return;
        }

        // Рассчитываем направление к игроку
        Vector3 directionToPlayer = player.position - transform.position;

        // Проверяем, что есть направление (чтобы объект не завис при совпадении позиций)
        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            // Рассчитываем целевую ротацию
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Применяем дополнительный сдвиг
            targetRotation *= Quaternion.Euler(rotationOffset);

            // Плавно поворачиваем объект к игроку
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
