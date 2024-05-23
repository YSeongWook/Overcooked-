using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Transform = UnityEngine.Transform;

public class Player2InteractController : MonoBehaviour
{
    // 상태 변화 패턴
    private IPlayerState currentState;
    private FreeState freeState;
    private HoldState holdState;

    // 애니메이션
    public Animator anim;

    // 상호작용 할 수 있는 오브젝트
    public GameObject interactObject;
    public ObjectHighlight objectHighlight;
    public GameObject nextInteractObject;

    // 변경되는 Bool값
    [SerializeField] public bool isHolding = false;
    // 재료를 제외한 모든 오브젝트 활성화 확인값
    [SerializeField] public bool canActive = false;

    // 던지는 힘
    [SerializeField] private Vector3 throwPower;

    [Header("Grab Object Control")]
    [SerializeField] private GameObject idleR;
    [SerializeField] private GameObject idleL;
    [SerializeField] private GameObject grabR;
    [SerializeField] private GameObject grabL;
    [SerializeField] private GameObject knife;

    [Header("PlayerInputSystem")]
    [SerializeField] private GameObject PlayerInputSystem;


    private void Awake()
    {
        //freeState = new FreeState(this);
        //holdState = new HoldState(this);
        //currentState = freeState;  // 초기 상태 설정
    }

    private void Update()
    {
        SetHand();
    }

    #region OnSwitch
    public void OnSwitch(InputValue inputValue)
    {
        PlayerInputSystem.GetComponent<PlayerMasterController>().SwitchPlayerComponent();
    }
    #endregion

    #region CatchOrKnockback,CookOrThrow, PickupOrPlace
    public void CatchOrKnockback()
    {
    }

    public void CookOrThrow()
    {
    }

    public void PickupOrPlace()
    {

    }
    #endregion

    #region OnCookOrThrow
    public void OnCookOrThrow(InputValue inputValue)
    {
        Debug.Log("OnCookOrThrow");
        if (checkInteractObject())
        {
            if (ShouldStartCutting())
                StartCuttingProcess();
            else
                SoundManager.Instance.PlayEffect("no");
        }
        else
        {
            if (isHolding && CanThrowIngredient())
            {
                ThrowIngredient();
            }
        }
    }

    bool checkInteractObject()
    {
        if (interactObject != null)
        {
            if (interactObject.GetComponent<ObjectHighlight>().objectType == ObjectHighlight.ObjectType.Ingredient)
                return false;
            else
                return true;
        }
        return false;
    }

    bool ShouldStartCutting()
    {
        return objectHighlight.objectType == ObjectHighlight.ObjectType.Board &&
               interactObject.transform.parent.childCount > 2 &&
               !interactObject.transform.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>().isCooked &&
               !isHolding &&
                interactObject.transform.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>().type != Ingredient.IngredientType.Rice &&
                interactObject.transform.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>().type != Ingredient.IngredientType.SeaWeed &&
                interactObject.transform.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient>().type != Ingredient.IngredientType.Tortilla;
    }

    void StartCuttingProcess()
    {
        var cuttingBoard = interactObject.transform.GetChild(0).GetComponent<CuttingBoard>();

        if (cuttingBoard._CoTimer == null) // 한번도 실행 안된거면 시작 가능
        {
            anim.SetTrigger("startCut");
            cuttingBoard.Pause = false;
            cuttingBoard.CuttingTime = 0;
            cuttingBoard.StartCutting1();
        }
        else if (cuttingBoard.Pause) // 실행되다 만거라면
        {
            anim.SetTrigger("startCut");
            cuttingBoard.PauseSlider(false);
        }
    }

    bool CanThrowIngredient()
    {
        return transform.GetChild(1).GetComponent<Ingredient>() == null;
    }

    void ThrowIngredient()
    {
        PlayThrowSound();
        SetThrowAnimation();
        UpdateHoldingStatus(false);
        AdjustIngredientPosition();
        ApplyThrowForce();
        EnableIngredientCollision();
        ReleaseIngredient();
    }

    void PlayThrowSound()
    {
        SoundManager.Instance.PlayEffect("throwItem");
    }

    void SetThrowAnimation()
    {
        anim.SetTrigger("throw");
    }

    void UpdateHoldingStatus(bool status)
    {
        isHolding = status;
        anim.SetBool("isHolding", isHolding);
    }

    void AdjustIngredientPosition()
    {
        Transform ingredient = GetIngredientTransform();
        ingredient.localPosition += new Vector3(0, 0.3f, 0);
    }

    Transform GetIngredientTransform()
    {
        return transform.GetChild(1).GetChild(0);
    }

    void ApplyThrowForce()
    {
        Transform ingredient = GetIngredientTransform();
        Rigidbody ingreRigid = ingredient.GetComponent<Rigidbody>();
        Vector3 dir = transform.TransformDirection(throwPower);
        ingreRigid.AddForce(dir, ForceMode.Impulse);
    }

    void EnableIngredientCollision()
    {
        Transform ingredient = GetIngredientTransform();
        MeshCollider ingreCollider = ingredient.GetComponent<MeshCollider>();
        ingreCollider.isTrigger = false;
        Rigidbody ingreRigid = ingredient.GetComponent<Rigidbody>();
        ingreRigid.constraints = RigidbodyConstraints.None;
    }

    void ReleaseIngredient()
    {
        transform.GetChild(1).SetParent(transform.parent);
    }
    #endregion

    #region OnPickupOrPlace
    public void OnPickupOrPlace(InputValue inputValue)
    {
        ProcessInteraction();
        //SetHand();
    }

    private void ProcessInteraction()
    {
        if (interactObject == null && !isHolding) return;

        if (isHolding && objectHighlight == null)
        {
            // 뭔가 들고있고 앞에 상호작용 객체가 없을때
            HoldingItemDropObject();
            return;
        }

        Debug.Log($"objectHighlight.objectType : {objectHighlight.objectType}");
        switch (objectHighlight.objectType)
        {
            case ObjectHighlight.ObjectType.CounterTop:
            case ObjectHighlight.ObjectType.Board:
            case ObjectHighlight.ObjectType.Return:
                HandleCounterTopOrBoardInteraction();
                break;
            case ObjectHighlight.ObjectType.Craft:
                HandleCraftInteraction();
                break;
            case ObjectHighlight.ObjectType.Bin:
                HandleBinInteraction();
                break;
            case ObjectHighlight.ObjectType.Station:
                HandleStationInteraction();
                break;
            case ObjectHighlight.ObjectType.Oven:
                HandleOvenInteraction();
                break;
            default:
                HandleGeneralObjectInteraction();
                break;
        }
    }

    private void HandleOvenInteraction()
    {
        // Ingredient 컴포넌트가 존재하고, 그 타입이 Plate인지 확인
        if (isHolding && transform.GetChild(1).gameObject.GetComponent<Ingredient>() != null
            && transform.GetChild(1).gameObject.GetComponent<Ingredient>().type == Ingredient.IngredientType.Plate)
        {
            GameObject plateComponent = transform.GetChild(1).gameObject;  // Plates Object
            GameObject Oven = objectHighlight.transform.parent.gameObject;
            bool isDough = plateComponent.transform.GetChild(9).gameObject.activeSelf;
            if (Oven.transform.childCount == 2 && !plateComponent.transform.GetComponent<Ingredient>().pizzazIsCooked && isDough)
            {
                plateComponent.transform.SetParent(Oven.transform);

                // 위치 설정
                plateComponent.transform.localPosition = new Vector3(0f, 0.01f, 0f);

                // 회전 설정
                plateComponent.transform.localRotation = Quaternion.Euler(0f, 2.281f, 0f);

                // 크기 조정
                plateComponent.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

                Oven.GetComponent<Oven>().StartCooking();

                isHolding = false;
                anim.SetBool("isHolding", isHolding);
            }
            else
                SoundManager.Instance.PlayEffect("no");
        }
        else if (!isHolding)
        {
            // 다시 꺼냄. 근데 요리중일땐 못꺼냄.
            GameObject Oven = objectHighlight.transform.parent.gameObject;
            GameObject plateComponent = Oven.transform.GetChild(2).gameObject;

            plateComponent.transform.SetParent(transform);

            // 위치 설정
            plateComponent.transform.localPosition = new Vector3(-0.4100023f, 0.4699999f, 1.840027f);

            // 회전 설정
            plateComponent.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            // 크기 조정
            plateComponent.transform.localScale = new Vector3(312.5f, 312.5f, 312.5f);

            isHolding = true;
            anim.SetBool("isHolding", isHolding);
        }
    }

    // Station
    private void HandleStationInteraction()
    {
        if (isHolding && transform.GetChild(1).gameObject.GetComponent<Ingredient>() != null
            && transform.GetChild(1).gameObject.GetComponent<Ingredient>().type == Ingredient.IngredientType.Plate)
        {
            // Ingredient 컴포넌트가 존재하고, 그 타입이 Plate인지 확인
            Plates plateComponent = transform.GetChild(1).gameObject.GetComponent<Plates>();  // Plates 컴포넌트를 가져옴

            if (GameManager.instance.CheckMenu(plateComponent.containIngredients))
            {
                // 접시의 재료가 메뉴와 일치하면
                SoundManager.Instance.PlayEffect("right");  // 성공 효과음 재생
                GameManager.instance.MakeOrder();  // 주문을 만듦
            }
            else
            {
                // 접시의 재료가 메뉴와 일치하지 않으면
                SoundManager.Instance.PlayEffect("no");  // 실패 효과음 재생
                //TriggerFailureEffect();  // 실패 시 빨간색 불 들어오는 함수 호출 (추가 구현 필요)
            }

            Destroy(transform.GetChild(1).gameObject);  // 접시 전체를 삭제 (추후 재활용을 고려)
            isHolding = false;  // 아이템을 들고 있는 상태를 해제
            anim.SetBool("isHolding", isHolding);  // 애니메이션 상태를 업데이트
            GameManager.instance.PlateReturn();  // 접시 반환 처리
        }

    }

    // CounterTop, Board
    private void HandleCounterTopOrBoardInteraction()
    {

        if (canActive && isHolding && !objectHighlight.onSomething)
        {
            // 내가 뭘 들고있고, 테이블이나 찹핑테이블 위에 없을떄
            TablePlaceOrDropObject(false);
        }
        else if (canActive && isHolding && objectHighlight.onSomething)
        {
            // 내가 뭘 들고있고, 테이블이나 찹핑테이블 리턴 위에 있을때
            Debug.Log("뭔가 있냐?");
            TablePlaceOrDropObject(true);
        }
        else if (canActive && interactObject.GetComponent<ObjectHighlight>().onSomething)
        {
            GameObject handleThing = interactObject.transform.parent.GetChild(2).gameObject; ;

            if (interactObject.transform.parent.GetChild(2).name.Equals("PFX_PanFire"))
            {
                handleThing = interactObject.transform.parent.GetChild(3).gameObject;
            }

            if (handleThing.CompareTag("Ingredient") && objectHighlight.objectType == ObjectHighlight.ObjectType.Board &&
                interactObject.transform.GetChild(0).GetComponent<CuttingBoard>().cookingBar.IsActive())
            {
                // 손질 중인 재료는 집을 수 없음
            }
            else
            {
                //Debug.Log($"handleThing.name : {handleThing.name}");

                //화덕에서 onSomething을 끄지않고 꺼내야함.

                TryPickupObject(handleThing);
            }
        }
    }
    // Craft
    private void HandleCraftInteraction()
    {
        Debug.Log($"objectHighlight.onSomething : {objectHighlight.onSomething}");
        // Craft 위에 뭔가 없을때
        if (!objectHighlight.onSomething)
        {
            if (!isHolding)
            {
                PickupFromCraft();
            }
            else
            {
                //PlaceOrDropObject(false);
            }
        }
        else if (!isHolding) // 테이블 위에 뭔가 있는데, 손에 든게 아무것도 없을때
        {
            PickupFromCraft();
        }
    }
    // Bin
    private void HandleBinInteraction()
    {
        if (isHolding && transform.GetChild(1).GetComponent<Ingredient>()?.type == Ingredient.IngredientType.Plate)
        {
            DisposePlate();
        }
        else if (transform.childCount > 1)
        {
            DisposeObject();
        }
    }

    private void HandleGeneralObjectInteraction()
    {
        if (!isHolding && interactObject.CompareTag("Ingredient"))
        {
            PickupIngredient();
        }
        else if (!isHolding && interactObject.CompareTag("Plate"))
        {
            PickupPlate();
        }
        else if (!isHolding && (interactObject.CompareTag("Pot") || interactObject.CompareTag("Pan")))
        {
            PickupPot();
        }
    }

    void HoldingItemDropObject()
    {
        // 떨어트리기 로직 상세 구현 필요
        GameObject handlingThing = transform.GetChild(1).gameObject;
        // 객체를 내려놓을 때의 로직
        Debug.Log($"handlingThing name : {handlingThing.name}");

        if (handlingThing.CompareTag("Plate"))
        {
            // 떨구는 객체가 접시면.
            //Debug.Log("접시 내려");
            handlingThing.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        else if (handlingThing.CompareTag("Pot") || handlingThing.CompareTag("Pan"))
        {
            handlingThing.transform.GetComponent<BoxCollider>().isTrigger = false;
            handlingThing.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
        else
        {
            handlingThing.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            handlingThing.transform.GetChild(0).GetComponent<MeshCollider>().isTrigger = false;
        }

        handlingThing.transform.SetParent(null); // 부모 설정 해제
        anim.SetBool("isHolding", false);
        isHolding = false;
    }

    private void TryPickupObject(GameObject handleThing)
    {
        SoundManager.Instance.PlayEffect("take");
        objectHighlight.onSomething = false;
        //isHolding = true;
        //anim.SetBool("isHolding", isHolding);
        HandleObject(handleThing);
    }

    // 내가 뭔가를 들고있을때
    // true 테이블 위에 뭔가 있음 , false 테이블 위에 뭔가 없음
    private void TablePlaceOrDropObject(bool drop)
    {
        SoundManager.Instance.PlayEffect(drop ? "put" : "put");
        if (drop)
        {
            // true 테이블 위에 뭔가 있는데 내가 가진게 접시고, 음식이면 담음
            if (CanPlaceIngredient())
            {
                PlaceIngredient();
            }
            else
            {
                SoundManager.Instance.PlayEffect("no");
            }

            if (objectHighlight.transform.parent.GetChild(2) != null)
            {
                GameObject ingredientObj = transform.GetChild(1).gameObject;
                var ingredient = ingredientObj.transform.GetChild(0).GetChild(0).GetComponent<Ingredient>().type;

                //Debug.Log($"ingredientObj : {ingredientObj.name}");
                //Debug.Log($"objectHighlight.transform.parent.GetChild(2).gameObject : {objectHighlight.transform.parent.GetChild(2).gameObject.name}");

                GameObject potAndPan = objectHighlight.transform.parent.GetChild(2).gameObject;
                if (objectHighlight.transform.parent.GetChild(2).name.Equals("PFX_PanFire"))
                    potAndPan = objectHighlight.transform.parent.GetChild(3).gameObject;

                // 테이블에 있는게, Pan이고 내가 든게 미트, 닭고기면 실행
                if (potAndPan.CompareTag("Pan") && (ingredient == Ingredient.IngredientType.Meat || ingredient == Ingredient.IngredientType.Chicken))
                {
                    ingredientObj.transform.SetParent(potAndPan.transform);
                    // 위치 설정
                    ingredientObj.transform.localPosition = new Vector3(2e-05f, -0.00017f, 0.00013f);
                    // 회전 설정
                    ingredientObj.transform.localRotation = Quaternion.Euler(0f, -168.905f, 0f);

                    potAndPan.GetComponent<PanOnStove>().inSomething = true;

                    anim.SetBool("isHolding", false);
                    isHolding = false;
                }
                else
                {
                    SoundManager.Instance.PlayEffect("no");
                }

                // 테이블에 있는게, Pot이고 내가 든게 쌀이고 화덕이면 Pot에 붙이기
                if (potAndPan.CompareTag("Pot") && ingredient == Ingredient.IngredientType.Rice)
                {
                    ingredientObj.transform.SetParent(potAndPan.transform);
                    // 위치 설정
                    ingredientObj.transform.localPosition = new Vector3(2e-05f, -0.00017f, 0.00013f);
                    // 회전 설정
                    ingredientObj.transform.localRotation = Quaternion.Euler(0f, -168.905f, 0f);

                    potAndPan.GetComponent<PotOnStove>().inSomething = true;

                    anim.SetBool("isHolding", false);
                    isHolding = false;
                }
                else
                {
                    SoundManager.Instance.PlayEffect("no");
                }
            }

        }
        else
        {
            // false 테이블 위에 뭔가 없음 => 놓기.
            GameObject handlingThing = transform.GetChild(1).gameObject;
            Debug.Log($"handlingThing.name : {handlingThing.name}");
            HandleObject(handlingThing, false);
        }
    }



    private bool CanPlaceIngredient()
    {
        //Debug.Log($"canActive : {canActive}");
        //Debug.Log($"objectHighlight.onSomething : {objectHighlight.onSomething}");
        //Debug.Log($"interactObject.transform.parent.childCount > 2 : {interactObject.transform.parent.childCount > 2}");
        //Debug.Log($"IsPlate(interactObject.transform.parent.GetChild(2)) : {IsPlate(interactObject.transform.parent.GetChild(2))}");
        //Debug.Log($"isHolding: {isHolding}");
        //Debug.Log($"IsHoldingCookedIngredient() : {IsHoldingCookedIngredient()}");

        return canActive && objectHighlight.onSomething
               && interactObject.transform.parent.childCount > 2
               && IsPlate(interactObject.transform.parent.GetChild(2))
               && isHolding
               && IsHoldingCookedIngredient();
    }

    private bool IsPlate(UnityEngine.Transform obj)
    {
        var handle = obj.GetComponent<Ingredient>();
        return handle != null && handle.type == Ingredient.IngredientType.Plate;
    }

    private bool IsHoldingCookedIngredient()
    {
        var holdingObj = transform.GetChild(1).GetChild(0);
        if (holdingObj.childCount > 0)
        {
            var handle = holdingObj.GetChild(0).GetComponent<Ingredient>();
            bool checkisCooked = handle.isCooked;
            // 김은 조리 안되어도 접시 올라감
            if (handle.type == Ingredient.IngredientType.SeaWeed ||
                handle.type == Ingredient.IngredientType.Tortilla)
            {
                checkisCooked = true;
            }

            // 미트 치킨은 조리안하면 못올라감
            //if (handle.type == Ingredient.IngredientType.Meat ||
            //    handle.type == Ingredient.IngredientType.Chicken)
            //{
            //    checkisCooked = false;
            //}

            return handle != null && checkisCooked;
        }
        return false;
    }

    private void PlaceIngredient()
    {
        var plate = interactObject.transform.parent.GetChild(2).GetComponent<Plates>();
        var ingredient = transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Ingredient>().type;
        if (plate.AddIngredient(ingredient))
        {
            SoundManager.Instance.PlayEffect("put");
            plate.InstantiateUI();
            Destroy(transform.GetChild(1).gameObject);
            isHolding = false;
            anim.SetBool("isHolding", false);
        }
    }

    private void HandleObject(GameObject obj, bool isPickingUp = true)
    {
        if (isPickingUp)
        {
            // 객체를 들어 올릴 때의 로직
            Debug.Log("올려");
            if (obj.CompareTag("Plate"))
            {
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                obj.transform.SetParent(transform); // 플레이어의 하위 객체로 설정
                // 플레이어에서 위치 잡기
                SetPositionbetweenPlayerandObject(obj);
                anim.SetBool("isHolding", true);
                isHolding = true;
            }
            else if (obj.CompareTag("Pan") || obj.CompareTag("Pot"))
            {
                //인데 Ingredient의 IsCooked = true이면 재료꺼내기.
                if (obj.transform.childCount == 3 && obj.transform.GetChild(2).transform.GetChild(0).GetChild(0).GetComponent<Ingredient>().isCooked)
                {
                    //포트는 놔둠
                    objectHighlight.onSomething = true;
                    Debug.Log($"obj : {obj.name}");

                    GameObject cookedIngredientObj = obj.transform.GetChild(2).gameObject;
                    cookedIngredientObj.transform.SetParent(transform); // 플레이어의 하위 객체로 설정
                    cookedIngredientObj.transform.GetChild(0).GetChild(0).GetComponent<Ingredient>().HandleIngredient(transform, cookedIngredientObj.transform.GetChild(0).GetChild(0).GetComponent<Ingredient>().type, true);

                    anim.SetBool("isHolding", true);
                    isHolding = true;
                }
                else
                {
                    obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    obj.transform.GetComponent<Ingredient>().HandleIngredient(transform, obj.transform.GetComponent<Ingredient>().type, true);
                    objectHighlight.onSomething = false;

                    obj.transform.SetParent(transform); // 플레이어의 하위 객체로 설정
                    // 플레이어에서 위치 잡기
                    SetPositionbetweenPlayerandObject(obj);
                    anim.SetBool("isHolding", true);
                    isHolding = true;
                }
            }
            else
            {
                obj.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                obj.transform.GetChild(0).GetChild(0).GetComponent<Ingredient>().HandleIngredient(transform, obj.transform.GetChild(0).GetChild(0).GetComponent<Ingredient>().type, true);

                obj.transform.SetParent(transform); // 플레이어의 하위 객체로 설정
                // 플레이어에서 위치 잡기
                SetPositionbetweenPlayerandObject(obj);
                anim.SetBool("isHolding", true);
                isHolding = true;
            }
        }
        else
        {
            // 객체를 내려놓을 때의 로직
            //Debug.Log("내려");
            //obj.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //obj.transform.SetParent(null); // 부모 설정 해제
            //anim.SetBool("isHolding", false);
            //isHolding = false;
            SoundManager.Instance.PlayEffect("put");
            GameObject handleThing = transform.GetChild(1).gameObject;
            if (handleThing.CompareTag("Ingredient"))
            {
                objectHighlight.onSomething = true;
                isHolding = false;
                handleThing.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient>().isOnDesk = true;
                handleThing.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient>().
                    IngredientHandleOff(
                    interactObject.transform.parent,
                    interactObject.transform.parent.GetChild(1).localPosition,
                    handleThing.transform.GetChild(0).GetChild(0).GetComponent<Ingredient>().type);
            }
            else if (handleThing.CompareTag("Pan") || handleThing.CompareTag("Pot"))
            {
                if (objectHighlight.objectType != ObjectHighlight.ObjectType.Board)
                {
                    objectHighlight.onSomething = true;
                    isHolding = false;
                    handleThing.GetComponent<Ingredient>().isOnDesk = true;
                    handleThing.GetComponent<Ingredient>().
                        PlayerHandleOff(interactObject.transform.parent,
                        interactObject.transform.parent.GetChild(1).localPosition);
                }
            }
            else //접시
            {
                if (objectHighlight.objectType != ObjectHighlight.ObjectType.Board)
                {
                    objectHighlight.onSomething = true;
                    isHolding = false;
                    handleThing.GetComponent<Ingredient>().isOnDesk = true;
                    handleThing.GetComponent<Ingredient>().
                        PlayerHandleOff(interactObject.transform.parent,
                        interactObject.transform.parent.GetChild(1).localPosition);
                }

            }
            anim.SetBool("isHolding", isHolding);
        }
    }

    private void PickupFromCraft()
    {
        SoundManager.Instance.PlayEffect("take");
        // Craft에서 아이템 꺼내기 로직 구현
        interactObject.GetComponent<Craft>().OpenCraftPlayer2();
        objectHighlight.onSomething = false;
        isHolding = true;
        anim.SetBool("isHolding", isHolding);
    }

    private void DisposePlate()
    {
        transform.GetChild(1).gameObject.GetComponent<Plates>().ClearIngredient();
    }

    private void DisposeObject()
    {
        Destroy(transform.GetChild(1).gameObject);
        isHolding = false;
        anim.SetBool("isHolding", isHolding);
    }

    private void PickupIngredient()
    {
        SoundManager.Instance.PlayEffect("take");
        isHolding = true;
        anim.SetBool("isHolding", isHolding);
        // 재료 줍기 로직 상세 구현 필요
        GameObject ingredientObj = interactObject.transform.parent.gameObject;
        ingredientObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        ingredientObj.transform.GetChild(0).GetComponent<Ingredient>().HandleIngredient(transform, ingredientObj.transform.GetChild(0).GetComponent<Ingredient>().type, true);
    }

    private void PickupPlate()
    {
        //isHolding = true;
        //anim.SetBool("isHolding", isHolding);
        // 접시인식.
        GameObject plateObject = interactObject.transform.parent.gameObject;
        TryPickupObject(plateObject);
        //plateObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //SetPositionbetweenPlayerandObject(plateObject);
    }

    private void PickupPot()
    {
        SoundManager.Instance.PlayEffect("take");
        isHolding = true;
        anim.SetBool("isHolding", isHolding);
        GameObject ingredientObj = interactObject.transform.parent.gameObject;
        ingredientObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        ingredientObj.transform.GetComponent<Ingredient>().HandleIngredient(transform, ingredientObj.transform.GetComponent<Ingredient>().type, true);
    }

    #endregion

    #region OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("deadZone"))
        //{
        //    HandleDeadZone();
        //    return;
        //}

        if (CheckForIngredientHandling(other))
            return;

        HandleActiveObjectInteraction(other);
    }

    private void HandleDeadZone()
    {
        // 사운드 메니져
        SoundManager.Instance.PlayEffect("fall");
        //DieRespawn();
    }

    private bool CheckForIngredientHandling(Collider other)
    {
        // 손에 잡고있고, interactObject가 Null이 아니고, 그 태그가 Ingredient일때 True
        // 손에 없거나, interactObject가 Null이거나, 그 태그가 Ingredient가 아니면 False -> HandlePickupIngredient
        if (interactObject != null && interactObject.CompareTag("Ingredient") && isHolding)
        {
            DeactivateObjectHighlight();
            ResetinteractObjects();
            return true;
        }

        return HandlePickupIngredient(other);
    }

    private void ResetinteractObjects()
    {
        interactObject = null;
        objectHighlight = null;
    }

    private void DeactivateObjectHighlight()
    {
        interactObject.GetComponent<ObjectHighlight>().DeactivateHighlight(interactObject.GetComponent<Ingredient>().isCooked);
    }

    private bool HandlePickupIngredient(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            if (interactObject == null && !other.GetComponent<Ingredient>().isOnDesk)
            {
                SetinteractObject(other.gameObject);
                other.GetComponent<ObjectHighlight>().ActivateHighlight(other.GetComponent<Ingredient>().isCooked);
                return true;
            }
        }
        return false;
    }

    private void SetinteractObject(GameObject obj)
    {
        interactObject = obj;
        objectHighlight = obj.GetComponent<ObjectHighlight>();
    }

    private void HandleActiveObjectInteraction(Collider other)
    {
        if (other == interactObject || interactObject != null)
            return;

        if (other.GetComponent<ObjectHighlight>() == null)
            return;

        canActive = true;

        SetinteractObject(other.gameObject);
        other.GetComponent<ObjectHighlight>().activeObject = true;

        if (other.CompareTag("Ingredient"))
        {
            other.GetComponent<ObjectHighlight>().ActivateHighlight(other.GetComponent<Ingredient>().isCooked);
        }
        else
        {
            other.GetComponent<ObjectHighlight>().ActivateHighlight(other.GetComponent<ObjectHighlight>().onSomething);
        }

        if (interactObject.GetComponent<ObjectHighlight>().objectType == ObjectHighlight.ObjectType.Board)
        {
            anim.SetBool("canCut", true);
        }
    }
    #endregion

    #region OnTriggerStay
    private void OnTriggerStay(Collider other)
    {
        if (ShouldReturnEarly(other))
            return;

        if (HandleActiveIngredientSwitch(other))
            return;

        InitializeActiveObjectIfNull(other);
    }

    private bool ShouldReturnEarly(Collider other)
    {
        return (other.CompareTag("Ingredient") || other.CompareTag("Plate") || other.CompareTag("Pan") || other.CompareTag("Pot")) && isHolding;
    }

    private bool HandleActiveIngredientSwitch(Collider other)
    {
        if (interactObject != null && interactObject.CompareTag("Ingredient") && isHolding)
        {
            if (nextInteractObject != null)
            {
                SwitchActiveObject();
            }
            else
            {
                ClearActiveObjects();
            }
            return true;
        }
        return false;
    }

    private void SwitchActiveObject()
    {
        interactObject.GetComponent<ObjectHighlight>().DeactivateHighlight(interactObject.GetComponent<Ingredient>().isCooked);
        interactObject = nextInteractObject;
        objectHighlight = interactObject.GetComponent<ObjectHighlight>();
        interactObject.GetComponent<ObjectHighlight>().DeactivateHighlight(interactObject.GetComponent<Ingredient>().isCooked);
        nextInteractObject = null;
    }

    private void ClearActiveObjects()
    {
        interactObject.GetComponent<ObjectHighlight>().DeactivateHighlight(interactObject.GetComponent<Ingredient>().isCooked);
        ResetinteractObjects();
    }

    private void InitializeActiveObjectIfNull(Collider other)
    {
        if (interactObject == null)
        {
            canActive = true;
            if (other.GetComponent<ObjectHighlight>() != null)
            {
                SetinteractObject(other.gameObject);
                other.GetComponent<ObjectHighlight>().activeObject = true;
                HighlightBasedOnTag(other);
            }
        }
    }

    private void HighlightBasedOnTag(Collider other)
    {
        bool highlightState = other.CompareTag("Ingredient") ? other.GetComponent<Ingredient>().isCooked : true;
        other.GetComponent<ObjectHighlight>().ActivateHighlight(highlightState);
    }
    #endregion

    #region OnTriggerExit
    private void OnTriggerExit(Collider other)
    {
        if (ShouldDeactivateObjects())
        {
            HandleBoardInteraction();
            DeactivateObjects();
        }
        else if (ShouldSwitchToNextObject(other))
        {
            SwitchActiveToNextObject();
        }
        else
        {
            ClearNextObject();
        }
    }

    private bool ShouldDeactivateObjects()
    {
        return interactObject != null && nextInteractObject == null;
    }

    private bool ShouldSwitchToNextObject(Collider other)
    {
        return other.gameObject == interactObject;
    }

    private void HandleBoardInteraction()
    {
        if (interactObject.GetComponent<ObjectHighlight>().objectType == ObjectHighlight.ObjectType.Board)
        {
            anim.SetBool("canCut", false);
            interactObject.transform.GetChild(0).GetComponent<CuttingBoard>().PauseSlider(true);
        }
    }

    private void DeactivateObjects()
    {
        //Debug.Log($"DeactivateObjects : {canActive}");
        canActive = false;
        OffHighlightCurrentObject();
        interactObject = null;
        objectHighlight = null;
    }

    private void SwitchActiveToNextObject()
    {
        anim.SetBool("canCut", false);
        OffHighlightCurrentObject();
        interactObject = nextInteractObject;
        objectHighlight = interactObject?.GetComponent<ObjectHighlight>();
        nextInteractObject = null;
        OnHighlightCurrentObject();
    }

    private void ClearNextObject()
    {
        nextInteractObject = null;
    }

    private void OffHighlightCurrentObject()
    {
        if (interactObject != null && interactObject.GetComponent<ObjectHighlight>() != null)
        {
            bool highlightState = interactObject.CompareTag("Ingredient") ? interactObject.GetComponent<Ingredient>().isCooked : true;
            interactObject.GetComponent<ObjectHighlight>().DeactivateHighlight(highlightState);
        }
    }

    private void OnHighlightCurrentObject()
    {
        if (interactObject != null && interactObject.GetComponent<ObjectHighlight>() != null)
        {
            bool highlightState = interactObject.CompareTag("Ingredient") ? interactObject.GetComponent<Ingredient>().isCooked : true;
            interactObject.GetComponent<ObjectHighlight>().DeactivateHighlight(highlightState);
        }
    }
    #endregion

    #region SetPosition
    private void SetHand()
    {
        if (isHolding) //뭘 집었다면 손 접기
        {
            knife.SetActive(false);
            idleL.SetActive(false);
            idleR.SetActive(false);
            grabL.SetActive(true);
            grabR.SetActive(true);
        }
        else
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("New_Chef@Chop")) //다지는 중이면
            {
                knife.SetActive(true);
                idleL.SetActive(false);
                idleR.SetActive(false);
                grabL.SetActive(true);
                grabR.SetActive(true);
            }
            else
            {
                knife.SetActive(false);
                idleL.SetActive(true);
                idleR.SetActive(true);
                grabL.SetActive(false);
                grabR.SetActive(false);
            }
        }
    }

    void SetPositionbetweenPlayerandObject(GameObject obj)
    {
        //string name = obj.name;

        Vector3 localPosition = Vector3.zero;
        Quaternion localRotation = Quaternion.identity;

        if (obj.CompareTag("Plate"))
        {
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localPosition = new Vector3(-0.409999996f, 0.4700001f, 1.84000003f);
        }

        //switch (name)
        //{
        //    case "Plate":
        //        //obj.GetComponent<Ingredient>().HandleIngredient(obj.transform, obj.transform.GetComponent<Ingredient>().type, true);
        //        //Transform parentTransform = obj.transform.parent;
        //        //parentTransform.localPosition = localPosition;
        //        //parentTransform.localRotation = localRotation;
        //        //parentTransform.parent.SetParent(something);
        //        obj.transform.localRotation = Quaternion.identity;
        //        obj.transform.localPosition = new Vector3(-0.409999996f, 0.4700001f, 1.84000003f);
        //        break;
        //    default:
        //        break;
        //}
    }
    #endregion

}
