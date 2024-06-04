using Cinemachine;
using Cysharp.Threading.Tasks;
using EnumTypes;
using EventLibrary;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    private AsyncOperation _operation;
    private void OnEnable()
    {
        // 씬 로드 완료 이벤트에 OnSceneLoaded 메서드를 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
        EventManager<SceneChangeEvent>.StartListening(SceneChangeEvent.WorldMapOpen, ChangeToWorldMap);
        EventManager<SceneChangeEvent>.StartListening(SceneChangeEvent.IntroMapOpen, ChangeToIntroMap);
        EventManager<SceneChangeEvent>.StartListening(SceneChangeEvent.BattleRoomOpen, ChangeToBattleLobby);
        EventManager<SceneChangeEvent>.StartListening(SceneChangeEvent.TestStageMapOpen, ChangeToTestStage);
        EventManager<SceneChangeEvent>.StartListening(SceneChangeEvent.Stage1_4MapOpen, ChangeToStage1_4);
        EventManager<SceneChangeEvent>.StartListening(SceneChangeEvent.Stage2_5MapOpen, ChangeToStage2_5);
        EventManager<SceneChangeEvent>.StartListening(SceneChangeEvent.Stage3_3MapOpen, ChangeToStage3_3);
        EventManager<SceneChangeEvent>.StartListening(SceneChangeEvent.StageWizardMapOpen, ChangeToStageWizard);
        EventManager<SceneChangeEvent>.StartListening(SceneChangeEvent.StageMineMapOpen, ChangeToStageMine);
    }

    private void OnDisable()
    {
        // 씬 로드 완료 이벤트에서 OnSceneLoaded 메서드를 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 버스 맵으로 전환
    private void ChangeToWorldMap()
    {
        ChangeScene("WorldMap", "WorldMap", UIManager.Instance.loadingKeyBar);
    }

    // 인트로 맵으로 전환
    private void ChangeToIntroMap()
    {
        ChangeScene("Intro", "Intro", UIManager.Instance.loadingKeyBar);
    }

    private void ChangeToBattleLobby()
    {
        ChangeScene("Battle", "BattleLobby", UIManager.Instance.loadingKeyBar);
    }
    // 테스트 스테이지로 전환
    private void ChangeToTestStage()
    {
        ChangeScene("Mine", "Mine", UIManager.Instance.loadingMapBar);
    }
    public void ChangeToStageTutorial()
    {
        ChangeScene("bgmName", "sceneName", UIManager.Instance.loadingMapBar);
    }

    private void ChangeToStage1_4()
    {
        ChangeScene("Sushi", "TestStage", UIManager.Instance.loadingMapBar);
        //ChangeScene("Sushi", "Stage1_4", UIManager.Instance.loadingMapBar);
    }

    private void ChangeToStage2_5()
    {
        ChangeScene("Mine", "Mine_Single", UIManager.Instance.loadingMapBar);
    }

    private void ChangeToStage3_3()
    {
        ChangeScene("Wizard", "Stage3_3", UIManager.Instance.loadingMapBar);
    }

    private void ChangeToStageWizard()
    {
        ChangeScene("Wizard", "Wizard", UIManager.Instance.loadingMapBar);
    }

    private void ChangeToStageMine()
    {
        ChangeScene("Mine", "Mine", UIManager.Instance.loadingMapBar);
    }

    // 씬 전환을 위한 공통 메서드
    private void ChangeScene(string bgmName, string sceneName, Image loadingBar)
    {
        if (SoundManager.Instance.bgmChangeAudioSource.isPlaying)
        {
            SoundManager.Instance.FadeOutAudio(SoundManager.Instance.bgmChangeAudioSource, 0);
            SoundManager.Instance.FadeInAudio(SoundManager.Instance.bgmAudioSource, 0, bgmName);
        }
        else if (SoundManager.Instance.bgmAudioSource.isPlaying)
        {
            SoundManager.Instance.FadeOutAudio(SoundManager.Instance.bgmAudioSource, 0);
            SoundManager.Instance.FadeInAudio(SoundManager.Instance.bgmChangeAudioSource, 0, bgmName);
        }

        LoadSceneAsyncUniTask(sceneName, loadingBar).Forget();      
    }

    // 씬을 비동기로 로드하는 코루틴

    private async UniTaskVoid LoadSceneAsyncUniTask(string sceneName, Image loadingBar)
    {
        _operation = SceneManager.LoadSceneAsync(sceneName);
        UIManager.Instance.LoadingFood();

        _operation.allowSceneActivation = false;
        float timer = 0;

        while (!_operation.isDone)
        {
            timer += Time.deltaTime;
            if (_operation.progress < 0.9f)
            {
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, _operation.progress, timer);
                if (loadingBar.fillAmount >= _operation.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 1f, timer);
                if (loadingBar.fillAmount == 1.0f)
                {
                    _operation.allowSceneActivation = true;
                    UIManager.Instance.LoadingFoodOff();
                    break;
                }
            }
            await UniTask.Yield(); // 프레임을 넘기기 위해 대기
        }
    }

    // 씬 로드 완료 후 호출되는 메서드
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        VanSingleton.Instance.van.SetActive(false);

        Debug.Log($"씬 로드 완료: {scene.name}");
        switch (scene.name)
        {
            case "WorldMap":
                //VanSingleton.Instance.van.SetActive(false);
                UIManager.Instance.EnterBusMapMaskIn();
#if UNITY_ANDROID
                UIManager.Instance.escButton.SetActive(true);
#endif
                break;
            case "Intro":
                VanSingleton.Instance.van.SetActive(true);
                UIManager.Instance.vanCamera = GameObject.Find("VanCam").GetComponent<CinemachineVirtualCamera>();
                UIManager.Instance.shutterCamera = GameObject.Find("ShutterCam").GetComponent<CinemachineVirtualCamera>();
                UIManager.Instance.sceneType = SceneType.Intro;
                UIManager.Instance.escButton.SetActive(false);
                if (!UIManager.Instance.first)
                    UIManager.Instance.EnterIntroMaskIn();
                break;
            case "BattleLobby":
                //UIManager.Instance.buttonUI.SetActive(false);
                UIManager.Instance.BattleUIOn();
                //UIManager.Instance.escButton.SetActive(false);

                break;
            case "TestStage":
                //UIManager.Instance.sceneType = SceneType.StageMap;
                UIManager.Instance.RecipeUIOn(0);
                UIManager.Instance.EnterStageMaskIn();

#if UNITY_ANDROID
                UIManager.Instance.escButton.SetActive(true);
#endif
                break;
            case "tutorial scene_name":
                UIManager.Instance.sceneType = SceneType.StageMap;
                UIManager.Instance.RecipeUIOn(0);
                UIManager.Instance.EnterStageMaskIn();
#if UNITY_ANDROID
                UIManager.Instance.escButton.SetActive(true);
#endif
                break;
            case "Mine_Single":
                UIManager.Instance.sceneType = SceneType.StageMap;
                UIManager.Instance.RecipeUIOn(1);
                UIManager.Instance.EnterStageMaskIn();
#if UNITY_ANDROID
                UIManager.Instance.escButton.SetActive(true);
#endif
                break;
            case "Stage2_5":
                UIManager.Instance.sceneType = SceneType.StageMap;
                UIManager.Instance.RecipeUIOn(1);
                UIManager.Instance.EnterStageMaskIn();
#if UNITY_ANDROID
                UIManager.Instance.escButton.SetActive(true);
#endif
                break;
            case "Stage3_3":
                UIManager.Instance.sceneType = SceneType.StageMap;
                UIManager.Instance.RecipeUIOn(2);
                UIManager.Instance.EnterStageMaskIn();
#if UNITY_ANDROID
                UIManager.Instance.escButton.SetActive(true);
#endif
                break;
            case "Wizard":
                //UIManager.Instance.sceneType = SceneType.BattleMap;
                UIManager.Instance.RecipeUIOn(0);
                UIManager.Instance.EnterStageMaskIn();
#if UNITY_ANDROID
                UIManager.Instance.escButton.SetActive(true);
#endif
                break;
            case "Mine":
                //UIManager.Instance.sceneType = SceneType.BattleMap;
                UIManager.Instance.RecipeUIOn(1);
                UIManager.Instance.EnterStageMaskIn();
#if UNITY_ANDROID
                UIManager.Instance.escButton.SetActive(true);
#endif
                break;
        }
    }
}
