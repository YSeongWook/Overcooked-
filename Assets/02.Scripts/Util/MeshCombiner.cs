using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshCombiner : MonoBehaviour
{
    public List<GameObject> objectsToMerge = new List<GameObject>(); // 병합할 오브젝트 리스트
    public bool addMeshCollider = false; // 병합된 메쉬에 메쉬 콜라이더를 추가할지 결정하는 변수
    public bool deactivateObjectsAfterMerge = true; // 병합 후 병합된 오브젝트들을 비활성화할지 결정하는 변수
    public bool destroyObjectsAfterMerge = false; // 병합 후 병합된 오브젝트들을 삭제할지 결정하는 변수
    public bool makeStatic = true; // 병합된 오브젝트를 정적 오브젝트로 만들지 결정하는 변수

    public bool isCombined = false; // 현재 병합 상태를 나타내는 변수

    // 병합 또는 클리어 동작을 수행하는 메서드
    public void MergeOrClearMeshes()
    {
        InitializeObjects(); // 초기화 메서드 호출

        // 병합할 오브젝트가 1개 이하인 경우 경고 메시지 출력 및 상태 업데이트
        if (objectsToMerge.Count <= 1)
        {
            Debug.LogWarning(objectsToMerge.Count == 0 ? "병합할 오브젝트가 없습니다." : "1개의 오브젝트는 병합할 수 없습니다.");
            UpdateStateAndButtonLabel(false);
            return;
        }

        MergeMeshes();
        ClearObjects(); // 병합 후 오브젝트 리스트를 클리어
        UpdateStateAndButtonLabel(true);
        Debug.Log("오브젝트가 성공적으로 병합되었습니다.");
    }

    // 오브젝트 리스트를 초기화하는 메서드
    public void InitializeObjects()
    {
        isCombined = false;
    }

    // 병합할 오브젝트 리스트를 비우는 메서드
    public void ClearObjects()
    {
        objectsToMerge.Clear();
        isCombined = false;
        UpdateStateAndButtonLabel(false);
    }

    // 병합 상태와 버튼 레이블을 업데이트하는 메서드
    private void UpdateStateAndButtonLabel(bool combinedState)
    {
        isCombined = combinedState;
        UpdateButtonLabel?.Invoke(isCombined);
        isCombined = false;
    }

    // 버튼 레이블 업데이트를 위한 델리게이트 및 이벤트 정의
    public delegate void ButtonLabelUpdate(bool isCombined);
    public static event ButtonLabelUpdate UpdateButtonLabel;

    // 메쉬를 병합하는 메서드
    private void MergeMeshes()
    {
        var meshFilters = new List<MeshFilter>();

        // 각 오브젝트에서 MeshFilter를 가져옴
        foreach (var obj in objectsToMerge)
        {
            meshFilters.AddRange(obj.GetComponentsInChildren<MeshFilter>());
        }

        // MeshFilter가 없으면 경고 메시지 출력 및 종료
        if (meshFilters.Count == 0)
        {
            Debug.LogWarning("병합할 오브젝트가 없습니다.");
            return;
        }

        // Material을 기준으로 메쉬를 그룹화
        var materialMeshMap = CreateMaterialMeshMap(meshFilters);

        // 병합된 오브젝트 생성 및 설정
        var combinedObject = CreateCombinedObject(materialMeshMap);

        // 필요한 경우 메쉬 콜라이더 추가
        if (addMeshCollider)
        {
            AddMeshCollider(combinedObject);
        }

        // 필요한 경우 병합된 오브젝트 비활성화
        if (deactivateObjectsAfterMerge)
        {
            DeactivateObjects(objectsToMerge);
        }

        // 필요한 경우 병합된 오브젝트 삭제
        if (destroyObjectsAfterMerge)
        {
            DestroyObjects(objectsToMerge);
        }
    }

    // MeshFilter 리스트에서 Material을 기준으로 메쉬를 그룹화하는 메서드
    private Dictionary<Material, List<CombineInstance>> CreateMaterialMeshMap(List<MeshFilter> meshFilters)
    {
        var materialMeshMap = new Dictionary<Material, List<CombineInstance>>();

        foreach (var meshFilter in meshFilters)
        {
            var meshRenderer = meshFilter.GetComponent<MeshRenderer>();
            if (meshRenderer == null || meshFilter.sharedMesh == null)
                continue;

            var sharedMaterials = meshRenderer.sharedMaterials;
            for (int subMeshIndex = 0; subMeshIndex < meshFilter.sharedMesh.subMeshCount; subMeshIndex++)
            {
                if (subMeshIndex >= sharedMaterials.Length)
                    continue;

                var material = sharedMaterials[subMeshIndex];
                if (material == null)
                    continue;

                if (!materialMeshMap.ContainsKey(material))
                {
                    materialMeshMap[material] = new List<CombineInstance>();
                }

                materialMeshMap[material].Add(new CombineInstance
                {
                    mesh = GetSubMesh(meshFilter.sharedMesh, subMeshIndex),
                    transform = meshFilter.transform.localToWorldMatrix
                });
            }
        }

        return materialMeshMap;
    }

    // 서브 메쉬를 추출하는 메서드
    private Mesh GetSubMesh(Mesh mesh, int index)
    {
        var subMesh = new Mesh
        {
            vertices = mesh.vertices,
            normals = mesh.normals,
            tangents = mesh.tangents,
            uv = mesh.uv,
            uv2 = mesh.uv2,
            uv3 = mesh.uv3,
            uv4 = mesh.uv4,
            colors = mesh.colors
        };
        subMesh.SetTriangles(mesh.GetTriangles(index), 0);
        return subMesh;
    }

    // 병합된 오브젝트를 생성하고 설정하는 메서드
    private GameObject CreateCombinedObject(Dictionary<Material, List<CombineInstance>> materialMeshMap)
    {
        var combinedObject = new GameObject("Combined Mesh");

        // 사용자가 정적 오브젝트로 만들지 선택할 수 있게 설정
        if (makeStatic) combinedObject.isStatic = true;

        foreach (var kvp in materialMeshMap)
        {
            var material = kvp.Key;
            var combineInstances = kvp.Value;

            var combinedMesh = new Mesh();

            // 버텍스 개수에 따라 인덱스 포맷 설정
            int totalVertexCount = combineInstances.Sum(c => c.mesh.vertexCount);
            combinedMesh.indexFormat = totalVertexCount > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16;

            combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

            var child = new GameObject(material.name);
            child.transform.SetParent(combinedObject.transform, false);
            if(makeStatic) child.isStatic = true;

            var meshFilter = child.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = combinedMesh;

            var meshRenderer = child.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;
        }

        return combinedObject;
    }

    // 병합된 오브젝트에 메쉬 콜라이더를 추가하는 메서드
    private void AddMeshCollider(GameObject combinedObject)
    {
        var meshColliders = combinedObject.GetComponentsInChildren<MeshFilter>()
            .Select(mf => mf.gameObject.AddComponent<MeshCollider>());

        foreach (var meshCollider in meshColliders)
        {
            meshCollider.sharedMesh = meshCollider.GetComponent<MeshFilter>().sharedMesh;
        }
    }

    // 오브젝트 리스트의 오브젝트들을 비활성화하는 메서드
    private void DeactivateObjects(List<GameObject> objects)
    {
        foreach (var obj in objects)
        {
            obj.SetActive(false);
        }
    }

    // 오브젝트 리스트의 오브젝트들을 삭제하는 메서드
    private void DestroyObjects(List<GameObject> objects)
    {
        foreach (var obj in objects)
        {
            Destroy(obj);
        }
    }
}
