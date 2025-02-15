using UnityEngine;
using System.Collections;

public class MoveAlongSplineWithRotation : MonoBehaviour
{
    [SerializeField] private Transform[] controlPoints; // Точки пути
    [SerializeField] private float moveSpeed = 5f; // Скорость движения
    [SerializeField] private float rotationSpeed = 5f; // Скорость поворота
    [SerializeField] private bool loop = true; // Зацикливание движения
    [SerializeField] private bool waitAtPoints = false; // Настройка ожидания у точек
    [SerializeField] private float waitTime = 1f; // Время ожидания у каждой точки

    private int currentPointIndex = 0;
    private Quaternion initialRotation; // Начальная ориентация объекта
    private bool isWaiting = false; // Флаг для ожидания

    private void Start()
    {
        if (controlPoints.Length < 2)
        {
            Debug.LogWarning("Нужно минимум две точки для создания пути.");
            return;
        }

        // Сохраняем начальную ориентацию объекта
        initialRotation = transform.rotation;

        // Устанавливаем начальную позицию объекта
        transform.position = controlPoints[0].position;
    }

    private void Update()
    {
        if (!isWaiting)
        {
            MoveAndRotate();
        }
    }

    private void MoveAndRotate()
    {
        if (controlPoints.Length < 2) return;

        // Целевая точка
        Transform targetPoint = controlPoints[currentPointIndex];

        // Двигаем объект к целевой точке
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // Поворачиваем объект в сторону целевой точки
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            // Рассчитываем поворот для движения в сторону следующей точки
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Применяем только поворот по оси Y, сохраняя изначальные вращения по X и Z
            Vector3 eulerRotation = targetRotation.eulerAngles;
            eulerRotation.x = initialRotation.eulerAngles.x; // Сохраняем изначальный X
            eulerRotation.z = initialRotation.eulerAngles.z; // Сохраняем изначальный Z

            // Плавный переход к новому повороту
            Quaternion finalRotation = Quaternion.Euler(eulerRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationSpeed * Time.deltaTime);
        }

        // Проверяем, достигли ли текущей точки
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex++;

            // Если достигли последней точки
            if (currentPointIndex >= controlPoints.Length)
            {
                if (loop)
                {
                    currentPointIndex = 0; // Начинаем заново
                }
                else
                {
                    enabled = false; // Останавливаем движение
                }
            }

            // Если нужно ожидать у точки, запускаем ожидание
            if (waitAtPoints)
            {
                StartCoroutine(WaitAtPoint());
            }
        }
    }

    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
    }
}
