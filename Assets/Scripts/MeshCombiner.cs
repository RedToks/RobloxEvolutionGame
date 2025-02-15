using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    [Tooltip("Объединять ли меши, если у них разные материалы?")]
    public bool combineWithDifferentMaterials = false;

    [Tooltip("Оставить ли исходные объекты после объединения?")]
    public bool keepOriginalObjects = false;

    [Tooltip("Удалить ли коллайдеры у исходных объектов после объединения?")]
    public bool removeColliders = true;

    private void Awake()
    {
        CombineMeshes();
    }
    public void CombineMeshes()
    {
        // Получаем все MeshFilter в дочерних объектах
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0)
        {
            Debug.LogWarning("Нет объектов для объединения.");
            return;
        }

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Material> materials = new List<Material>();
        Dictionary<Material, List<CombineInstance>> materialToMesh = new Dictionary<Material, List<CombineInstance>>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter == null || meshFilter.sharedMesh == null) continue;

            Renderer renderer = meshFilter.GetComponent<Renderer>();
            if (renderer == null || renderer.sharedMaterial == null) continue;

            Material material = renderer.sharedMaterial;

            CombineInstance combineInstance = new CombineInstance
            {
                mesh = meshFilter.sharedMesh,
                transform = meshFilter.transform.localToWorldMatrix
            };

            if (!combineWithDifferentMaterials)
            {
                if (!materialToMesh.ContainsKey(material))
                {
                    materialToMesh[material] = new List<CombineInstance>();
                }
                materialToMesh[material].Add(combineInstance);
            }
            else
            {
                combineInstances.Add(combineInstance);
                if (!materials.Contains(material))
                {
                    materials.Add(material);
                }
            }

            if (!keepOriginalObjects)
            {
                meshFilter.gameObject.SetActive(false);
            }

            if (removeColliders)
            {
                Collider collider = meshFilter.GetComponent<Collider>();
                if (collider != null)
                {
                    Destroy(collider);
                }
            }
        }

        if (!combineWithDifferentMaterials)
        {
            foreach (var kvp in materialToMesh)
            {
                CreateCombinedMesh(kvp.Value, kvp.Key);
            }
        }
        else
        {
            CreateCombinedMesh(combineInstances, materials.Count > 0 ? materials[0] : null);
        }

        Debug.Log("Объединение завершено!");
    }

    private void CreateCombinedMesh(List<CombineInstance> combineInstances, Material material)
    {
        GameObject combinedObject = new GameObject("CombinedMesh");
        combinedObject.transform.position = Vector3.zero;
        combinedObject.transform.rotation = Quaternion.identity;

        Mesh combinedMesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        combinedMeshFilter.mesh = combinedMesh;

        MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();
        combinedMeshRenderer.material = material;

        MeshCollider meshCollider = combinedObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = combinedMesh;
        meshCollider.convex = true;
    }
}
