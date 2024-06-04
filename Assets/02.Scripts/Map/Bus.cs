using EnumTypes;
using EventLibrary;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.UI;

public class Bus : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Speed")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotSpeed = 2f;
    [SerializeField] private float boostPower = 200f;

    public bool isBoost = false;

    private Vector2 moveInput = Vector3.zero;

    [Header("Puff")]
    public GameObject puffTransform;        // 퍼프가 실행되는 위치
    public GameObject puffWalkPrefab;       // 걷는 퍼프 프리팹
    public GameObject puffBurstPrefab;      // 버스트 퍼프 프리팹
    private IObjectPool<Puff> walkPool;     // 걷는 퍼프 풀
    private IObjectPool<Puff> burstPool;    // 버스트 퍼프 풀
    public int PuffCount = 0;

    [Header("Mobile Button")]
    public Button dashButton;   // 모바일 대쉬 버튼
    public Button clickButton;   // 모바일 입장 버튼
    public string enterFlagName;

    private void Awake() // 버스가 나올시 퍼프들의 풀을생성
    {
        walkPool = new ObjectPool<Puff>(CreateWalk, OnGetPuff, OnReleasePuff, OnDestroyPuff, maxSize: 1000);
        burstPool = new ObjectPool<Puff>(CreateBust, OnGetPuff, OnReleasePuff, OnDestroyPuff, maxSize: 1000);

        if (dashButton != null)
        {
            dashButton.onClick.AddListener(MobileBoost); // 버튼 클릭 이벤트에 MobileCookOrThrow 메서드 연결
        }

        if (clickButton != null)
        {
            clickButton.onClick.AddListener(MobileEnter);
        }
    }

    private void Start()
    {
        isBoost = false;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isBoost && moveInput != Vector2.zero)
        {
            MoveBus();
        }
    }

    void OnTriggerStay(Collider other)
    {    
        if (other.CompareTag("Flag"))
        {
            enterFlagName = other.name;
        }
    }
    void OnTriggerExit(Collider other)
    {
        enterFlagName = "";
    }


    public void OnMove(InputValue inputValue)
    {
        Debug.Log("OnMove");
        if (inputValue != null) moveInput = inputValue.Get<Vector2>();
    }

    public void MoveBus()
    {
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Vector3 movement = moveDir * moveSpeed;
        rb.velocity = movement;

        // 버스가 이동하는 방향을 바라보도록 회전
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
        }

        // 무브 중 퍼프
        PuffCount++;
        if (moveInput != Vector2.zero && PuffCount >= 5)
        {
            var walk = walkPool.Get();
            walk.transform.position = puffTransform.transform.position;
            PuffCount = 0;
        }
    }

    public void OnBoost()
    {
        if (!isBoost)
        {
            BoostPuff();
            isBoost = true;
            StartCoroutine("BoostCoroutine");
        }
    }

    public void MobileBoost()
    {
        if (!isBoost)
        {
            BoostPuff();
            isBoost = true;
            StartCoroutine("BoostCoroutine");
        }
    }


    public void MobileEnter()
    {
        Debug.Log(enterFlagName);
        switch (enterFlagName)
        {
            case "Stage1 UI":
                //UIManager.Instance.sceneType = SceneType.BattleMap;
                //UIManager.Instance.mapType = MapType.stageMine;
                UIManager.Instance.sceneType = SceneType.StageMap;
                UIManager.Instance.mapType = MapType.Stage2_5;
                UIManager.Instance.EnterLoadingMapUI();
                break;
            case "Stage2 UI":
                UIManager.Instance.sceneType = SceneType.StageMap;
                UIManager.Instance.mapType = MapType.Stage2_5;
                UIManager.Instance.EnterLoadingMapUI();
                break;
            case "Stage3 UI":
                UIManager.Instance.sceneType = SceneType.StageMap;
                UIManager.Instance.mapType = MapType.Stage3_3;
                UIManager.Instance.EnterLoadingMapUI();
                break;
            default:
                Debug.Log(enterFlagName);
                break;
        }
    }

    IEnumerator BoostCoroutine()
    {
        Rigidbody rb= GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * boostPower, ForceMode.Impulse);

        for (int i = 0; i < 4; i++)
        {
            rb.AddForce(transform.forward, ForceMode.Impulse);
            yield return new WaitForSecondsRealtime(0.05f);
        }

        rb.velocity = Vector3.zero;
        isBoost = false;
        //puff.Stop();
    }

    void BoostPuff() // 부스트 퍼프 실행
    {
        var bust = burstPool.Get();
        bust.transform.position = puffTransform.transform.position;
    }

    private Puff CreateWalk() // 워크퍼프 생성 후 풀에 담음
    {
        Puff puff = Instantiate(puffWalkPrefab).GetComponent<Puff>();
        puff.SetManagedPool(walkPool);
        puff.transform.localScale = Vector3.one;
        return puff;
    }

    private Puff CreateBust() // 버스트 퍼프 생성
    {
        Puff puff = Instantiate(puffBurstPrefab).GetComponent<Puff>();
        puff.SetManagedPool(burstPool);
        puff.transform.localScale = Vector3.one;
        return puff;
    }

    private void OnGetPuff(Puff puff) // 퍼프를 불러옴
    {
        puff.gameObject.SetActive(true);
    }

    private void OnReleasePuff(Puff puff) // 퍼프 비활성화
    {
        puff.gameObject.SetActive(false);
    }

    private void OnDestroyPuff(Puff puff) // 퍼프 삭제
    {
        Destroy(puff.gameObject);
    }
}
