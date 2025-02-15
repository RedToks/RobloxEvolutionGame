using UnityEngine;

public class LookAtPlayerFull : MonoBehaviour
{
    [SerializeField] private Transform player; // ������ �� ������ ������
    [SerializeField] private float rotationSpeed = 5f; // �������� ��������
    [SerializeField] private Vector3 rotationOffset = Vector3.zero; // �������������� ����� �� ���� (��������, ���� ������ ��������� �����������)

    private void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player transform not assigned.");
            return;
        }

        // ������������ ����������� � ������
        Vector3 directionToPlayer = player.position - transform.position;

        // ���������, ��� ���� ����������� (����� ������ �� ����� ��� ���������� �������)
        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            // ������������ ������� �������
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // ��������� �������������� �����
            targetRotation *= Quaternion.Euler(rotationOffset);

            // ������ ������������ ������ � ������
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
