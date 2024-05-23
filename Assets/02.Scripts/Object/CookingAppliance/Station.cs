using UnityEngine;

public class Station : MonoBehaviour
{
    [SerializeField]
    private float xSpeed = 5.5f; // x축 이동 속도 설정 (텍스처 오프셋 이동 속도)
    private float xOffset; // x축 오프셋 저장할 변수

    [SerializeField]
    private float ySpeed = 5.5f; // y축 이동 속도 설정 (텍스처 오프셋 이동 속도)
    private float yOffset; // y축 오프셋 저장할 변수

    private Renderer meshRenderer; // Renderer 컴포넌트 캐싱

    private void Awake()
    {
        // Renderer 컴포넌트 캐싱
        meshRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // x축 및 y축 오프셋 업데이트
        UpdateOffsets();

        // 텍스처 오프셋을 설정
        SetTextureOffset(new Vector2(xOffset, yOffset));
    }

    // 오프셋 업데이트 메서드
    private void UpdateOffsets()
    {
        xOffset -= (Time.deltaTime * xSpeed) / 10f;
        yOffset -= (Time.deltaTime * ySpeed) / 10f;
    }

    // 텍스처 오프셋 설정 메서드
    private void SetTextureOffset(Vector2 offset)
    {
        // 세 번째 소재(material)의 주 텍스처 오프셋 설정
        if (meshRenderer.materials.Length > 3)
        {
            meshRenderer.materials[3].mainTextureOffset = offset;
            meshRenderer.materials[3].SetTextureOffset("_MainTex", offset);
        }
        else
        {
            Debug.LogWarning("Not enough materials on the Renderer component.");
        }
    }
}
