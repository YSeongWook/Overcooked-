using UnityEngine;

public class ScrollMT : MonoBehaviour
{
    public MeshRenderer meshRenderer; // 매쉬 랜더러 참조
    public int materialIndex = 0; // 사용할 마테리얼의 인덱스
    public float speed = 0.5f; // 오프셋 변화 속도

    [Header("Y")]
    public bool YPlus; // Y 축 이동 방향 플래그 (위쪽)
    public bool y; // Y 축 이동 활성화 플래그
    public bool yLimit; // Y 축 제한 플래그
    public float startYOffset = 0.0f; // 시작 Y 오프셋
    public float endYOffset = -0.0f; // 도달해야 할 Y 오프셋

    [Header("X")]
    public bool XPlus; // X 축 이동 방향 플래그 (오른쪽)
    public bool x; // X 축 이동 활성화 플래그
    public bool xLimit; // X 축 제한 플래그
    public float startXOffset = 0.0f; // 시작 X 오프셋
    public float endXOffset = -0.0f; // 도달해야 할 X 오프셋

    private Vector2 currentOffset;
    private Material originalMaterial;

    private void Start()
    {
        // MeshRenderer를 컴포넌트에서 가져오기
        meshRenderer = GetComponent<MeshRenderer>();

        // 매테리얼의 인덱스가 유효한지 확인
        if (meshRenderer.sharedMaterials.Length > materialIndex)
        {
            originalMaterial = meshRenderer.sharedMaterials[materialIndex];
            originalMaterial.mainTextureOffset = new Vector2(0f, 0);
        }
    }

    void Update()
    {
        // 매쉬 랜더러와 마테리얼이 제대로 설정되었는지 확인
        if (meshRenderer == null || originalMaterial == null)
        {
            Debug.LogWarning("Mesh Renderer or Material not assigned properly.");
            return;
        }

        // 현재 오프셋
        currentOffset = originalMaterial.mainTextureOffset;

        // Y 축 이동 업데이트
        if (y)
        {
            currentOffset.y = UpdateOffset(currentOffset.y, startYOffset, endYOffset, YPlus, yLimit);
        }

        // X 축 이동 업데이트
        if (x)
        {
            currentOffset.x = UpdateOffset(currentOffset.x, startXOffset, endXOffset, XPlus, xLimit);
        }

        // 변경된 오프셋을 마테리얼에 적용
        originalMaterial.mainTextureOffset = currentOffset;
    }

    private void OnDisable()
    {
        // 스크립트가 비활성화될 때 오프셋 초기화
        if (originalMaterial != null)
        {
            originalMaterial.mainTextureOffset = new Vector2(0f, 0f);
        }
    }

    // 오프셋을 업데이트하는 메서드
    private float UpdateOffset(float current, float start, float end, bool isPositive, bool isLimited)
    {
        // 이동 방향에 따른 오프셋 변경
        current += (isPositive ? 1 : -1) * speed * Time.deltaTime;

        // 제한이 있는 경우, 오프셋이 한계를 넘어가면 다시 시작 위치로 설정
        if (isLimited)
        {
            if ((isPositive && current >= end) || (!isPositive && current <= end))
            {
                return start;
            }
        }

        return current;
    }
}
