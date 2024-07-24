using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanOnStove : MonoBehaviour
{
    [Header("UI")]
    public GameObject canvas; // UI 캔버스
    public Slider cookingBar; // 요리 진행 바
    public GameObject ingredientUI; // 재료 UI

    [Space(10)]
    public GameObject pfxFire; // 불 이펙트

    [Header("State")]
    public bool isOnStove; // 팬이 스토브 위에 있는지 여부
    public bool inSomething; // 팬에 재료가 있는지 여부
    public float cookingTime; // 요리 시간

    [Space(10)]
    public Material[] friedMaterials; // 요리된 재료의 재질 배열

    private Camera _camera;
    private CancellationTokenSource _cts; // 비동기 작업 취소 토큰
    private bool _pause; // 요리 일시 정지 여부
    private bool _stateIsCooked; // 재료가 요리되었는지 여부
    private Ingredient_Net _ingredient; // 현재 팬에 있는 재료

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        // 팬 위에 음식이 있을 때 불을 켜고, 없으면 끕니다.
        pfxFire.SetActive(isOnStove && inSomething);

        // 요리 중인 상태 업데이트
        if (isOnStove && inSomething && !_stateIsCooked)
        {
            UpdateCookingBarValue();
        }
    }

    // 요리 진행 바의 위치 업데이트
    private void UpdateCookingBarPosition()
    {
        if (_camera)
            cookingBar.transform.position = _camera.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0)); // 적절한 위치 조정
    }

    // 요리 진행 바의 값 업데이트
    private void UpdateCookingBarValue()
    {
        cookingBar.value = cookingTime;
    }

    // 새로운 재료가 팬에 추가될 때 호출되는 메서드
    public void AddNewIngredient()
    {
        Debug.Log("AddNewIngredient");
        _ingredient = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient_Net>();
        inSomething = true;
        
        // 이미 요리된 재료인 경우 반환합니다.
        if (_ingredient.isCooked)
        {
            Debug.LogWarning("이미 요리된 재료는 추가할 수 없습니다.");
            return;
        }
        
        _stateIsCooked = false; // 새로운 재료가 추가되면 요리 상태를 초기화
        cookingTime = 0; // 요리 시간을 초기화
        StartCooking();
        cookingBar.gameObject.SetActive(true); // 슬라이더를 활성화
        UpdateCookingBarPosition(); // 슬라이더의 위치를 업데이트
        UpdateCookingBarValue();
    }

    // 요리를 시작합니다.
    public async void StartCooking(UnityAction EndCallBack = null)
    {
        if (_cts == null)
        {
            Debug.Log("start cooking");
            ClearTime();

            _cts = new CancellationTokenSource();
            StartCookingAsync(EndCallBack, _cts.Token).Forget();
        }
        else if (_pause)
        {
            _pause = false; // 일시 정지를 해제합니다.
        }
    }

    // 비동기적으로 요리를 시작합니다.
    private async UniTask StartCookingAsync(UnityAction EndCallBack = null, CancellationToken cancellationToken = default)
    {
        if (!inSomething)
        {
            pfxFire.SetActive(false); // 재료가 없으면 불을 끕니다.
            return;
        }

        // 요리 시간을 증가시키며 요리 상태를 업데이트합니다.
        while (cookingTime <= 1)
        {
            if (!inSomething || cancellationToken.IsCancellationRequested)
            {
                pfxFire.SetActive(false);
                return;
            }

            while (_pause)
            {
                await UniTask.Yield(cancellationToken); // 일시 정지된 동안 대기합니다.
            }

            await UniTask.Delay(450, cancellationToken: cancellationToken); // 요리 시간이 증가하는 간격
            cookingTime += 0.25f;
            UpdateCookingBarValue();
        }

        Debug.Log("Cooking End");
        pfxFire.SetActive(false); // 요리가 끝나면 불을 끕니다.
        UpdateIsIngredientState();
        EndCallBack?.Invoke();
        OffSlider();

        _pause = false;
        cookingTime = 0;
        SoundManager.Instance.PlayEffect("imCooked");
        _stateIsCooked = true; // 요리가 끝난 후 상태를 요리됨으로 설정
        _cts.Dispose(); // 취소 토큰 소스를 해제합니다.
        _cts = null;
    }

    // 요리 시간을 초기화합니다.
    private void ClearTime()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
        _pause = false;
    }

    // 재료의 상태 업데이트
    private void UpdateIsIngredientState()
    {
        if (_ingredient == null) return;
        
        _stateIsCooked = _ingredient.isCooked;
        ChangeCookedMaterial();
    }

    // 요리 진행 바를 비활성화합니다.
    public void OffSlider()
    {
        cookingBar.value = 0f;
        cookingBar.gameObject.SetActive(false);
        UpdateIngredientState();
        InstantiateUI();
    }

    // 재료의 상태를 업데이트합니다.
    private void UpdateIngredientState()
    {
        if (_ingredient == null) return;

        _ingredient.isCooked = true;
        _ingredient.ChangeMesh(_ingredient.type);
    }

    // 재료 UI를 생성합니다.
    private void InstantiateUI()
    {
        if (_ingredient == null) return;

        GameObject madeUI = Instantiate(ingredientUI, Vector3.zero, Quaternion.identity, canvas.transform);
        madeUI.transform.GetChild(0).gameObject.SetActive(true);
        Image image = madeUI.transform.GetChild(0).GetComponent<Image>();
        image.sprite = IconManager_Net.Instance.GetIcon(_ingredient.type);
        madeUI.GetComponent<IngredientUI>().target = _ingredient.transform;
    }

    // 요리된 재료의 재질을 변경합니다.
    private void ChangeCookedMaterial()
    {
        Debug.LogError("ChangeCookedMaterial");
        if (_ingredient == null) return;

        // 부모 오브젝트에서 MeshFilter와 MeshRenderer를 가져옵니다.
        Transform parentTransform = _ingredient.transform.parent;
        if (parentTransform == null) return; // 부모가 없는 경우 반환

        MeshFilter meshFilter = parentTransform.GetComponent<MeshFilter>();
        MeshRenderer mr = parentTransform.GetComponent<MeshRenderer>();

        if (meshFilter == null || mr == null) return; // 부모에 MeshFilter나 MeshRenderer가 없는 경우 반환

        string meshFileName = meshFilter.sharedMesh.name;

        Debug.Log("메쉬 렌더러 머테리얼 변경~~~~~~~~~~~~~~~~");
        mr.material = meshFileName switch
        {
            "m_ingredients_chicken_sliced_01_0" => friedMaterials[0],
            "m_ingredients_meat_sliced_01_0" => friedMaterials[1],
            _ => mr.material
        };
    }
}
