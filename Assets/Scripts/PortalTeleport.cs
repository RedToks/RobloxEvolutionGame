using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;

public class PortalTeleport : MonoBehaviour
{
    [Header("��������� ������������")]
    public Transform targetPosition; // ���� ��������������� ������
    public long requiredBrainCurrency = 0; // ����������� ���� BrainCurrency

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ExampleCharacterController player) && BrainCurrency.Instance.brainCurrency >= requiredBrainCurrency)
        {           
            TeleportPlayer(other.gameObject);
        }
    }

    private void TeleportPlayer(GameObject player)
    {
        if (targetPosition != null)
        {
            KinematicCharacterMotor characterMotor = player.GetComponent<KinematicCharacterMotor>();
            characterMotor.enabled = false;
            player.transform.position = targetPosition.position;
            characterMotor.enabled = true;
            Debug.Log("����� ��������������!");
        }
        else
        {
            Debug.LogError("�� ������� ���� ������������!");
        }
    }
}
