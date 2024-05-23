using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger_Net : MonoBehaviour
{

    private void Awake()
    {
        ChangeScene("Intro_Net");
    }

    // 버튼 클릭 시 호출되는 메서드
    public void ChangeScene(string sceneName)
    {
        // SceneManager를 사용하여 씬을 변경합니다.
        SceneManager.LoadScene(sceneName);
    }
}
