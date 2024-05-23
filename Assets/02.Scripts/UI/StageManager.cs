using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager instance = null; // 싱글톤 인스턴스 선언
    [SerializeField] public enum State { start, stage1, stage2, stage3 }; // 게임 상태 열거형
    public State playStage = State.stage1; // 현재 게임 상태 초기값 설정

    private bool isStop = false; // 일시정지 여부

    [SerializeField] public bool isClearMap1 = false; // 맵1 클리어 여부
    [SerializeField] public bool isClearMap2 = false; // 맵2 클리어 여부
    [SerializeField] public bool isClearMap3 = false; // 맵3 클리어 여부
    [SerializeField] public bool isAllClear = false; // 모든 맵 클리어 여부

    [SerializeField] public int map1Star = 0; // 맵1 별 개수
    [SerializeField] public int map2Star = 0; // 맵2 별 개수
    [SerializeField] public int map3Star = 0; // 맵3 별 개수

    [SerializeField] public bool stage1Space = false; // 스테이지1 공간 여부
    [SerializeField] public bool stage2Space = false; // 스테이지2 공간 여부
    [SerializeField] public bool stage3Space = false; // 스테이지3 공간 여부

    [SerializeField] public int tipMoney = 0; // 팁 금액
    [SerializeField] public int success = 0; // 성공 횟수
    [SerializeField] public int fail = 0; // 실패 횟수
    [SerializeField] public int totalMoney = 0; // 총 금액
    [SerializeField] public int failMoney = 0; // 실패 금액
    [SerializeField] public int successMoney = 0; // 성공 금액

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); // 씬 전환 시 오브젝트 유지
    }

    public void SetMap(GameObject[] flags, Material[] flags_m)
    {
        // 스테이지1에 대한 설정
        if (stage1Space)
        {
            GameObject[] something = GameObject.FindGameObjectsWithTag("stage1");
            for (int i = 0; i < something.Length; i++)
            {
                something[i].transform.GetChild(0).GetComponent<Animator>().enabled = false; // 애니메이터 비활성화
            }
            Material[] materials = flags[0].GetComponent<MeshRenderer>().materials;
            if (map1Star == 0)
            {
                materials[0].mainTexture = flags_m[0].mainTexture; // 별 개수에 따라 텍스처 설정
            }
            else if (map1Star == 1)
            {
                materials[0].mainTexture = flags_m[1].mainTexture;
            }
            else if (map1Star == 2)
            {
                materials[0].mainTexture = flags_m[2].mainTexture;
            }
            else
            {
                materials[0].mainTexture = flags_m[3].mainTexture;
            }
            flags[0].GetComponent<MeshRenderer>().materials = materials;
        }

        // 스테이지2에 대한 설정
        if (stage2Space)
        {
            GameObject[] something = GameObject.FindGameObjectsWithTag("stage2");
            for (int i = 0; i < something.Length; i++)
            {
                something[i].transform.GetChild(0).GetComponent<Animator>().enabled = false; // 애니메이터 비활성화
            }
            Material[] materials = flags[1].GetComponent<MeshRenderer>().materials;
            if (map2Star == 0)
            {
                materials[0].mainTexture = flags_m[0].mainTexture; // 별 개수에 따라 텍스처 설정
            }
            else if (map2Star == 1)
            {
                materials[0].mainTexture = flags_m[1].mainTexture;
            }
            else if (map2Star == 2)
            {
                materials[0].mainTexture = flags_m[2].mainTexture;
            }
            else
            {
                materials[0].mainTexture = flags_m[3].mainTexture;
            }
            flags[1].GetComponent<MeshRenderer>().materials = materials;
        }

        // 스테이지3에 대한 설정
        if (stage3Space)
        {
            GameObject[] something = GameObject.FindGameObjectsWithTag("stage3");
            for (int i = 0; i < something.Length; i++)
            {
                something[i].transform.GetChild(0).GetComponent<Animator>().enabled = false; // 애니메이터 비활성화
            }
            Material[] materials = flags[2].GetComponent<MeshRenderer>().materials;
            if (map3Star == 0)
            {
                materials[0].mainTexture = flags_m[0].mainTexture; // 별 개수에 따라 텍스처 설정
            }
            else if (map3Star == 1)
            {
                materials[0].mainTexture = flags_m[1].mainTexture;
            }
            else if (map3Star == 2)
            {
                materials[0].mainTexture = flags_m[2].mainTexture;
            }
            else
            {
                materials[0].mainTexture = flags_m[3].mainTexture;
            }
            flags[2].GetComponent<MeshRenderer>().materials = materials;
        }
    }
}
