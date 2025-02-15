using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform target; // ����, ������ ������� ��������� UI (��������, ����)
    public float distance = 2f; // ���������� UI �� ����
    public Vector3 offset = Vector3.up; // �������� UI (�� ������)

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
            // �������� ����������� �� ���� � ������
            Vector3 direction = (playerCamera.position - target.position).normalized;

            // ������������� ����� ������� UI ������ ���� (�� �����)
            transform.position = target.position + direction * distance + offset;

            // ������������� UI � ������, �� ��������� ���������� �����������
            transform.LookAt(2 * transform.position - playerCamera.position);
        }
    }
}
