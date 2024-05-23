using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

public class GameManager_Net : MonoBehaviour
{
    public static GameManager_Net instance = null; // 싱글톤 인스턴스 선언

    private Coroutine alphaCo = null; // 알파값 변화 코루틴 저장용 변수
    public enum State { stage1, stage2, stage3 }; // 게임 상태 열거형
    [SerializeField] State state = State.stage1; // 현재 게임 상태
    public Coroutine activeCo = null; // 현재 활성화된 코루틴

    public bool isStop = true; // 일시정지 여부
    private bool move = false; // 이동 여부

    // 시작 전 Ready, Go UI 관련 변수들
    [SerializeField] private float StartTime = 0; // 시작 시간
    [SerializeField] private GameObject Ready; // Ready UI
    [SerializeField] private GameObject Go; // Go UI
    [SerializeField] private GameObject Timesup; // Time's up UI
    private bool PlayOnce = false; // Ready UI 표시 여부
    private bool PlayTwice = false; // Go UI 표시 여부
    private bool StartSetting = false; // 게임 시작 설정 여부
    private bool once = false; // 한번만 실행되도록 하는 플래그
    private bool isDone = false; // 게임 종료 여부
    private float lastSec = 0f; // 마지막 시간
    private float countSec = 0f; // 초 카운트


    // 1-4STAGE : 3분30초, 
    // 2-5STAGE : 3분 30초
    // 3-3STAGE : 4분
    // BATTLE MINE : 4분
    // BATTLE WIZARD : 3분 30초

    // 시간 UI 관련 변수들
    [SerializeField] public float GameTime = 160f; // 게임 시간
    [SerializeField] private Slider TimeSlider; // 시간 슬라이더 UI
    [SerializeField] private Text TextTime; // 시간 텍스트 UI
    [SerializeField] private GameObject SandTimer; // 모래시계 오브젝트

    // 돈 UI 관련 변수들
    public int OriginalMoney; // 원래 돈
    public int Player1Money; // 플레이어 1의 돈
    public int Player2Money; // 플레이어 2의 돈
    public GameObject OppositeUI; // 상대방 UI

    [SerializeField] private GameObject CoinOb; // 동전 오브젝트
    [SerializeField] private Slider TipSlider; // 팁 슬라이더 UI
    [SerializeField] private int Coin; // 동전 개수
    [SerializeField] private int Tip; // 팁
    [SerializeField] private Text TextCoin; // 동전 텍스트 UI
    [SerializeField] private Text TextTip; // 팁 텍스트 UI
    public int tipCombo; // 팁 콤보
    [SerializeField] private GameObject flame; // 불꽃 이펙트
    [SerializeField] GameObject TextPrefabs; // 텍스트 프리팹

    [SerializeField] public GameObject wrong; // 잘못된 경우 UI

    // 접시 리스폰 관련 변수들
    [SerializeField] private GameObject platePrefabs; // 접시 프리팹
    [SerializeField] private float respawnTime = 3f; // 리스폰 시간
    [SerializeField] private GameObject ReturnCounter; // 반환 카운터

    [SerializeField] private Menu_Net[] Menus; // 이번 스테이지에 등장할 메뉴들
    [SerializeField] private int maxMenuLimit; // 이번 스테이지에서 최대로 쌓일 수 있는 메뉴 개수들
    [SerializeField] private GameObject[] Single_Double_PoolUIs; // 오브젝트 풀링으로 쓸 단일 메뉴 UI들
    [SerializeField] private GameObject[] Triple_PoolUIs; // 오브젝트 풀링으로 쓸 3개짜리 메뉴 UI들
    public List<Menu_Net> CurrentOrder; // 현재 주문 목록
    public List<GameObject> CurrentOrderUI; // 현재 주문 UI 목록
    public Vector3 poolPos; // 풀 위치
    [SerializeField] private GameObject Canvas; // 캔버스

    public int i = -1; // 메뉴 인덱스
    private int j = -1; // UI 인덱스

    float duration = 75; // 색상 변화 시간
    float smoothness = 0.1f; // 색상 변화의 부드러움 정도
    Color Start = new Color(0, 192 / 255f, 5 / 255f, 255 / 255f); // 초록색
    Color Middle = new Color(243 / 255f, 239 / 255f, 0, 255 / 255f); // 노랑색
    Color End = new Color(215 / 255f, 11 / 255f, 0, 1f); // 빨강색
    Color currentColor; // 현재 색상

    IEnumerator LerpColor1()
    {
        // 시작 색상에서 중간 색상으로 천천히 변화
        float progress = 0; // 색상 변화 진행도
        float increment = smoothness / duration; // 색상 변화 정도
        while (progress < 1)
        {
            currentColor = Color.Lerp(Start, Middle, progress);
            progress += increment;
            yield return new WaitForSeconds(smoothness);
            // 시간 슬라이더의 색상을 현재 색상으로 업데이트
            TimeSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = currentColor;
        }
        // 중간 색상에서 종료 색상으로 변화하는 코루틴 시작
        StartCoroutine(LerpColor2());
    }

    IEnumerator LerpColor2()
    {
        // 중간 색상에서 종료 색상으로 천천히 변화
        float progress = 0; // 색상 변화 진행도
        float increment = smoothness / duration; // 색상 변화 정도
        while (progress < 1)
        {
            currentColor = Color.Lerp(Middle, End, progress);
            progress += increment;
            yield return new WaitForSeconds(smoothness);
            // 시간 슬라이더의 색상을 현재 색상으로 업데이트
            TimeSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = currentColor;
        }
    }

    public IEnumerator TurnAlpha(GameObject panel)
    {
        // 패널의 알파값을 증가시키는 코루틴
        while (panel.GetComponent<Image>().color.a < 0.4f)
        {
            float alpha = panel.GetComponent<Image>().color.a;
            alpha += 0.05f;
            panel.GetComponent<Image>().color = new Color(panel.GetComponent<Image>().color.r, panel.GetComponent<Image>().color.g, panel.GetComponent<Image>().color.b, alpha);
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine(TurnAlphaZero(panel));
    }

    public IEnumerator TurnAlphaZero(GameObject panel)
    {
        // 패널의 알파값을 감소시키는 코루틴
        while (panel.GetComponent<Image>().color.a > 0)
        {
            float alpha = panel.GetComponent<Image>().color.a;
            alpha -= 0.05f;
            panel.GetComponent<Image>().color = new Color(panel.GetComponent<Image>().color.r, panel.GetComponent<Image>().color.g, panel.GetComponent<Image>().color.b, alpha);
            yield return new WaitForSeconds(0.01f);
        }
        alphaCo = null; // 알파값 변화 코루틴 종료
    }

    void Awake()
    {
        // 싱글플레이가 아닐 때 초기화 작업
        if (!SoundManager.Instance.isSingle)
        {
            SoundManager.Instance.bgmAudioSource.volume = 0;
            SoundManager.Instance.alreadyPlayed = false;
            OppositeUI.SetActive(true);
        }
        else
        {
            if (OppositeUI != null) OppositeUI.SetActive(false);
        }

        // 색상 및 시간 관련 변수 초기화
        duration = GameTime / 2;
        currentColor = TimeSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color;
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 스테이지 매니저 초기화
        if (StageManager_Net.instance != null)
        {
            StageManager_Net.instance.success = 0;
            StageManager_Net.instance.tipMoney = 0;
            StageManager_Net.instance.fail = 0;
            StageManager_Net.instance.totalMoney = 0;
            StageManager_Net.instance.successMoney = 0;
            StageManager_Net.instance.failMoney = 0;
            if (state == State.stage1)
            {
                StageManager_Net.instance.playStage = StageManager_Net.State.stage1;
            }
            else if (state == State.stage2)
            {
                StageManager_Net.instance.playStage = StageManager_Net.State.stage2;
            }
            else if (state == State.stage3)
            {
                StageManager_Net.instance.playStage = StageManager_Net.State.stage3;
            }
        }

        // 시간 및 돈 관련 초기화
        isStop = true;
        TimeSlider.maxValue = GameTime;
        TimeSlider.value = TimeSlider.maxValue;
        Coin = 0;
        SetCoinText();
        StartCoroutine(LerpColor1());
    }

    private void Update()
    {
        // 게임 시작 전 Ready, Go UI 표시
        if (isStop && !StartSetting)
        {
            ToClock();
            StartTime += Time.unscaledDeltaTime;
            Time.timeScale = 0;
            if (StartTime > 1 && !PlayOnce)
            {
                SoundManager.Instance.PlayEffect("ready");
                Ready.SetActive(true);
                PlayOnce = true;
            }
            else if (StartTime > 1 && PlayOnce && Ready.transform.localScale.x < 1)
            {
                Vector3 scale = Ready.transform.localScale;
                scale.x += Time.unscaledDeltaTime;
                scale.y += Time.unscaledDeltaTime;
                scale.z += Time.unscaledDeltaTime;
                Ready.transform.localScale = scale;
            }
            else if (StartTime > 4 && PlayOnce && Ready.transform.localScale.x >= 1 && !PlayTwice)
            {
                Ready.SetActive(false);
                PlayTwice = true;
                Go.SetActive(true);
                SoundManager.Instance.PlayEffect("go");
            }
            else if (PlayTwice && Go.transform.localScale.x < 1)
            {
                Vector3 scale = Go.transform.localScale;
                scale.x += Time.unscaledDeltaTime;
                scale.y += Time.unscaledDeltaTime;
                scale.z += Time.unscaledDeltaTime;
                Go.transform.localScale = scale;
            }
            else if (StartTime > 6 && PlayTwice && Go.transform.localScale.x > 1)
            {
                //SoundManager.Instance.StagePlay(SoundManager.Instance.StageName);
                //if (!SoundManager.Instance.isSingle) SoundManager.Instance.StagePlay("SampleScene");
                Go.SetActive(false);
                isStop = false;
                StartSetting = true;
            }
        }
        else if (StartSetting && !once)
        {
            // 게임 시작 후 초기 설정
            isStop = false;
            Time.timeScale = 1;
            once = true;
            Invoke("MakeOrder", 0.5f);
            Invoke("MakeOrder", 5f);
            Invoke("MakeOrder", 30f);
            Invoke("MakeOrder", 80f);
            Invoke("MakeOrder", 150f);
        }
        else if (SoundManager.Instance.isSingle && StartSetting && once)
        {
            // 싱글플레이 일시정지 처리
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (isStop)
                {
                    Time.timeScale = 1;
                    isStop = false;
                }
                else
                {
                    Time.timeScale = 0;
                    isStop = true;
                }
            }
        }

        // 게임 시간 감소 및 처리
        GameTime -= Time.deltaTime;
        ToClock();
        if (GameTime < 30 && SoundManager.Instance.bgmAudioSource.pitch == 1)
        {
            SoundManager.Instance.bgmAudioSource.pitch = 1.5f;
        }
        else if (GameTime < 15 && SoundManager.Instance.bgmAudioSource.pitch == 1.5f)
        {
            SoundManager.Instance.bgmAudioSource.pitch = 2;
        }

        if (GameTime < 30)
        {
            countSec += Time.deltaTime;
            SandTimer.GetComponent<Animator>().SetTrigger("shake");
        }
        if (countSec >= 1)
        {
            countSec = 0;
            SoundManager.Instance.PlayEffect("beep");
        }
        if (GameTime <= 0 && !isDone) // 시간이 다 되면 멈추기
        {
            SoundManager.Instance.bgmAudioSource.pitch = 1;
            SoundManager.Instance.bgmAudioSource.Stop();
            SoundManager.Instance.PlayEffect("timesUp");
            Time.timeScale = 0;
            if (StageManager_Net.instance != null) StageManager_Net.instance.totalMoney = Coin;
            Timesup.SetActive(true);
            if (StageManager_Net.instance != null && StageManager_Net.instance.totalMoney >= 0)
            {
                if (state == State.stage1)
                {
                    StageManager_Net.instance.isClearMap1 = true;
                }
                else if (state == State.stage2)
                {
                    StageManager_Net.instance.isClearMap2 = true;
                }
                else if (state == State.stage3)
                {
                    StageManager_Net.instance.isClearMap3 = true;
                }
            }
            isDone = true;
        }
        else if (GameTime <= 0 && isDone)
        {
            // 게임 종료 후 처리
            if (Timesup.transform.localScale.x < 1)
            {
                Vector3 scale = Timesup.transform.localScale;
                scale.x += Time.unscaledDeltaTime;
                scale.y += Time.unscaledDeltaTime;
                scale.z += Time.unscaledDeltaTime;
                Timesup.transform.localScale = scale;
            }
            else
            {
                if (SoundManager.Instance.isSingle)
                { // 싱글플레이
                    lastSec += Time.unscaledDeltaTime;
                    if (lastSec > 1)
                    {
                        SceneManager.LoadScene("ResultScene");
                    }
                }
            }
        }
    }

    public void LoadResult()
    {
        // 결과 씬 로드
        Debug.Log("실행");
    }

    private void ToClock()
    {
        // 게임 시간을 시계 형식으로 변환
        TimeSlider.value = GameTime;
        int min = 0;
        int sec = 0;
        min = (int)((int)GameTime / 60);
        sec = (int)((int)GameTime % 60);
        TextTime.text = string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);
    }

    public void PlateReturn()
    {
        // 접시 반환 처리
        StartCoroutine(PlateReturn_co());
    }

    IEnumerator PlateReturn_co()
    {
        // 일정 시간 후에 접시를 생성하여 반환 카운터에 추가
        yield return new WaitForSeconds(respawnTime);
        GameObject newPlate = Instantiate(platePrefabs, Vector3.zero, Quaternion.identity);
        if (state == State.stage1)
        {
            newPlate.GetComponent<Plates>().limit = 3;
        }
        else if(state == State.stage2)
        {
            newPlate.GetComponent<Plates>().limit = 3;
        }
        else if (state == State.stage3)
        {
            newPlate.GetComponent<Plates>().limit = 3;
        }
        newPlate.transform.SetParent(ReturnCounter.transform);
        newPlate.transform.localScale = Vector3.one;
        ReturnCounter.transform.GetChild(1).GetComponent<Return>().returnPlates.Add(newPlate);
        Vector3 spawnPos = ReturnCounter.transform.GetChild(1).GetComponent<Return>().SetPosition();
        newPlate.transform.localPosition = spawnPos;
        newPlate.GetComponent<Plates>().Canvas = Canvas;
    }

    public void MakeOrder()
    {
        // 새로운 주문 생성
        if (CurrentOrder.Count >= maxMenuLimit)
        {
            return;
        }
        i = -1;
        j = -1;
        i = Random.Range(0, Menus.Length);
        if (Menus[i].Ingredient.Count == 1) // 재료가 한 개일 때
        {
            for (j = 0; j < Single_Double_PoolUIs.Length; j++)
            {
                if (!Single_Double_PoolUIs[j].activeSelf) // 꺼져있는 UI 찾기
                {
                    Single_Double_PoolUIs[j].SetActive(true); // 켜주고
                    Single_Double_PoolUIs[j].transform.GetChild(1).gameObject.SetActive(false); // 두번째 재료 부분은 끄고
                    Single_Double_PoolUIs[j].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Menus[i].IngredientIcon[0]; // 첫번째 재료 아이콘 바꾸기
                    Single_Double_PoolUIs[j].transform.GetChild(2).GetChild(1).GetComponent<Image>().sprite = Menus[i].MenuIcon; // 메뉴 아이콘 바꾸기
                    Single_Double_PoolUIs[j].transform.GetChild(2).GetChild(0).GetComponent<Slider>().maxValue = Menus[i].LimitTime; // 슬라이더 시간 할당
                    Single_Double_PoolUIs[j].transform.GetChild(2).GetChild(0).GetComponent<Slider>().value = Menus[i].LimitTime; // 풀로 시작
                    CurrentOrder.Add(Menus[i]);
                    CurrentOrderUI.Add(Single_Double_PoolUIs[j]);
                    return;
                }
                else // 다 켜져있으면 실패
                {
                    continue;
                }
            }
        }
        else if (Menus[i].Ingredient.Count == 2) // 재료가 두 개일 때
        {
            for (j = 0; j < Single_Double_PoolUIs.Length; j++)
            {
                if (!Single_Double_PoolUIs[j].activeSelf) // 꺼져있는 UI 찾기
                {
                    Single_Double_PoolUIs[j].SetActive(true); // 켜주고
                    Single_Double_PoolUIs[j].transform.GetChild(1).gameObject.SetActive(true); // 두번째 재료 부분 켜기
                    Single_Double_PoolUIs[j].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Menus[i].IngredientIcon[0]; // 첫번째 재료 아이콘 바꾸기
                    Single_Double_PoolUIs[j].transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = Menus[i].IngredientIcon[1]; // 두번째 재료 아이콘 바꾸기
                    Single_Double_PoolUIs[j].transform.GetChild(2).GetChild(1).GetComponent<Image>().sprite = Menus[i].MenuIcon; // 메뉴 아이콘 바꾸기
                    Single_Double_PoolUIs[j].transform.GetChild(2).GetChild(0).GetComponent<Slider>().maxValue = Menus[i].LimitTime; // 슬라이더 시간 할당
                    Single_Double_PoolUIs[j].transform.GetChild(2).GetChild(0).GetComponent<Slider>().value = Menus[i].LimitTime; // 풀로 시작
                    CurrentOrder.Add(Menus[i]);
                    CurrentOrderUI.Add(Single_Double_PoolUIs[j]);
                    return;
                }
                else // 다 켜져있으면 실패
                {
                    continue;
                }
            }
        }
        else // 재료가 세 개일 때
        {
            for (j = 0; j < Triple_PoolUIs.Length; j++)
            {
                if (!Triple_PoolUIs[j].activeSelf) // 꺼져있는 UI 찾기
                {
                    Triple_PoolUIs[j].SetActive(true); // 켜주고
                    Triple_PoolUIs[j].transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Menus[i].IngredientIcon[0]; // 첫번째 재료 아이콘 바꾸기
                    Triple_PoolUIs[j].transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = Menus[i].IngredientIcon[1]; // 두번째 재료 아이콘 바꾸기
                    Triple_PoolUIs[j].transform.GetChild(0).GetChild(2).GetComponent<Image>().sprite = Menus[i].IngredientIcon[2]; // 세번째 재료 아이콘 바꾸기
                    Triple_PoolUIs[j].transform.GetChild(2).GetChild(1).GetComponent<Image>().sprite = Menus[i].MenuIcon; // 메뉴 아이콘 바꾸기
                    Triple_PoolUIs[j].transform.GetChild(2).GetChild(0).GetComponent<Slider>().maxValue = Menus[i].LimitTime; // 슬라이더 시간 할당
                    Triple_PoolUIs[j].transform.GetChild(2).GetChild(0).GetComponent<Slider>().value = Menus[i].LimitTime; // 풀로 시작
                    CurrentOrder.Add(Menus[i]);
                    CurrentOrderUI.Add(Triple_PoolUIs[j]);
                    return;
                }
                else // 다 켜져있으면 실패
                {
                    continue;
                }
            }
        }
    }

    private void SuccessEffect()
    {
        // 성공 효과
    }

    public bool CheckMenu(List<Ingredient_Net.IngredientType> containIngredients) // plate의 재료 list들 통으로 받아서 비교
    {
        // 메뉴 확인 및 처리
        OriginalMoney = Coin;
        if (containIngredients == null) // 빈 접시만 내면 무조건 실패
        {
            // 빨간색 띵
            // StartCoroutine(TurnAlpha(wrong));
            return false;
        }
        else
        {
            for (int i = 0; i < CurrentOrder.Count; i++) // 현재 쌓인 order에서 비교
            {
                if (containIngredients.Count != CurrentOrder[i].Ingredient.Count) // 재료 개수부터 다르면 실패
                {
                    // StartCoroutine(TurnAlpha(wrong));
                    continue;
                }
                else // 재료 개수가 같다면
                {
                    if (containIngredients.Count == 1) // 재료가 하나일 때
                    {
                        if (containIngredients[0] == CurrentOrder[i].Ingredient[0])
                        {
                            if (i == 0) // 순서대로 메뉴를 냈다면 콤보
                            {
                                tipCombo += 1;
                                if (tipCombo >= 4)
                                {
                                    if (!flame.activeSelf)
                                    {
                                        flame.SetActive(true);
                                    }
                                    tipCombo = 4; // 최대 4콤보까지
                                }
                            }
                            else
                            {
                                flame.SetActive(false);
                                tipCombo = 0;
                            }
                            CurrentOrderUI[i].transform.position = poolPos;
                            CurrentOrderUI[i].SetActive(false);
                            if (StageManager_Net.instance != null) StageManager_Net.instance.successMoney += CurrentOrder[i].Price;
                            Coin += CurrentOrder[i].Price;
                            CoinOb.transform.parent.GetChild(2).GetComponent<Animator>().SetTrigger("spin");
                            AddTip(i);
                            SetPosition(i);
                            CurrentOrder.RemoveAt(i);
                            CurrentOrderUI.RemoveAt(i);
                            if (StageManager_Net.instance != null) StageManager_Net.instance.success += 1;
                            return true;
                        }
                    }
                    else if (containIngredients.Count == 2) // 재료가 두 개일 때
                    {
                        if ((containIngredients[0] == CurrentOrder[i].Ingredient[0] && containIngredients[1] == CurrentOrder[i].Ingredient[1]) || (containIngredients[1] == CurrentOrder[i].Ingredient[0] && containIngredients[0] == CurrentOrder[i].Ingredient[1]))
                        {
                            if (i == 0) // 순서대로 메뉴를 냈다면 콤보
                            {
                                tipCombo += 1;
                                if (tipCombo >= 4)
                                {
                                    if (!flame.activeSelf)
                                    {
                                        flame.SetActive(true);
                                    }
                                    tipCombo = 4; // 최대 4콤보까지
                                }
                            }
                            else
                            {
                                flame.SetActive(false);
                                tipCombo = 0;
                            }
                            CurrentOrderUI[i].transform.position = poolPos;
                            CurrentOrderUI[i].SetActive(false);
                            if (StageManager_Net.instance != null) StageManager_Net.instance.successMoney += CurrentOrder[i].Price;
                            Coin += CurrentOrder[i].Price;
                            CoinOb.transform.parent.GetChild(2).GetComponent<Animator>().SetTrigger("spin");
                            AddTip(i);
                            SetPosition(i);
                            CurrentOrder.RemoveAt(i);
                            CurrentOrderUI.RemoveAt(i);
                            if (StageManager_Net.instance != null) StageManager_Net.instance.success += 1;
                            return true;
                        }
                    }
                    else if (containIngredients.Count == 3) // 재료가 세 개일 때
                    {
                        int count = 0;
                        for (int j = 0; j < containIngredients.Count; j++)
                        {
                            for (int k = 0; k < CurrentOrder[i].Ingredient.Count; k++)
                            {
                                if (containIngredients[j].Equals(CurrentOrder[i].Ingredient[k]))
                                {
                                    count++;
                                    break;
                                }
                            }
                        }
                        if (count == 3)
                        {
                            if (i == 0) // 순서대로 메뉴를 냈다면 콤보
                            {
                                tipCombo += 1;
                                if (tipCombo >= 4)
                                {
                                    if (!flame.activeSelf)
                                    {
                                        flame.SetActive(true);
                                    }
                                    tipCombo = 4; // 최대 4콤보까지
                                }
                            }
                            else
                            {
                                flame.SetActive(false);
                                tipCombo = 0;
                            }
                            CurrentOrderUI[i].transform.position = poolPos;
                            CurrentOrderUI[i].SetActive(false);
                            if (StageManager_Net.instance != null) StageManager_Net.instance.successMoney += CurrentOrder[i].Price;
                            Coin += CurrentOrder[i].Price;
                            CoinOb.transform.parent.GetChild(2).GetComponent<Animator>().SetTrigger("spin");
                            AddTip(i);
                            SetPosition(i);
                            CurrentOrder.RemoveAt(i);
                            CurrentOrderUI.RemoveAt(i);
                            if (StageManager_Net.instance != null) StageManager_Net.instance.success += 1;
                            return true;
                        }
                        else
                        {
                            // StartCoroutine(TurnAlpha(wrong));
                            return false;
                        }
                    }
                }
            }
        }
        // StartCoroutine(TurnAlpha(wrong));
        return false;
    }

    private void AddTip(int i)
    {
        // 팁 추가 처리
        if (CurrentOrderUI[i].GetComponent<OrderUI_Net>().timer.value > CurrentOrderUI[i].GetComponent<OrderUI_Net>().timer.maxValue * 0.6f)
        {
            Tip = 8;
        }
        else if (CurrentOrderUI[i].GetComponent<OrderUI_Net>().timer.value > CurrentOrderUI[i].GetComponent<OrderUI_Net>().timer.maxValue * 0.3f)
        {
            Tip = 5;
        }
        else
        {
            Tip = 3;
        }
        SetCoinText();
        StartCoroutine(StartBigger()); //---> 커졌다가 작아지는 코루틴 
    }

    private void SetCoinText()
    {
        // 동전 텍스트 업데이트
        if (tipCombo < 2) // 팁 콤보 업데이트
        {
            TextTip.text = "";
        }
        else
        {
            TextTip.text = "팁 x " + tipCombo.ToString(); // 팁 x 3 같은 글씨 바꾸기
            Tip *= tipCombo;
        }
        TipSlider.value = tipCombo;
        if (StageManager_Net.instance != null) StageManager_Net.instance.tipMoney += Tip;
        Coin += Tip;
        TextCoin.text = Coin.ToString(); // 돈 얼마됐다고 업데이트

        if (Tip != 0)
        {
            GameObject tipText = Instantiate(TextPrefabs, Camera.main.WorldToScreenPoint(FindObjectOfType<Station_Net>().transform.position), Quaternion.identity, Canvas.transform);
            tipText.GetComponent<Text>().text = "+" + Tip + " 팁!";
        }
    }

    IEnumerator StartBigger()
    {
        // 동전 텍스트 커지는 애니메이션
        float progress = 0;
        Color textColor = TextCoin.GetComponent<Text>().color;
        while (CoinOb.transform.localScale.x < 2)
        {
            textColor = Color.Lerp(Color.white, Start, progress);
            progress += Time.deltaTime * 3;
            TextCoin.GetComponent<Text>().color = textColor;
            Vector3 CurrentScale = TextCoin.gameObject.transform.localScale;
            CurrentScale.x += Time.deltaTime * 3;
            CurrentScale.y += Time.deltaTime * 3;
            CurrentScale.z += Time.deltaTime * 3;
            TextCoin.gameObject.transform.localScale = CurrentScale;
            yield return null;
        }
        StartCoroutine(StartSmaller());
    }

    IEnumerator StartSmaller()
    {
        // 동전 텍스트 작아지는 애니메이션
        float progress = 0;
        Color textColor = TextCoin.GetComponent<Text>().color;
        while (CoinOb.transform.localScale.x > 1)
        {
            textColor = Color.Lerp(Start, Color.white, progress);
            progress += Time.deltaTime * 3;
            TextCoin.GetComponent<Text>().color = textColor;
            Vector3 CurrentScale = TextCoin.gameObject.transform.localScale;
            CurrentScale.x -= Time.deltaTime * 3;
            CurrentScale.y -= Time.deltaTime * 3;
            CurrentScale.z -= Time.deltaTime * 3;
            TextCoin.gameObject.transform.localScale = CurrentScale;
            yield return null;
        }
    }

    public void MenuFail(GameObject whichUI) // 메뉴를 주어진 시간 내로 전달 못했을 때 작동
    {
        // 메뉴 실패 처리
        if (StageManager_Net.instance != null)
        {
            StageManager_Net.instance.fail += 1;
        }
        for (int i = 0; i < CurrentOrderUI.Count; i++)
        {
            if (CurrentOrderUI[i] == whichUI)
            {
                tipCombo = 0;
                Tip = 0;
                TextTip.text = "";
                TipSlider.value = tipCombo;
                if (StageManager_Net.instance != null) StageManager_Net.instance.failMoney += (int)(CurrentOrder[i].Price * 0.5f);
                Coin -= (int)(CurrentOrder[i].Price * 0.5f);
                TextCoin.text = Coin.ToString(); // 돈 얼마됐다고 업데이트
                if (alphaCo == null)
                {
                    alphaCo = StartCoroutine(TurnAlpha(wrong));
                }
                whichUI.transform.position = poolPos;
                whichUI.SetActive(false);
                SetPosition(i);
                CurrentOrderUI.RemoveAt(i);
                CurrentOrder.RemoveAt(i);
                MakeOrder();
            }
        }
        if (flame.activeSelf)
        {
            flame.SetActive(false);
        }
    }

    public void SetPosition(int i)
    {
        // UI 위치 조정
        float width = CurrentOrderUI[i].GetComponent<BoxCollider2D>().size.x;

        for (int j = 0; j < CurrentOrderUI.Count; j++)
        {
            if (i < j && !CurrentOrderUI[j].GetComponent<OrderUI_Net>().goLeft)
            {
                Vector3 CurrentPosition = CurrentOrderUI[j].transform.position;
                CurrentPosition.x -= width * 0.92f;
                CurrentOrderUI[j].transform.position = CurrentPosition;
            }
        }
    }
}
