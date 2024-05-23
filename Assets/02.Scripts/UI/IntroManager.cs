using System.Collections;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    private bool isSpace;
    private bool isLoading = true;

    void Start()
    {
        InitUI();
    }

    void FixedUpdate()
    {
        if (isSpace==false && isLoading==false && Input.GetKeyDown(KeyCode.Space))
        {           
            StartSpace();
        }

        #if UNITY_ANDROID
            if (isSpace == false && isLoading == false && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                StartSpace();
            }
        #endif
    }

    // 처음 인트로 들어갈때 세팅
    public void InitUI()
    {
        if (UIManager.Instance.first == true)
        {
            // First Intro UI
            UIManager.Instance.loadingUI.SetActive(true);
            UIManager.Instance.spaceToStart.SetActive(true);

            // VAN 
            UIManager.Instance.ingamePlayerUI.SetActive(false);
            UIManager.Instance.buttonUI.SetActive(false);
            UIManager.Instance.shutter.SetActive(true);

            // UnderBar UI
            //UIManager.Instance.underBarStop.SetActive(false);
            //UIManager.Instance.underBarCancle.SetActive(false);

            StartCoroutine("IntroSetting");
        }
    }


    // 게임을 시작하려면 스페이스바
    public void StartSpace()
    {
        UIManager.Instance.first = false;
        isSpace = true;

        SoundManager.Instance.Load();
        SoundManager.Instance.StartPlay();

        // VAN 
        UIManager.Instance.ingamePlayerUI.SetActive(true);
        UIManager.Instance.buttonUI.SetActive(true);
        UIManager.Instance.shutterAnim.SetTrigger("ShutterOn");
        SoundManager.Instance.VanShutter();
        UIManager.Instance.shutterCamera.Priority = 9;
        StartCoroutine("ShutterOut");

        // First Intro UI
        UIManager.Instance.loadingUI.SetActive(false);
        UIManager.Instance.spaceToStart.SetActive(false);

        // UnderBar UI
        // UIManager.Instance.UnderBarStop.SetActive(true);
    }

    // 셔터 비활성화
    IEnumerator ShutterOut()
    {
        yield return new WaitForSeconds(2f);

        UIManager.Instance.shutter.SetActive(false);
    }

    // 처음 인트로 로딩 영상
    IEnumerator IntroSetting()
    {
        yield return new WaitForSeconds(18f);

        isLoading = false;
    }
}
