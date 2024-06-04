using EnumTypes;
using UnityEngine;
using UnityEngine.UI;

public class PressSkip : MonoBehaviour
{
    public Image fillImage; // fillAmount를 조절할 이미지
    public float fillSpeed = 0.1f; // 증가 및 감소 속도

    private const float MAX_FILL = 1f;
    private const float MIN_FILL = 0f;

    private void Start()
    {
        fillImage.fillAmount = MIN_FILL;
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            HandleInput(Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved));
        }
        else
        {
            HandleInput(Input.GetKey(KeyCode.Space));
        }
    }

    private void HandleInput(bool isPressed)
    {
        if (isPressed)
        {
            // 스페이스바나 터치를 누르고 있을 때 fillAmount 증가
            fillImage.fillAmount += fillSpeed * Time.deltaTime;

            if (fillImage.fillAmount >= MAX_FILL)
            {
                TriggerSkipActions();
                fillImage.fillAmount = MIN_FILL;
            }
        }
        else
        {
            // 스페이스바나 터치를 누르고 있지 않을 때 fillAmount 감소
            fillImage.fillAmount -= fillSpeed * Time.deltaTime;
        }

        // fillAmount 값 제한 (0과 1 사이로 클램핑)
        fillImage.fillAmount = Mathf.Clamp(fillImage.fillAmount, MIN_FILL, MAX_FILL);
    }

    private void TriggerSkipActions()
    {
        UIManager.Instance.RecipeUIOff();
        SoundManager.Instance.RecipeUIPopOut();
        if (UIManager.Instance.mapType == MapType.StageMine)
        {
            //EventManager<SoundEvents>.TriggerEvent(SoundEvents.MineBgmPlay);
        }
        // UI 비활성화 및 게임 시작
    }
}
