using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EnumTypes;
using EventLibrary;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using Random = UnityEngine.Random;

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
    public CustomReadyButtonScript battleUIReadyBtn;

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
    public GameObject worldMapEscUI;
    public GameObject busTopUI;

    [Header("StageMap")]
    public GameObject stageMapEscUI;
    [SerializeField] private GameObject[] stageEscMapName;

    [Header("RecipeUI")]
    public GameObject recipeUI;
    [SerializeField] private GameObject[] recipeArr;

    [Header("Resolution")]
    public TextMeshProUGUI resolutionText;
    public GameObject fullScreenButton;
    public GameObject fullScreenCheck;
    public bool windowScreen;
    private bool settingWindowScreen;
    public int resolutionArrNum;
    private int settingResolutionArrNum;
    [SerializeField] private string[] resolutionTextArr
        = new string[] { "1280 x 720", "1680 x 1050", "1920 x 1080", "2560 x 1440", "3840 x 2160" };

    public bool first = true;
    public SceneType sceneType;
    public MapType mapType = MapType.None;

    private Stack<GameObject> popupStack = new Stack<GameObject>();

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
        //Debug.Log(sceneType);
        //Debug.Log(mapType);
        HandleEscapeInput();
    }



    #region Popup
    private void ToggleUI(GameObject uiElement, bool state)
    {
        uiElement.SetActive(state);
        if (state)
        {
            popupStack.Push(uiElement);
        }
        else
        {
            RemoveFromStack(uiElement);
        }
    }

    private void RemoveFromStack(GameObject uiElement)
    {
        if (popupStack.Contains(uiElement))
        {
            var tempStack = new Stack<GameObject>();
            while (popupStack.Count > 0)
            {
                var popup = popupStack.Pop();
                if (popup != uiElement)
                {
                    tempStack.Push(popup);
                }
            }
            while (tempStack.Count > 0)
            {
                popupStack.Push(tempStack.Pop());
            }
        }
    }
    // 옵션창 On
    public void SettingOn()
    {
        ToggleUI(optionSettingUI, true);
    }
    // 옵션창 Off
    public void SeetingOff()
    {
        ToggleUI(optionSettingUI, false);
    }
    // 그만두기창 On
    public void StopUIOn()
    {
        ToggleUI(stopUI, true);
    }
    // 그만두기창 Off
    public void StopUIOff()
    {
        ToggleUI(stopUI, false);
        ToggleUI(worldMapEscUI, false);
        ToggleUI(stageMapEscUI, false);
    }
    
    public void ExitLobbyUIOn()
    {
        ToggleUI(exitLobbyUI, true);
    }
    public void ExitLobbyUIOff() //CancleExitLobby
    {
        ToggleUI(exitLobbyUI, false);
    }

    public void StageMapEscUION()
    {
        ToggleUI(stageMapEscUI, true);
    }
    public void StageMapEscUIOff() // StageEscUICancle
    {
        ToggleUI(stageMapEscUI, false);
    }

    public void worldMapEscUIOn()
    {
        ToggleUI(worldMapEscUI, true);
    }
    public void worldMapEscUIOff() //BusMapEscUICancle
    {
        ToggleUI(worldMapEscUI, false);
    }

    public void BattleRoomUION()
    {
        ToggleUI(battleRoomUI, true);
    }
    public void BattleRoomUIOFF()
    {
        ToggleUI(battleRoomUI, false);
    }

    #endregion

    #region Option UI

    // 볼륨 네모칸 UI
    public void SetSoundSquares(float volumeBGM, GameObject[] BGMSquares)
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

    #region Mask InOut Tool

    // 마스크 축소 UI
    private async void MaskInUI(GameObject inMask, string goTo)
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
        else if (inMask == pineappleMask)
        {
            inMaskRect.sizeDelta = new Vector2(7300, 7300);
            MaskInTime = 0.5f;
        }

        await MaskInOutAsync(inMaskRect, Vector2.zero, MaskInTime, () =>
        {
            LoadingFood();
            switch (goTo)
            {
                case "BattleUISet":
                    Invoke(goTo, 1F);
                    break;
                case "LoadingKeyUIOn":
                    Invoke(goTo, 1f);
                    break;
                case "LoadingKeyUIOnBattle":
                    Invoke(goTo, 1f);
                    break;
                case "EnterBusMapMaskOut":
                    Invoke(goTo, 1f);
                    break;
                case "EnterIntroLoadingMaskOut":
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
        });
    }

    // 마스크 확대 UI
    public async void MaskOutUI(GameObject inMask, GameObject outMask, string goTo)
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

        await MaskInOutAsync(outMaskRect, targetRect, MaskOutTime, () =>
        {
            outMask.SetActive(false);

            switch (goTo)
            {
                case "GoToWorldMap":
                    sceneType = SceneType.WorldMap;
                    EventManager<SceneChangeEvent>.TriggerEvent(SceneChangeEvent.WorldMapOpen);
                    break;
                case "GoToIntroMap":
                    sceneType = SceneType.Intro;
                    EventManager<SceneChangeEvent>.TriggerEvent(SceneChangeEvent.IntroMapOpen);
                    break;
                case "GoToBattleRoom":
                    sceneType = SceneType.BattleLobby;
                    EventManager<SceneChangeEvent>.TriggerEvent(SceneChangeEvent.BattleRoomOpen);
                    break;
                case "GoToTestStage":
                    //mapType = MapType.Tuto;
                    EventManager<SceneChangeEvent>.TriggerEvent(SceneChangeEvent.TestStageMapOpen);
                    break;
                case "GoToStage2_5":
                    EventManager<SceneChangeEvent>.TriggerEvent(SceneChangeEvent.Stage2_5MapOpen);
                    break;
                case "GoToMine":
                    EventManager<SceneChangeEvent>.TriggerEvent(SceneChangeEvent.StageMineMapOpen);
                    break;
                case "GoToWizard":
                    EventManager<SceneChangeEvent>.TriggerEvent(SceneChangeEvent.StageWizardMapOpen);
                    break;
                case "busTopUIOn":
                    busTopUI.SetActive(true);
                    break;

                default:
                    break;
            }
        });
    }

    private async UniTask MaskInOutAsync(RectTransform rt, Vector2 toSize, float time, System.Action onComplete = null)
    {
        Vector2 fromSize = rt.sizeDelta;
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            rt.sizeDelta = Vector2.Lerp(fromSize, toSize, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }

        rt.sizeDelta = toSize; // 최종 크기 설정
        onComplete?.Invoke();  // 완료 시 콜백 호출
    }

    #endregion

    #region BattleUI Mask

    // 배틀로비 들어가기 전 LodingUI MaskIn
    public void EnterLoadingKeyUIBattle()
    {
        MaskInUI(broccoliMask, "LoadingKeyUIOnBattle");
    }

    // 배틀로비 들어가기전 LoadingUI MaskOut
    public void LoadingKeyUIOnBattle()
    {
        buttonUI.SetActive(false);
        escButton.SetActive(false);
        loadingKeyUI.SetActive(true);
        BattleRoomUIOFF();
        StopUIOff();
        StageMapEscUIOff();
        RecipeUIOff();
        MaskOutUI(broccoliMask, pineappleMask, "GoToBattleRoom");
    }


    // 배틀로비 MaskIn
    public void BattleUIOn()
    {
        MaskInUI(broccoliMask, "BattleUISet");
    }

    // 배틀로비 MaskOut
    public void BattleUISet()
    {
        loadingKeyBar.fillAmount = 0;
        loadingKeyUI.SetActive(false);
        battleUI.SetActive(true);
        MaskOutUI(broccoliMask, pineappleMask, "");
    }
    #endregion

    #region Intro Mask
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

    // 인트로씬 MaskIn
    public void EnterIntroMaskIn()
    {
        VanSingleton.Instance.van.SetActive(true);
        busTopUI.SetActive(false);
        buttonUI.SetActive(true);
        battleUI.SetActive(false);
        shutterCamera.Priority = 9;
        ExitLobbyUIOff();
        MaskInUI(pineappleMask, "EnterIntroMaskOut");
    }

    // 인트로씬 MaskOut
    public void EnterIntroMaskOut()
    {
        loadingKeyBar.fillAmount = 0f;
        loadingKeyUI.SetActive(false);
        MaskOutUI(pineappleMask, broccoliMask, "");
    }

    #endregion

    #region WorldMap Mask
    public void EnterLoadingKeyUI()
    {
        MaskInUI(broccoliMask, "LoadingKeyUIOn");
    }

    public void LoadingKeyUIOn()
    {
        loadingKeyUI.SetActive(true);
        MaskOutUI(broccoliMask, pineappleMask, "GoToWorldMap");
        //MaskOutUI(broccoliMask, pineappleMask, "");
    }

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

    #region EnterStage

    public void EnterLoadingMapUI()
    {
        MaskInUI(pineappleMask, "LoadingMapUIOn");
    }

    public void LoadingMapUIOn()
    {
        loadingMapUI.SetActive(true);
        battleUI.SetActive(false);
        switch (mapType)
        {
            case MapType.Tuto:
                SetStageImageText(0);
                MaskOutUI(pineappleMask, broccoliMask, "GoToTestStage");
                break;
            case MapType.Stage1_4:
                SetStageImageText(1);
                MaskOutUI(pineappleMask, broccoliMask, "GoToTestStage");
                break;
            case MapType.Stage2_5:
                SetStageImageText(2);
                MaskOutUI(pineappleMask, broccoliMask, "GoToStage2_5");
                break;
            case MapType.Stage3_3:
                SetStageImageText(3);
                MaskOutUI(pineappleMask, broccoliMask, "GoToTestStage");
                break;
            case MapType.StageWizard:
                SetStageImageText(4);
                MaskOutUI(pineappleMask, broccoliMask, "GoToWizard");
                break;
            case MapType.StageMine:
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

    private void HandleEscapeInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SoundManager.Instance.ButtonTick();
            
            if (popupStack.Count > 0)
            {
                var topPopup = popupStack.Pop();
                topPopup.SetActive(false);
            }
            else
            {
                ShowDefaultUIForScene();
            }
        }
    }

    private void ShowDefaultUIForScene()
    {
        switch (sceneType)
        {
            case SceneType.WorldMap:
                ToggleUI(worldMapEscUI, true);
                break;
            case SceneType.BattleMap:
            case SceneType.StageMap:
                ToggleUI(stageMapEscUI, true);
                break;
            case SceneType.Intro:
                ToggleUI(stopUI, true);
                break;
            case SceneType.BattleLobby:
                ToggleUI(exitLobbyUI, true);
                break;
        }
    }

    public void EscUIButton()
    {
        SoundManager.Instance.ButtonTick();
        if (sceneType == SceneType.WorldMap)
        {
            worldMapEscUIOn();
        }

        if (sceneType == SceneType.BattleMap || sceneType == SceneType.StageMap)
        {
            StageMapEscUION();
        }
    }

    public void ExitGame()
    {
        mapType = MapType.None;
        for (int i = 0; i < mapImage.Length; i++)
        {
            mapImage[i].SetActive(false);
            mapText[i].SetActive(false);
            stageEscMapName[i].SetActive(false);
        }
        if (sceneType == SceneType.WorldMap)
        {
            StopUIOff();
            worldMapEscUIOff();
            EnterIntroLoadingMaskIn();
        }
        else if(sceneType == SceneType.BattleLobby)
        {
            EnterIntroLoadingMaskIn();
        }
        else if (sceneType == SceneType.BattleMap)
        {
            EnterLoadingKeyUIBattle();
        }
        else if (sceneType == SceneType.StageMap)
        {
            StopUIOff();
            StageMapEscUIOff();
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

    #endregion

    #region LoadingFood
    public void LoadingFood()
    {
        switch (Random.Range(0, 2))
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
        RecipeUIPopIn().Forget();
    }

    private async UniTask RecipeUIPopIn()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        SoundManager.Instance.RecipeUIPopIn();
    }

    public void RecipeUIOff()
    {
        recipeUI.SetActive(false);
        if (Application.platform == RuntimePlatform.Android)
            EventManager<UIEvents>.TriggerEvent(UIEvents.MobilePlayerController);
        foreach (var recipe in recipeArr)
            recipe.SetActive(false);
    }

    #endregion 

}