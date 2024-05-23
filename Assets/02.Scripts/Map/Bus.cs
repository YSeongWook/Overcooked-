using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Bus : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float rotspeed = 2f;

    private Vector3 inputMovement = Vector3.zero;
    private Vector3 move = Vector3.zero;
    
    public Rigidbody busRigid;

    public GameObject puffTransform; //퍼프가 실행되는 위치
    public bool isboost=false;

    public GameObject puffWalkPrefab;   // 걷는 퍼프 프리팹
    public GameObject puffBustPrefab;   // 버스트 퍼프 프리팹
    private IObjectPool<Puff> walkPool; // 걷는 퍼프 풀
    private IObjectPool<Puff> bustPool; // 버스트 퍼프 풀
    public int PuffCount = 0;


    private void Awake()//버스가 나올시 퍼프들의 풀을생성
    {
        walkPool = new ObjectPool<Puff>(CreateWalk, OnGetPuff, OnReleasePuff, OnDestroyPuff, maxSize: 1000);
        bustPool = new ObjectPool<Puff>(CreateBust, OnGetPuff, OnReleasePuff, OnDestroyPuff, maxSize: 1000);
    }


    private void Start()
    {
        isboost = false;
        busRigid= GetComponent<Rigidbody>();
    }

    void Update()
    {
        BusMove();
        BusRotate();
        OnBoost();
    }

    private void OnMove(InputValue inputValue)
    {
        Debug.Log("input1");
        inputMovement = inputValue.Get<Vector2>(); 
        move = new  Vector3(inputMovement.x, 0f, inputMovement.y);
    }

    public void BusMove()
    {
        transform.Translate(move * speed * Time.deltaTime, Space.World);
        
        //무브중 퍼프
        PuffCount++;
        if (move != Vector3.zero && PuffCount>=5) 
        {
            var walk = walkPool.Get();
            walk.transform.position = puffTransform.transform.position;
            PuffCount = 0;
        }
    }
    public void BusRotate()
    {
        //rotbusobjdet.transform.rotation= move=
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotspeed * Time.deltaTime);
        }
    }

    public void OnBoost()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            //Debug.Log("BOost");
            if (!isboost)
            {
                BoostPuff();
                isboost = true;
                StartCoroutine("BoostCoroutine");
            }
        }
    }

    IEnumerator BoostCoroutine()
    {
        Debug.Log("Boost");

        Rigidbody rb= GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 20,ForceMode.Impulse);

        for (int i = 0; i < 4; i++)
        {
            rb.AddForce(transform.forward, ForceMode.Impulse);
            yield return new WaitForSecondsRealtime(0.05f);
        }

        rb.velocity = Vector3.zero;
        isboost = false;
        //puff.Stop();
    }


    // 아래부터는 퍼프 관련 함수
    void BoostPuff() // 부스트 퍼프 실행
    {
        var bust = bustPool.Get();
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
        Puff puff = Instantiate(puffBustPrefab).GetComponent<Puff>();
        puff.SetManagedPool(bustPool);
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
