using UnityEngine;
using System.Collections;

public class MoveAlongSplineWithRotation : MonoBehaviour
{
    [SerializeField] private Transform[] controlPoints; // ����� ����
    [SerializeField] private float moveSpeed = 5f; // �������� ��������
    [SerializeField] private float rotationSpeed = 5f; // �������� ��������
    [SerializeField] private bool loop = true; // ������������ ��������
    [SerializeField] private bool waitAtPoints = false; // ��������� �������� � �����
    [SerializeField] private float waitTime = 1f; // ����� �������� � ������ �����

    private int currentPointIndex = 0;
    private Quaternion initialRotation; // ��������� ���������� �������
    private bool isWaiting = false; // ���� ��� ��������

    private void Start()
    {
        if (controlPoints.Length < 2)
        {
            Debug.LogWarning("����� ������� ��� ����� ��� �������� ����.");
            return;
        }

        // ��������� ��������� ���������� �������
        initialRotation = transform.rotation;

        // ������������� ��������� ������� �������
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

        // ������� �����
        Transform targetPoint = controlPoints[currentPointIndex];

        // ������� ������ � ������� �����
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // ������������ ������ � ������� ������� �����
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            // ������������ ������� ��� �������� � ������� ��������� �����
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // ��������� ������ ������� �� ��� Y, �������� ����������� �������� �� X � Z
            Vector3 eulerRotation = targetRotation.eulerAngles;
            eulerRotation.x = initialRotation.eulerAngles.x; // ��������� ����������� X
            eulerRotation.z = initialRotation.eulerAngles.z; // ��������� ����������� Z

            // ������� ������� � ������ ��������
            Quaternion finalRotation = Quaternion.Euler(eulerRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationSpeed * Time.deltaTime);
        }

        // ���������, �������� �� ������� �����
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex++;

            // ���� �������� ��������� �����
            if (currentPointIndex >= controlPoints.Length)
            {
                if (loop)
                {
                    currentPointIndex = 0; // �������� ������
                }
                else
                {
                    enabled = false; // ������������� ��������
                }
            }

            // ���� ����� ������� � �����, ��������� ��������
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
