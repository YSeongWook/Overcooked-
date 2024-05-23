using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EnumTypes;
using EventLibrary;
using System.Collections.Generic;
using System.Linq;



public class UIManager : Singleton<UIManager>
{
    [Header("Camera")]
    public CinemachineVirtualCamera vanCamera;
    public CinemachineVirtualCamera shutterCamera;

    [Header("Van")]
    public GameObject buttonUI;
    public GameObject ingamePlayerUI;
    public GameObject shutter;
    public Animator shutterAnim;

    [Header("FirstIntro")]
    public GameObject loadingUI;
    public GameObject spaceToStart;

    [Header("OptionUI")]
    public GameObject optionSettingUI;
    public GameObject escButton;

    [Header("StopUI")]
    public GameObject stopUI;

    [Header("UnderBarUI")]
    public GameObject underBarCancle;
    public GameObject underBarStop;

    [Header("MaskTransitionUI")]
    public GameObject broccoliMask; // 크기를 변경할 RectTransform
    public GameObject pineappleMask; // 크기를 변경할 RectTransform

    [Header("Battle")]
    public GameObject battleRoomUI;
    public GameObject battleUI;
    public GameObject battleResultUI;

    [Header("ExitLobbyUI")]
    public GameObject exitLobbyUI;

    [Header("Loading")]
    public GameObject[] loadingFoodArr;
    public GameObject loadingKeyUI;
    public Image loadingKeyBar;
    public GameObject loadingMapUI;
    public Image loadingMapBar;
    public GameObject[] mapImage;
    public GameObject[] mapText;

    [Header("BusMap")]
    public GameObject busMapEscUI;
    public GameObject busTopUI;

    [Header("StageMap")]
    public GameObject stageMapEscUI;
    public GameObject[] stageEscMapName;

    [Header("RecipeUI")]
    public GameObject recipeUI;
    public GameObject [] recipeArr;

    [Header("Resolution")]  
    public TextMeshProUGUI resolutionText;
    public GameObject fullScreenButton;
    public GameObject fullScreenCheck;
    public bool windowScreen;
    private bool settingWindowScreen;
    public int resolutionArrNum;
    private int settingResolutionArrNum;
    public string[] resolutionTextArr 
        = new string[] { "1280 x 720", "1680 x 1050", "1920 x 1080", "2560 x 1440", "3840 x 2160" };


    public bool first = true;

    public SceneType sceneType;
    public MapType mapType = MapType.None;

    public void JsonUILoad()
    {
        settingWindowScreen = LoadData.Instance.optionData.saveWindowMode;
        windowScreen = LoadData.Instance.optionData.saveWindowMode;
        resolutionArrNum = LoadData.Instance.optionData.saveResolutionNum;
        settingResolutionArrNum = LoadData.Instance.optionData.saveResolutionNum;

        SetResolution();
        resolutionText.text = resolutionTextArr[resolutionArrNum]; 
        fullScreenCheck.SetActive(windowScreen);
    }

    private void Start()
    {
        resolutionText.text = resolutionTextArr[resolutionArrNum];
        fullScreenCheck.SetActive(windowScreen);
    }


    private void Update()
    {
        Debug.Log(sceneType);
        Debug.Log(mapType);
        //BusMapEscUI();
        EscUI();
        //if(!isSetting && !isExit && Input.GetKeyDown(KeyCode.Escape)) StopUIOn();
        //if (!isSetting && isExit && Input.GetKeyDown(KeyCode.Escape)) StopUIOff();
    }

    #region Option UI

    // 옵션창 On
    public void SettingOn()
    {
        optionSettingUI.SetActive(true);
    }

    // 옵션창 Off
    public void SeetingOff()
    {
        optionSettingUI.SetActive(false);
    }

    // BGM 볼륨 네모칸 UI
    public void SetBGMSquares(float volumeBGM, GameObject[] BGMSquares)
    {
        int j = 0;
        for (float i = 0; i < 1; i += 0.1f)
        {
            if (i < volumeBGM) //켜있는거
            {
                BGMSquares[j].transform.GetChild(0).gameObject.SetActive(false);
                BGMSquares[j].transform.GetChild(1).gameObject.SetActive(true);
            }
            else //꺼있는거
            {
                BGMSquares[j].transform.GetChild(0).gameObject.SetActive(true);
                BGMSquares[j].transform.GetChild(1).gameObject.SetActive(false);
            }
            j++;
        }
    }

    // EFFECT 볼륨 네모칸 UI
    public void SetEffectSquares(float volumeEffect, GameObject[] effectSquares)
    {
        int j = 0;
        for (float i = 0; i < 1; i += 0.1f)
        {
            if (i < volumeEffect)
            {
                effectSquares[j].transform.GetChild(0).gameObject.SetActive(false);
                effectSquares[j].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                effectSquares[j].transform.GetChild(0).gameObject.SetActive(true);
                effectSquares[j].transform.GetChild(1).gameObject.SetActive(false);
            }
            j++;
        }
    }



 
    // 해상도 오른쪽 화살표 버튼클릭
    public void ResolutionRightButton()
    {
        resolutionArrNum = (resolutionArrNum + 1) % 5;
        resolutionText.text = resolutionTextArr[resolutionArrNum];
    }

    // 해상도 왼쪽 화살표 버튼클릭
    public void ResolutionLeftButton()
    {
        if (resolutionArrNum == 0) resolutionArrNum = 4;
        else
            resolutionArrNum = (resolutionArrNum - 1) % 5;

        resolutionText.text = resolutionTextArr[resolutionArrNum];
    }

    // 창모드 버튼 클릭
    public void OnClickFullScreenButton()
    {
        windowScreen = !windowScreen;
        fullScreenCheck.SetActive(windowScreen);
    }

    // 해상도 저장
    public void SaveResolution()
    {
        settingResolutionArrNum = resolutionArrNum;
        settingWindowScreen = windowScreen;

        //LoadData.Instance.SaveOptionDataToJson();

        SetResolution();
    }

    // 해상도 적용
    public void SetResolution()
    {
        switch (resolutionArrNum)
        {
            case 0:
                Screen.SetResolution(1280, 720, !windowScreen);
                break;
            case 1:
                Screen.SetResolution(1680, 1050, !windowScreen);
                break;
            case 2:
                Screen.SetResolution(1920, 1080, !windowScreen);
                break;
            case 3:
                Screen.SetResolution(2560, 1440, !windowScreen);
                break;
            case 4:
                Screen.SetResolution(3840, 2160, !windowScreen);
                break;
        }
    }

    // 옵션 취소시
    public void CancleResolution()
    {
        // 원래 설정값으로 변경
        resolutionArrNum = settingResolutionArrNum;
        windowScreen = settingWindowScreen;

        // 적용
        resolutionText.text = resolutionTextArr[resolutionArrNum];
        fullScreenCheck.SetActive(windowScreen);
    }

    #endregion


    // 그만두기창 On
    public void StopUIOn()
    {
        stopUI.SetActive(true);      
    }

    // 그만두기창 Off
    public void StopUIOff()
    {
        stopUI.SetActive(false);

        // BusMapEscUI도 같이 종료
        if(SceneManager.GetActiveScene().name == "WorldMap")
        {
            busMapEscUI.SetActive(false);
        }

        // stageMapEscUI도 같이 종료
        stageMapEscUI.SetActive(false);
        
    }

    #region Mask InOut UI

    // 마스크 축소 UI
    public void MaskInUI(GameObject inMask, string goTo)
    {
        SoundManager.Instance.ScreenInUI();
        inMask.SetActive(true);

        RectTransform inMaskRect = inMask.GetComponent<RectTransform>();
        float MaskInTime = 0;
        
        // 마스크 RECT 크게 설정
        if (inMask == broccoliMask)
        {
            inMaskRect.sizeDelta = new Vector2(4300, 4300);
            MaskInTime = 0.3f;
        }
        else if(inMask == pineappleMask) 
        {
            inMaskRect.sizeDelta = new Vector2(7300, 7300);
            MaskInTime = 0.5f;
        }

        StartCoroutine(MaskInOut(inMaskRect, Vector2.zero, MaskInTime, () =>
        {
            LoadingFood();
            switch (goTo)
            {
                case "BattleUISet":
                    Invoke(goTo, 1F);
                    break;
                case "BattleUIOff":
                    Invoke(goTo, 1F);
                    break;
                case "LoadingKeyUIOn":
                    Invoke(goTo, 1f);
                    break;
                case "LoadingKeyUIONBattle":
                    Invoke(goTo, 1f);
                    break;
                case "EnterBusMapMaskOut":
                    Invoke(goTo, 1f);
                    break;
                case "EnterIntroLoadingMaskOut":
                    busTopUI.SetActive(false);
                    Invoke(goTo, 1f);
                    break;
                case "LoadingMapUIOn":
                    busTopUI.SetActive(false);
                    Invoke(goTo, 1f);
                    break;
                case "EnterIntroMaskOut":
                    Invoke(goTo, 1f);
                    break;
                case "EnterStageMaskOut":
                    Invoke(goTo, 1f);
                    break;           
            }        
        }));
    }

    // 마스크 확대 UI
    public void MaskOutUI(GameObject inMask, GameObject outMask, string goTo)
    {
        SoundManager.Instance.ScreenOutUI();
        inMask.SetActive(false);
        outMask.SetActive(true);

        RectTransform outMaskRect = outMask.GetComponent<RectTransform>();
        outMaskRect.sizeDelta = new Vector2(0, 0);

        Vector2 targetRect = Vector2.zero;
        float MaskOutTime = 0;
        
        // 마스크 타겟 RECT 설정
        if (inMask == broccoliMask)
        {
            targetRect = new Vector2(4300, 4300);
            MaskOutTime = 0.3f;
        }
        else if (inMask == pineappleMask)
        {
            targetRect = new Vector2(7300, 7300);
            MaskOutTime = 0.5f;
        }

        // 씬 이동이 없을시 LoadingFoodOff 조금 느리게 꺼지게
        if (goTo == "") Invoke("LoadingFoodOff", 0.5f);
        else LoadingFoodOff();

        StartCoroutine(MaskInOut(outMaskRect, targetRect, MaskOutTime, () =>
        {
            outMask.SetActive(false);

            switch (goTo)
            {
                case "GoToBusMap":
                    EventManager<UIEvents>.TriggerEvent(UIEvents.WorldMapOpen);
                    break;
                case "GoToIntroMap":
                    EventManager<UIEvents>.TriggerEvent(UIEvents.IntroMapOpen);
                    break;
                case "GoToBattleRoom":
                    EventManager<UIEvents>.TriggerEvent(UIEvents.BattleRoomOpen);
                    break;
                case "GoToTestStage":
                    //mapType = MapType.Tuto;
                    EventManager<UIEvents>.TriggerEvent(UIEvents.TestStageMapOpen);
                    break;
                case "GoToMine":
                    EventManager<UIEvents>.TriggerEvent(UIEvents.stageMineMapOpen);
                    break;
                case "busTopUIOn":
                    busTopUI.SetActive(true);
                    break;
                
                default:
                    break;
            }
        }));
    }

    IEnumerator MaskInOut(RectTransform rt, Vector2 toSize, float time, System.Action onComplete)
    {
        Vector2 fromSize = rt.sizeDelta;
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            rt.sizeDelta = Vector2.Lerp(fromSize, toSize, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rt.sizeDelta = toSize; // 최종 크기 설정
        onComplete?.Invoke(); // 완료 시 콜백 호출
    }

    #endregion

    #region BattileUIOn

    public void BattleRoomUION()
    {
        battleRoomUI.SetActive(true);
    }

    public void BattleRoomUIOFF()
    {
        battleRoomUI.SetActive(false);
    }

    public void BattleUIOn()
    {
        MaskInUI(broccoliMask, "BattleUISet");
    }



    private void BattleUISet()
    {
        loadingKeyBar.fillAmount = 0;
        loadingKeyUI.SetActive(false);
        battleUI.SetActive(true);
        //buttonUI.SetActive(false);
        //SoundManager.Instance.FadeInAudio(SoundManager.Instance.bgmChangeAudioSource, 0, "Battle");
        //SoundManager.Instance.FadeOutAudio(SoundManager.Instance.bgmAudioSource, 0);
        MaskOutUI(broccoliMask, pineappleMask, "");
    }
    #endregion

    #region ExitLobbyUI & BattleUIOff

    public void ExitLobbyUIOn()
    {
        exitLobbyUI.SetActive(true);
    }

    public void CancleExitLobby()
    {
        exitLobbyUI.SetActive(false);
    }

    public void AgreeExitLobby()
    {
        EnterIntroLoadingMaskIn();
    }

    public void BattleUIOff()
    {
        buttonUI.SetActive(true);
        battleUI.SetActive(false);
        SoundManager.Instance.FadeInAudio(SoundManager.Instance.bgmAudioSource, 0, "Intro");
        SoundManager.Instance.FadeOutAudio(SoundManager.Instance.bgmChangeAudioSource, 0);
        MaskOutUI(pineappleMask, broccoliMask, "");      
    }

    #endregion

    #region LoadingKeyUI
    public void EnterLoadingKeyUI()
    {
        MaskInUI(broccoliMask, "LoadingKeyUIOn");
    }


    public void LoadingKeyUIOn()
    {
        loadingKeyUI.SetActive(true);
        MaskOutUI(broccoliMask, pineappleMask, "GoToBusMap");
    }

    public void EnterLoadingKeyUIBattle()
    {
        MaskInUI(broccoliMask, "LoadingKeyUIONBattle");
    }

    public void LoadingKeyUIONBattle()
    {
        loadingKeyUI.SetActive(true);
        MaskOutUI(broccoliMask, pineappleMask, "GoToBattleRoom");
    }


    #endregion

    #region LoadingMapUI
    public void EnterLoadingMapUI()
    {
        MaskInUI(pineappleMask, "LoadingMapUIOn");
    }

    public void LoadingMapUIOn()
    {
        loadingMapUI.SetActive(true);
        battleUI.SetActive(false);
        switch(mapType)
        {
            case MapType.Tuto:
                SetStageImageText(0);
                MaskOutUI(pineappleMask, broccoliMask, "GoToTestStage");
                break;
            case MapType.stage1_4:
                SetStageImageText(1);
                MaskOutUI(pineappleMask, broccoliMask, "GoToTestStage");
                break;
            case MapType.stage2_5:
                SetStageImageText(2);
                MaskOutUI(pineappleMask, broccoliMask, "GoToTestStage");
                break;
            case MapType.stage3_3:
                SetStageImageText(3);
                MaskOutUI(pineappleMask, broccoliMask, "GoToTestStage");
                break;
            case MapType.stageWizard:
                SetStageImageText(4);
                MaskOutUI(pineappleMask, broccoliMask, "GoToTestStage");
                break;
            case MapType.stageMine:
                SetStageImageText(5);
                MaskOutUI(pineappleMask, broccoliMask, "GoToMine");
                break;
        }
    }


    public void SetStageImageText(int arrNum)
    {
        mapImage[arrNum].SetActive(true);
        mapText[arrNum].SetActive(true);
        stageEscMapName[arrNum].SetActive(true);
    }
    #endregion

    #region EnterBusMap
    public void EnterBusMapMaskIn()
    {
        MaskInUI(pineappleMask, "EnterBusMapMaskOut");
    }

    public void EnterBusMapMaskOut()
    {
        loadingKeyBar.fillAmount = 0f;
        loadingKeyUI.SetActive(false);
        MaskOutUI(pineappleMask, broccoliMask, "busTopUIOn");
    }

    #endregion

    #region EnterIntroMap
    
    // 인트로 들어가는 MaskIn
    public void EnterIntroMaskIn()
    {
        MaskInUI(pineappleMask, "EnterIntroMaskOut");
    }

    // 인트로 들어가는 MaskOut
    public void EnterIntroMaskOut()
    {
        loadingKeyBar.fillAmount = 0f;
        loadingKeyUI.SetActive(false);
        MaskOutUI(pineappleMask, broccoliMask, "");
    }

    // 인트로 들어가기 전 LoadingUI MaskIn
    public void EnterIntroLoadingMaskIn()
    {
        MaskInUI(broccoliMask, "EnterIntroLoadingMaskOut");
    }

    // 인트로 들어가기 전 LoadingUI MaskOut
    public void EnterIntroLoadingMaskOut()
    {
        loadingKeyUI.SetActive(true);
        MaskOutUI(broccoliMask, pineappleMask, "GoToIntroMap");
    }
    #endregion

    #region EnterStage
    public void EnterStageMaskIn()
    {
        MaskInUI(broccoliMask, "EnterStageMaskOut");
    }

    public void EnterStageMaskOut()
    {
        loadingMapBar.fillAmount = 0f;
        loadingMapUI.SetActive(false);
        MaskOutUI(broccoliMask, pineappleMask, "");
    }


    #endregion

    #region EscUI
    public void BusMapEscUI()
    {

    }

    public void BusMapEscUICancle()
    {
        busMapEscUI.SetActive(false);
    }

    public void EscUI()
    { 
        if (sceneType == SceneType.BusMap)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                busMapEscUI.SetActive(true);
                SoundManager.Instance.ButtonTick();
            }
        }

        if (sceneType == SceneType.BattleMap || sceneType == SceneType.StageMap)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                stageMapEscUI.SetActive(true);
                SoundManager.Instance.ButtonTick();
            }
        }
    }

    public void EscUIButton()
    {
        if (sceneType == SceneType.BusMap)
        {
            busMapEscUI.SetActive(true);
            SoundManager.Instance.ButtonTick();
        }

        if (sceneType == SceneType.BattleMap || sceneType == SceneType.StageMap)
        {
            stageMapEscUI.SetActive(true);
            SoundManager.Instance.ButtonTick();
        }
    }



    public void StageEscUICancle()
    {
        stageMapEscUI.SetActive(false);
    }

    public void EscUIStopOn()
    {
        stopUI.SetActive(true);
    }

    public void EscUIStopOff()
    {
        stopUI.SetActive(false);
    }
    #endregion

    #region LoadingFood
    public void LoadingFood()
    {
        switch(Random.Range(0, 2))
        {
            case 0:
                loadingFoodArr[0].SetActive(true);
                loadingFoodArr[1].SetActive(false);
                loadingFoodArr[2].SetActive(false);
                break;
            case 1:
                loadingFoodArr[0].SetActive(false);
                loadingFoodArr[1].SetActive(true);
                loadingFoodArr[2].SetActive(false);
                break;
            case 2:
                loadingFoodArr[0].SetActive(false);
                loadingFoodArr[1].SetActive(false);
                loadingFoodArr[2].SetActive(true);
                break;
        }
    }

    public void LoadingFoodOff()
    {
        for (int i = 0; i < loadingFoodArr.Length; i++)
            loadingFoodArr[i].SetActive(false);
    }

    #endregion

    #region RecipeUI
    public void RecipeUIOn(int arr)
    {
        recipeUI.SetActive(true);
        recipeArr[arr].SetActive(true);
        StartCoroutine("IRecipeUIPopIn");
    }

    IEnumerator IRecipeUIPopIn()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.RecipeUIPopIn();
    }

    public void RecipeUIOff()
    {
        recipeUI.SetActive(false);
        for (int i=0; i<recipeArr.Length; i++)
            recipeArr[i].SetActive(false);
    }
    #endregion 

    public void ExitGame()
    {
        mapType = MapType.None;
        for (int i=0; i<mapImage.Length; i++)
        {
            mapImage[i].SetActive(false);
            mapText[i].SetActive(false);
            stageEscMapName[i].SetActive(false);
        }

        //if (SceneManager.GetActiveScene().name == "Map")
        if (sceneType == SceneType.BusMap)
        {
            EscUIStopOff();
            BusMapEscUICancle();
            EnterIntroLoadingMaskIn();
        }
        //else if (SceneManager.GetActiveScene().name == "TestStage")
        else if (sceneType == SceneType.BattleMap)
        {
            EscUIStopOff();
            StageEscUICancle();
            EnterLoadingKeyUIBattle();
            RecipeUIOff();
        }
        else if (sceneType == SceneType.StageMap)
        {
            EscUIStopOff();
            StageEscUICancle();
            EnterLoadingKeyUI();
            RecipeUIOff();
        }
        else if (sceneType == SceneType.Intro)
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                                                    Application.Quit();
            #endif
        }
    }
}