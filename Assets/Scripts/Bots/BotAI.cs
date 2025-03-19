using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

public class BotAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;

    [Header("��������� ������������")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 5f;
    public float moveRadius = 10f;

    [Header("������� ������")]
    public SkinManager skinManager; // �������� ������
    public SkinnedMeshRenderer botRenderer; // ������ ����

    [Header("������� �����")]
    public Transform hairContainer; // ������, ���������� ��� ������

    [Header("��������� ������")]
    public float minJumpInterval = 3f;
    public float maxJumpInterval = 8f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.2f; // ��������� ��� �������� �����

    private Rigidbody rb;
    private bool isJumping = false; // ����, ����� �� ������� �� ����� ������

    private void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        AssignRandomSkin();
        AssignRandomHair();

        if (!agent.isOnNavMesh) // ���� ��� �� �� NavMesh, �������� ��������� ���
        {
            TryPlaceOnNavMesh();
        }

        StartCoroutine(BotBehavior());
        StartCoroutine(RandomJumpBehavior());
    }

    private void TryPlaceOnNavMesh()
    {
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            transform.position = hit.position; // ���������� ���� �� ��������� ����� NavMesh
            agent.Warp(hit.position); // ���������� ����� ����� � ��� �������
        }
        else
        {
            Debug.LogError($"��� {gameObject.name} �� ���� ����� ��������� ����� �� NavMesh!");
        }
    }

    private IEnumerator BotBehavior()
    {
        while (true)
        {
            if (agent.isActiveAndEnabled && agent.isOnNavMesh) // ���������, ������� �� ����� � ��������� �� �� �� NavMesh
            {
                Vector3 randomPoint = GetRandomNavMeshPoint();
                agent.SetDestination(randomPoint);
                animator.SetBool("run", true);

                yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
                agent.ResetPath();
                animator.SetBool("run", false);
            }

            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator RandomJumpBehavior()
    {
        while (true)
        {
            float waitTime = Random.Range(minJumpInterval, maxJumpInterval);
            yield return new WaitForSeconds(waitTime);

            if (!isJumping && agent.velocity.magnitude < 0.1f && IsGrounded())
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        if (rb == null) return;

        isJumping = true;
        animator.SetBool("jump", true); // �������� �������� ������

        agent.enabled = false;  // ��������� NavMeshAgent
        rb.isKinematic = false; // �������� ������
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        StartCoroutine(CheckLanding());
    }

    private IEnumerator CheckLanding()
    {
        yield return new WaitForSeconds(0.2f); // ���� �������, ����� �� ��������� �����

        while (!IsGrounded())
        {
            yield return null; // ����, ���� ��� � �������
        }

        rb.isKinematic = true;  // ����� �������� isKinematic
        agent.enabled = true;    // �������� NavMeshAgent
        isJumping = false;       // ���������� ���� ������
        animator.SetBool("jump", false); // ��������� �������� ������
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    private Vector3 GetRandomNavMeshPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, moveRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    private void AssignRandomSkin()
    {
        if (skinManager != null && botRenderer != null && skinManager.skins.Count > 0)
        {
            SkinData randomSkin = skinManager.skins[Random.Range(0, skinManager.skins.Count)];
            botRenderer.material.mainTexture = randomSkin.textureSprite.texture;
        }
    }

    private void AssignRandomHair()
    {
        if (hairContainer == null) return;

        int childCount = hairContainer.childCount;
        if (childCount == 0) return;

        for (int i = 0; i < childCount; i++)
        {
            hairContainer.GetChild(i).gameObject.SetActive(false);
        }

        int randomIndex = Random.Range(0, childCount);
        GameObject selectedHair = hairContainer.GetChild(randomIndex).gameObject;
        selectedHair.SetActive(true);

        ChangeHairColor(selectedHair);
    }

    private void ChangeHairColor(GameObject hairObject)
    {
        MeshRenderer hairRenderer = hairObject.GetComponentInChildren<MeshRenderer>();

        if (hairRenderer != null && hairRenderer.material != null)
        {
            Color randomColor = new Color(Random.value, Random.value, Random.value);
            hairRenderer.material.color = randomColor;
        }
    }
}
