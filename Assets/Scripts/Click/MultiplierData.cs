using UnityEngine;

[System.Serializable]
public class MultiplierData
{
    public Sprite icon;         // ������ ���������
    public float multiplier;    // �������� ��������� (��������, x200)
    [HideInInspector]
    public long unlockCost; // ������� long
    [SerializeField]
    private string unlockCostString = "0"; // ������������ ������

    public void ParseUnlockCost()
    {
        if (!long.TryParse(unlockCostString, out unlockCost))
        {
            Debug.LogError($"������: ������������ ����� � unlockCostString ({unlockCostString})");
        }
    }
}
