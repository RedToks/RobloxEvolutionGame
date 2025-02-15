using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform target; // Цель, вокруг которой вращается UI (например, яйцо)
    public float distance = 2f; // Расстояние UI от цели
    public Vector3 offset = Vector3.up; // Смещение UI (по высоте)

    private Transform playerCamera;

    private void Start()
    {
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (playerCamera != null && target != null)
        {
            // Получаем направление от цели к камере
            Vector3 direction = (playerCamera.position - target.position).normalized;

            // Устанавливаем новую позицию UI вокруг цели (по сфере)
            transform.position = target.position + direction * distance + offset;

            // Разворачиваем UI к камере, но сохраняем корректное направление
            transform.LookAt(2 * transform.position - playerCamera.position);
        }
    }
}
