using UnityEngine;
using UnityEngine.UI;

public class PressSkip : MonoBehaviour
{
    public Image fillImage; // fillAmount를 조절할 이미지
    public float fillSpeed = 0.1f; // 증가 및 감소 속도

    private void Start()
    {
        fillImage.fillAmount = 0f;
    }

    private void Update()
    {
        Skip();

        #if UNITY_ANDROID
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            UIManager.Instance.RecipeUIOff();
            SoundManager.Instance.RecipeUIPopOut();
            fillImage.fillAmount = 0;
            // UI 비활성화 및 게임 시작
        }
        #endif
    }

    private void Skip()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // 스페이스바를 누르고 있을 때 fillAmount 증가
            fillImage.fillAmount += fillSpeed * Time.deltaTime;

            if (fillImage.fillAmount >= 1f)
            {
                UIManager.Instance.RecipeUIOff();
                SoundManager.Instance.RecipeUIPopOut();
                fillImage.fillAmount = 0;
                // UI 비활성화 및 게임 시작
            }
        }
        else
        {
            // 스페이스바를 누르고 있지 않을 때 fillAmount 감소
            fillImage.fillAmount -= fillSpeed * Time.deltaTime;
        }

        // fillAmount 값 제한 (0과 1 사이로 클램핑)
        fillImage.fillAmount = Mathf.Clamp(fillImage.fillAmount, 0f, 1f);
    }

    public void MobileSkip()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            UIManager.Instance.RecipeUIOff();
            SoundManager.Instance.RecipeUIPopOut();
            fillImage.fillAmount = 0;
            // UI 비활성화 및 게임 시작
        }
    }

}
