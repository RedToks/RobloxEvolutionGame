using System.Collections;
using UnityEngine;

public class SpriteLookAtPlayerOptimized : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float updateInterval = 0.1f;

    private Renderer spriteRenderer;
    private bool isVisible = true;

    void Start()
    {
        spriteRenderer = GetComponent<Renderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Renderer не найден! Проверьте, чтобы объект имел SpriteRenderer или MeshRenderer.");
        }

        StartCoroutine(UpdateRotationCoroutine());
    }

    private void Update()
    {
        if (spriteRenderer != null)
        {
            isVisible = spriteRenderer.isVisible;
        }
    }

    private IEnumerator UpdateRotationCoroutine()
    {
        while (true)
        {
            if (player != null && isVisible)
            {
                float distance = Vector3.Distance(transform.position, player.position);
                if (distance <= maxDistance)
                {
                    Vector3 direction = player.position - transform.position;
                    direction.y = 0;

                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }
}
