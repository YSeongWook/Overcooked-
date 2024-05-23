using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    private MeshCombiner meshCombiner;

    private void OnEnable()
    {
        meshCombiner = (MeshCombiner)target;
        MeshCombiner.UpdateButtonLabel += OnUpdateButtonLabel; // 버튼 레이블 업데이트 이벤트 구독
    }

    private void OnDisable()
    {
        MeshCombiner.UpdateButtonLabel -= OnUpdateButtonLabel; // 버튼 레이블 업데이트 이벤트 구독 해지
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 기본 인스펙터 드로잉

        EditorGUILayout.Space(); // 공간 추가

        EditorGUILayout.BeginHorizontal(); // 수평 레이아웃 시작

        EditorGUI.BeginDisabledGroup(meshCombiner.objectsToMerge.Count < 1); // 병합할 오브젝트가 1개 미만이면 버튼 비활성화

        // Clear 버튼
        if (GUILayout.Button("Clear"))
        {
            meshCombiner.ClearObjects();
            Debug.Log("병합할 오브젝트 리스트가 클리어되었습니다.");
        }

        EditorGUI.EndDisabledGroup();

        // Combine 버튼
        if (GUILayout.Button("Combine"))
        {
            if (CanCombine())
            {
                meshCombiner.MergeOrClearMeshes();
                Debug.Log("오브젝트가 성공적으로 병합되었습니다.");
            }
        }

        EditorGUILayout.EndHorizontal(); // 수평 레이아웃 종료
    }

    // Combine이 가능한지 여부를 결정하는 메서드
    private bool CanCombine()
    {
        if (meshCombiner.objectsToMerge.Count == 0)
        {
            Debug.LogWarning("병합할 오브젝트가 없습니다.");
            return false;
        }

        if (meshCombiner.objectsToMerge.Count == 1)
        {
            Debug.LogWarning("1개의 오브젝트는 병합할 수 없습니다.");
            return false;
        }

        return true;
    }

    // 버튼 레이블 업데이트 메서드
    private void OnUpdateButtonLabel(bool isCombined)
    {
        Repaint(); // 인스펙터를 다시 그리도록 요청
        SceneView.RepaintAll(); // 씬 뷰를 다시 그리도록 요청
    }
}
