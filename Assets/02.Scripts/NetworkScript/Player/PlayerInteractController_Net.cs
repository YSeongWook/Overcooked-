using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Transform = UnityEngine.Transform;

public class PlayerInteractController_Net : NetworkBehaviour
{
    public Sprite[] Icons;

    //public GameObject Canvas;

    //public GameObject madeUI;

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

    //[Header("PlayerInputSystem")]
    //[SerializeField] private GameObject PlayerInputSystem;
    //private PlayerMasterController2 masterController;

    [Header("Mobile Button")]
    public Button pickupButton; // 모바일 줍기/놓기 버튼
    public Button cookButton;   // 모바일 요리/던지기 버튼

    Vector3 placeTransform = Vector3.zero;

    //private void Awake()
    //{
    //    masterController = PlayerInputSystem.GetComponent<PlayerMasterController2>();
    //
    //    if (pickupButton != null && masterController.currentPlayer == this.gameObject)
    //    {
    //        pickupButton.onClick.AddListener(MobilePickupOrPlace); // 버튼 클릭 이벤트에 MobileCookOrThrow 메서드 연결
    //    }
    //
    //    if (cookButton != null && masterController.currentPlayer == this.gameObject)
    //    {
    //        cookButton.onClick.AddListener(MobileCookOrThrow); // 버튼 클릭 이벤트에 MobileCookOrThrow 메서드 연결
    //    }
    //}
    public int coin_;
    private void Update()
    {
        if (isLocalPlayer)
        {
            int localCoin = GameManager_Net.instance.Coin;
            if (localCoin != Player1Money)
            {
                CmdUpdatePlayerMoney(localCoin);
            }
        }

        if (isLocalPlayer)
            SetHand();

        if (GameManager_Net.instance.isDone)
        {
            StartCoroutine(StartResult());
        }
    }

    IEnumerator StartResult()
    {
        yield return new WaitForSecondsRealtime(2f);
        GameObject.Find("Canvas").SetActive(false);
        UIManager.Instance.battleResultUI.SetActive(true);
        UIManager.Instance.battleResultUI.GetComponent<BattleResultText>().targetRedTotalNum = Player1Money;
        UIManager.Instance.battleResultUI.GetComponent<BattleResultText>().targetBlueTotalNum = Player2Money;
    }

    [SyncVar(hook = nameof(OnPlayer1MoneyChanged))]
    public int Player1Money; // 플레이어 1의 돈

    [SyncVar(hook = nameof(OnPlayer2MoneyChanged))]
    public int Player2Money; // 플레이어 2의 돈

    [Command]
    private void CmdUpdatePlayerMoney(int amount)
    {
        Player1Money = amount;

        // 서버에서 각 클라이언트의 Player2Money를 업데이트합니다.
        RpcUpdateSelfMoney(amount);
        RpcUpdateOpponentMoney(amount);
    }

    [ClientRpc]
    private void RpcUpdateSelfMoney(int amount)
    {
        if (isLocalPlayer)
        {
            GameManager_Net.instance.Coin = amount;
        }
    }

    [ClientRpc]
    private void RpcUpdateOpponentMoney(int amount)
    {
        if (!isLocalPlayer)
        {
            Player2Money = amount;
            UpdateOpponentMoneyUI(Player2Money);
        }
    }

    private void OnPlayer1MoneyChanged(int oldMoney, int newMoney)
    {
        if (isLocalPlayer)
        {
            GameManager_Net.instance.Coin = newMoney;
        }
    }

    private void OnPlayer2MoneyChanged(int oldMoney, int newMoney)
    {
        if (!isLocalPlayer)
        {
            UpdateOpponentMoneyUI(newMoney);
        }
    }

    private void UpdateOpponentMoneyUI(int amount)
    {
        GameManager_Net.instance.OppositeUI.transform.GetChild(0).GetComponent<Text>().text = amount.ToString();
    }

    #region OnSwitch
    //public void OnSwitch(InputValue inputValue)
    //{
    //    PlayerInputSystem.GetComponent<PlayerMasterController2>().SwitchPlayerComponent();
    //}
    #endregion

    #region OnCookOrThrow
    public void OnCookOrThrow(InputValue inputValue)
    {
        Debug.Log("OnCookOrThrow");
        //Debug.Log(interactObject.transform.parent.name);
        if (isLocalPlayer)
        {
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
    }

    public void MobileCookOrThrow()
    {
        Debug.Log("MobileCookOrThrow");
        Debug.Log(interactObject);
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
               !interactObject.transform.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().isCooked &&
               !isHolding &&
                interactObject.transform.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().type != Ingredient_Net.IngredientType.Rice &&
                interactObject.transform.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().type != Ingredient_Net.IngredientType.SeaWeed &&
                interactObject.transform.parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().type != Ingredient_Net.IngredientType.Tortilla;
    }

    void StartCuttingProcess()
    {
        var cuttingBoard = interactObject.transform.GetChild(0).GetComponent<CuttingBoard_Net>();

        if (cuttingBoard._CoTimer == null) // 한번도 실행 안된거면 시작 가능
        {
            anim.SetTrigger("startCut");
            cuttingBoard.Pause = false;
            cuttingBoard.CuttingTime = 0;
            cuttingBoard.StartCutting1(transform.gameObject);
        }
        else if (cuttingBoard.Pause) // 실행되다 만거라면
        {
            anim.SetTrigger("startCut");
            cuttingBoard.PauseSlider(false);
        }
    }

    bool CanThrowIngredient()
    {
        return transform.GetChild(1).GetComponent<Ingredient_Net>() == null;
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
        // ingreRigid.constraints = RigidbodyConstraints.None;
        ingreRigid.constraints = RigidbodyConstraints.FreezeRotationY;
    }

    void ReleaseIngredient()
    {
        transform.GetChild(1).SetParent(transform.parent);
    }
    #endregion

    #region OnPickupOrPlace
    public void OnPickupOrPlace(InputValue inputValue)
    {
        if (isLocalPlayer)
            ProcessInteraction();
        //SetHand();
    }

    public void MobilePickupOrPlace()
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
            if (isLocalPlayer)
            {
                CmdHoldingItemDropObject();
                return;
            }
        }

        Debug.Log($"objectHighlight.objectType : {objectHighlight.objectType}");
        switch (objectHighlight.objectType)
        {
            case ObjectHighlight.ObjectType.CounterTop:
            case ObjectHighlight.ObjectType.Board:
            case ObjectHighlight.ObjectType.Return:
                if (isLocalPlayer)
                    HandleCounterTopOrBoardInteraction();
                break;
            case ObjectHighlight.ObjectType.Craft:
                if (isLocalPlayer)
                    HandleCraftInteraction();
                break;
            case ObjectHighlight.ObjectType.Bin:
                if (isLocalPlayer)
                    HandleBinInteraction();
                break;
            case ObjectHighlight.ObjectType.Station:
                    HandleStationInteraction();
                break;
            case ObjectHighlight.ObjectType.Oven:
                if (isLocalPlayer)
                    HandleOvenInteraction();
                break;
            default:
                if (isLocalPlayer)
                    HandleGeneralObjectInteraction();
                break;
        }
    }

    [Command]
    private void CmdHoldingItemDropObject()
    {
        RpcHoldingItemDropObject();
    }

    [ClientRpc]
    private void RpcHoldingItemDropObject()
    {
        HoldingItemDropObject();
    }

    private void HandleOvenInteraction()
    {
        // Ingredient 컴포넌트가 존재하고, 그 타입이 Plate인지 확인
        if (isHolding && transform.GetChild(1).gameObject.GetComponent<Ingredient_Net>() != null
            && transform.GetChild(1).gameObject.GetComponent<Ingredient_Net>().type == Ingredient_Net.IngredientType.Plate)
        {
            GameObject plateComponent = transform.GetChild(1).gameObject;  // Plates Object
            GameObject Oven = objectHighlight.transform.parent.gameObject;
            bool isDough = plateComponent.transform.GetChild(9).gameObject.activeSelf;
            if (Oven.transform.childCount == 2 && !plateComponent.transform.GetComponent<Ingredient_Net>().pizzazIsCooked && isDough)
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
        if (isHolding && transform.GetChild(1).gameObject.GetComponent<Ingredient_Net>() != null
            && transform.GetChild(1).gameObject.GetComponent<Ingredient_Net>().type == Ingredient_Net.IngredientType.Plate)
        {
            // Ingredient 컴포넌트가 존재하고, 그 타입이 Plate인지 확인
            Plates_Net plateComponent = transform.GetChild(1).gameObject.GetComponent<Plates_Net>();  // Plates 컴포넌트를 가져옴

            if (GameManager_Net.instance.CheckMenu(plateComponent.containIngredients))
            {
                // 접시의 재료가 메뉴와 일치하면
                SoundManager.Instance.PlayEffect("right");  // 성공 효과음 재생
                GameManager_Net.instance.MakeOrder();  // 주문을 만듦
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
            GameManager_Net.instance.PlateReturn();  // 접시 반환 처리
        }

    }

    // CounterTop, Board
    private void HandleCounterTopOrBoardInteraction()
    {
        if (canActive && isHolding && !objectHighlight.onSomething)
        {
            // 내가 뭘 들고있고, 테이블이나 찹핑테이블 위에 없을떄
            CmdTablePlaceOrDropObject(false);
            //TablePlaceOrDropObject(false);
        }
        else if (canActive && isHolding && objectHighlight.onSomething)
        {
            // 내가 뭘 들고있고, 테이블이나 찹핑테이블 리턴 위에 있을때
            Debug.Log("뭔가 있냐?");
            CmdTablePlaceOrDropObject(true);
            //TablePlaceOrDropObject(true);
        }
        else if (canActive && interactObject.GetComponent<ObjectHighlight>().onSomething)
        {
            GameObject handleThing = interactObject.transform.parent.GetChild(2).gameObject; ;

            if (interactObject.transform.parent.GetChild(2).name.Equals("PFX_PanFire"))
            {
                handleThing = interactObject.transform.parent.GetChild(3).gameObject;
            }

            if (handleThing.CompareTag("Ingredient") && objectHighlight.objectType == ObjectHighlight.ObjectType.Board &&
                interactObject.transform.GetChild(0).GetComponent<CuttingBoard_Net>().cookingBar.IsActive())
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
        if (isHolding && transform.GetChild(1).GetComponent<Ingredient_Net>()?.type == Ingredient_Net.IngredientType.Plate)
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
            // Debug.Log("접시 내려");
            handlingThing.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
        }
        else if (handlingThing.CompareTag("Pot") || handlingThing.CompareTag("Pan"))
        {
            handlingThing.transform.GetComponent<BoxCollider>().isTrigger = false;
            handlingThing.transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
        }
        else
        {
            handlingThing.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
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
        // isHolding = true;
        // anim.SetBool("isHolding", isHolding);
        HandleObject(handleThing);
    }

    [Command]
    private void CmdTablePlaceOrDropObject(bool drop)
    {
        RpcTablePlaceOrDropObject(drop);
    }

    [ClientRpc]
    private void RpcTablePlaceOrDropObject(bool drop)
    {
        TablePlaceOrDropObject(drop);
    }

    // 내가 뭔가를 들고있을 때
    // true 테이블 위에 뭔가 있음 , false 테이블 위에 뭔가 없음
    private void TablePlaceOrDropObject(bool drop)
    {
        SoundManager.Instance.PlayEffect(drop ? "put" : "put");
        if (drop && isLocalPlayer)
        {
            // true 테이블 위에 뭔가 있는데 내가 가진게 접시고, 음식이면 담음
            if (CanPlaceIngredient())
            {
                //CmdPlaceIngredient();
                PlaceIngredient();
            }
            else
            {
                SoundManager.Instance.PlayEffect("no");
            }

            if (objectHighlight.transform.parent.childCount > 2)
            {
                GameObject ingredientObj = transform.GetChild(1).gameObject;
                var ingredient = ingredientObj.transform.GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().type;

                GameObject potAndPan = objectHighlight.transform.parent.GetChild(2).gameObject;
                if (objectHighlight.transform.parent.GetChild(2).name.Equals("PFX_PanFire"))
                    potAndPan = objectHighlight.transform.parent.GetChild(3).gameObject;

                // 테이블에 있는게, Pan이고 내가 든게 미트, 닭고기면 실행
                if (potAndPan.CompareTag("Pan") && (ingredient == Ingredient_Net.IngredientType.Meat || ingredient == Ingredient_Net.IngredientType.Chicken))
                {
                    ingredientObj.transform.SetParent(potAndPan.transform);
                    // 위치 설정
                    ingredientObj.transform.localPosition = new Vector3(2e-05f, -0.00017f, 0.00013f);
                    // 회전 설정
                    ingredientObj.transform.localRotation = Quaternion.Euler(0f, -168.905f, 0f);

                    potAndPan.GetComponent<PanOnStove>().inSomething = true;
                    potAndPan.GetComponent<PanOnStove>().AddNewIngredient();

                    anim.SetBool("isHolding", false);
                    isHolding = false;
                }
                else
                {
                    SoundManager.Instance.PlayEffect("no");
                }

                // 테이블에 있는게, Pot이고 내가 든게 쌀이고 화덕이면 Pot에 붙이기
                if (potAndPan.CompareTag("Pot") && ingredient == Ingredient_Net.IngredientType.Rice)
                {
                    ingredientObj.transform.SetParent(potAndPan.transform);
                    // 위치 설정
                    ingredientObj.transform.localPosition = new Vector3(2e-05f, -0.00017f, 0.00013f);
                    // 회전 설정
                    ingredientObj.transform.localRotation = Quaternion.Euler(0f, -168.905f, 0f);

                    potAndPan.GetComponent<PotOnStove>().inSomething = true;
                    potAndPan.GetComponent<PotOnStove>().AddNewIngredient();

                    anim.SetBool("isHolding", false);
                    isHolding = false;
                }
                else
                {
                    SoundManager.Instance.PlayEffect("no");
                }
            }

        }
        else if (isLocalPlayer)
        {
            if (transform.childCount > 1)
            {
                // false 테이블 위에 뭔가 없음 => 놓기.
                GameObject handlingThing = transform.GetChild(1).gameObject;
                Debug.Log($"handlingThing.name : {handlingThing.name}");
                HandleObject(handlingThing, false);
            }
        }
    }

    [Command]
    private void CmdPlaceIngredient()
    {
        // PlaceIngredient() 서버에서 호출
        RpcPlaceIngredient();
    }

    [ClientRpc]
    private void RpcPlaceIngredient()
    {
        PlaceIngredient();
    }

    private bool CanPlaceIngredient()
    {
        return canActive && objectHighlight.onSomething
               && interactObject.transform.parent.childCount > 2
               && IsPlate(interactObject.transform.parent.GetChild(2))
               && isHolding
               && IsHoldingCookedIngredient();
    }

    private bool IsPlate(UnityEngine.Transform obj)
    {
        var handle = obj.GetComponent<Ingredient_Net>();
        return handle != null && handle.type == Ingredient_Net.IngredientType.Plate;
    }

    private bool IsHoldingCookedIngredient()
    {
        var holdingObj = transform.GetChild(1).GetChild(0);
        if (holdingObj.childCount > 0)
        {
            var handle = holdingObj.GetChild(0).GetComponent<Ingredient_Net>();
            bool checkisCooked = handle.isCooked;

            // 김은 조리 안되어도 접시 올라감
            if (handle.type == Ingredient_Net.IngredientType.SeaWeed ||
                handle.type == Ingredient_Net.IngredientType.Tortilla)
            {
                checkisCooked = true;
            }

            // 미트 치킨은 조리안하면 못올라감
            //if (handle.type == Ingredient_Net.IngredientType.Meat ||
            //    handle.type == Ingredient_Net.IngredientType.Chicken)
            //{
            //    checkisCooked = false;
            //}

            return handle != null && checkisCooked;
        }
        return false;
    }

    private void PlaceIngredient()
    {
        if (isLocalPlayer) 
        {
            var plate = interactObject.transform.parent.GetChild(2).GetComponent<Plates_Net>();
            var ingredient = transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Ingredient_Net>().type;
            //plate.CmdAddIngredient(ingredient);
    
            if (plate.AddIngredient(ingredient))
            {
                //CmdPlayPutSound();
                plate.InstantiateUI();
                Destroy(transform.GetChild(1).gameObject);
                isHolding = false;
                anim.SetBool("isHolding", false);
            }
    
        }
    }

    //private void PlaceIngredient()
    //{
    //    if (isLocalPlayer)
    //    {
    //        var plate = interactObject.transform.parent.GetChild(2).GetComponent<Plates_Net>();
    //        var ingredient = transform.GetChild(1).GetChild(0).GetChild(0).gameObject.GetComponent<Ingredient_Net>().type;
    //
    //        // 서버에서 클라이언트의 요청을 처리할 수 있도록 서버측 메서드를 호출
    //        CmdAddIngredient(plate.netIdentity, ingredient);
    //    }
    //}


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
                if (obj.transform.childCount == 3 && obj.transform.GetChild(2).transform.GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().isCooked)
                {
                    //포트는 놔둠
                    objectHighlight.onSomething = true;

                    if (obj.CompareTag("Pan"))
                    {
                        obj.GetComponent<PanOnStove>().inSomething = false;
                    }
                    else if (obj.CompareTag("Pot"))
                    {
                        obj.GetComponent<PotOnStove>().inSomething = false;
                    }

                    Debug.Log($"obj : {obj.name}");

                    GameObject cookedIngredientObj = obj.transform.GetChild(2).gameObject;
                    cookedIngredientObj.transform.SetParent(transform); // 플레이어의 하위 객체로 설정
                    cookedIngredientObj.transform.GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().HandleIngredient(transform, cookedIngredientObj.transform.GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().type, true);

                    anim.SetBool("isHolding", true);
                    isHolding = true;
                }
                else
                {
                    obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    obj.transform.GetComponent<Ingredient_Net>().HandleIngredient(transform, obj.transform.GetComponent<Ingredient_Net>().type, true);
                    objectHighlight.onSomething = false;

                    obj.transform.SetParent(transform); // 플레이어의 하위 객체로 설정
                    // 플레이어에서 위치 잡기
                    // SetPositionbetweenPlayerandObject(obj);
                    anim.SetBool("isHolding", true);
                    isHolding = true;
                    if (obj.CompareTag("Pan")) obj.transform.GetChild(0).GetComponent<BoxCollider>().size *= 2f;
                }
            }
            else
            {
                obj.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                obj.transform.GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().HandleIngredient(transform, obj.transform.GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().type, true);

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
            SoundManager.Instance.PlayEffect("put");
            GameObject handleThing = transform.GetChild(1).gameObject;

            if (interactObject.transform.parent.CompareTag("MineCounter"))
            {
                placeTransform = interactObject.transform.parent.GetChild(1).localPosition + new Vector3(0.072f, 0.006f, 0.024f);
            }
            else if (interactObject.transform.parent.CompareTag("WizardCounter"))
            {
                placeTransform = interactObject.transform.parent.GetChild(1).localPosition + new Vector3(0.10746f, 0.00500000005f, 0.0235699993f);
                Debug.Log(interactObject.transform.parent.GetChild(1).name);
            }
            else if (interactObject.transform.parent.CompareTag("MineBoard"))
            {
                Debug.Log("MineBoard");
                placeTransform = interactObject.transform.parent.GetChild(1).localPosition + new Vector3(0f, 0.0055f, 0f);
            }
            else
            {
                placeTransform = interactObject.transform.parent.GetChild(1).localPosition;
            }

            Vector3 playerDirection = transform.forward;

            if (handleThing.CompareTag("Ingredient"))
            {
                objectHighlight.onSomething = true;
                isHolding = false;
                handleThing.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient_Net>().isOnDesk = true;
                handleThing.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient_Net>().
                    IngredientHandleOff(
                    interactObject.transform.parent,
                    placeTransform,
                    handleThing.transform.GetChild(0).GetChild(0).GetComponent<Ingredient_Net>().type);
            }
            else if (handleThing.CompareTag("Pot"))
            {
                if (objectHighlight.objectType != ObjectHighlight.ObjectType.Board)
                {
                    objectHighlight.onSomething = true;
                    isHolding = false;
                    handleThing.GetComponent<Ingredient_Net>().isOnDesk = true;
                    handleThing.transform.GetChild(0).GetComponent<BoxCollider>().size /= 2f;

                    handleThing.GetComponent<Ingredient_Net>().
                        PlayerHandleOff(interactObject.transform.parent,
                        placeTransform, Quaternion.LookRotation(playerDirection).normalized);
                }
            }
            else if (handleThing.CompareTag("Pan"))
            {
                if (objectHighlight.objectType != ObjectHighlight.ObjectType.Board)
                {
                    objectHighlight.onSomething = true;
                    isHolding = false;
                    handleThing.GetComponent<Ingredient_Net>().isOnDesk = true;
                    handleThing.GetComponent<Ingredient_Net>().
                        PlayerHandleOff(interactObject.transform.parent,
                        placeTransform, Quaternion.LookRotation(playerDirection).normalized);
                }
                // 콜라이더 감소
                handleThing.transform.GetChild(0).GetComponent<BoxCollider>().size /= 2f;
            }
            else // 접시
            {
                if (objectHighlight.objectType != ObjectHighlight.ObjectType.Board)
                {
                    objectHighlight.onSomething = true;
                    isHolding = false;
                    handleThing.GetComponent<Ingredient_Net>().isOnDesk = true;
                    handleThing.GetComponent<Ingredient_Net>().
                        PlayerHandleOff(interactObject.transform.parent,
                        placeTransform);
                }
            }

            // 모든 물체의 rotation.y는 고정
            // handleThing.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;
            anim.SetBool("isHolding", isHolding);
        }
    }

    //public void GetCraftIngredient(GameObject foodPrefabs)
    //{
    //    GameObject newFood = Instantiate(foodPrefabs, Vector3.zero, Quaternion.identity);
    //    isHolding = true;
    //    newFood.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    //    Craft_Net craft_Net = interactObject.GetComponent<Craft_Net>();
    //    newFood.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient_Net>().HandleIngredient(transform, craft_Net.ConvertFoodTypeToHandleType(craft_Net.food), true);
    //}

    private void PickupFromCraft()
    {
        SoundManager.Instance.PlayEffect("take");
        // Craft에서 아이템 꺼내기 로직 구현
        interactObject.GetComponent<Craft_Net>().OpenCraft();
        GameObject ingredient = interactObject.GetComponent<Craft_Net>().foodPrefabs;
        GetCraftIngredient(ingredient);
        objectHighlight.onSomething = false;
        isHolding = true;
        anim.SetBool("isHolding", isHolding);
    }

    // 그냥 지가 꺼내고 아무도 못봄.
    //public void GetCraftIngredient(GameObject foodPrefabs)
    //{
    //    Ingredient_Net.IngredientType it =  foodPrefabs.transform.GetChild(0).GetChild(0).transform.GetComponent<Ingredient_Net>().type;
    //    OverNetworkRoomManager manager = (OverNetworkRoomManager)NetworkRoomManager.singleton;
    //    GameObject foodPrefab = manager.GetCraftIngredient_RoomManager(it);
    //    isHolding = true;
    //    foodPrefab.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    //    Craft_Net craft_Net = interactObject.GetComponent<Craft_Net>();
    //    foodPrefab.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient_Net>().HandleIngredient(transform, craft_Net.ConvertFoodTypeToHandleType(craft_Net.food), true);
    //}

    public void GetCraftIngredient(GameObject foodPrefabs)
    {
        Ingredient_Net.IngredientType it = foodPrefabs.transform.GetChild(0).GetChild(0).transform.GetComponent<Ingredient_Net>().type;
        CmdRequestCraftIngredient(it);
    }

    [Command]
    private void CmdRequestCraftIngredient(Ingredient_Net.IngredientType it)
    {
        OverNetworkRoomManager manager = (OverNetworkRoomManager)NetworkRoomManager.singleton;
        GameObject foodPrefab = manager.GetCraftIngredient_RoomManager(it);
        NetworkServer.Spawn(foodPrefab);
        RpcAssignFoodToPlayer(foodPrefab.GetComponent<NetworkIdentity>().netId, it);
    }

    [ClientRpc]
    private void RpcAssignFoodToPlayer(uint netId, Ingredient_Net.IngredientType it)
    {
        GameObject newFood = NetworkClient.spawned[netId].gameObject;
        if (newFood != null)
        {
            isHolding = true;
            newFood.transform.GetChild(0).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            newFood.transform.GetChild(0).transform.GetChild(0).GetComponent<Ingredient_Net>().HandleIngredient(transform, it, true);
        }
        else
        {
            Debug.LogWarning("Failed to assign food to player, newFood is null.");
        }
    }

    private void DisposePlate()
    {
        transform.GetChild(1).gameObject.GetComponent<Plates_Net>().ClearIngredient();
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
        ingredientObj.transform.GetChild(0).GetComponent<Ingredient_Net>().HandleIngredient(transform, ingredientObj.transform.GetChild(0).GetComponent<Ingredient_Net>().type, true);
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
        ingredientObj.transform.GetComponent<Ingredient_Net>().HandleIngredient(transform, ingredientObj.transform.GetComponent<Ingredient_Net>().type, true);
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
        if (isLocalPlayer)
            if (CheckForIngredientHandling(other)) return;

        if (isLocalPlayer)
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
        interactObject.GetComponent<ObjectHighlight>().DeactivateHighlight(interactObject.GetComponent<Ingredient_Net>().isCooked);
    }

    private bool HandlePickupIngredient(Collider other)
    {
        if (other.CompareTag("Ingredient"))
        {
            if (interactObject == null && !other.GetComponent<Ingredient_Net>().isOnDesk)
            {
                SetinteractObject(other.gameObject);
                other.GetComponent<ObjectHighlight>().ActivateHighlight(other.GetComponent<Ingredient_Net>().isCooked);
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
            other.GetComponent<ObjectHighlight>().ActivateHighlight(other.GetComponent<Ingredient_Net>().isCooked);
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
        if (isLocalPlayer)
            if (ShouldReturnEarly(other)) return;

        if (isLocalPlayer)
            if (HandleActiveIngredientSwitch(other)) return;

        if (isLocalPlayer)
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
        interactObject.GetComponent<ObjectHighlight>().DeactivateHighlight(interactObject.GetComponent<Ingredient_Net>().isCooked);
        interactObject = nextInteractObject;
        objectHighlight = interactObject.GetComponent<ObjectHighlight>();
        interactObject.GetComponent<ObjectHighlight>().DeactivateHighlight(interactObject.GetComponent<Ingredient_Net>().isCooked);
        nextInteractObject = null;
    }

    private void ClearActiveObjects()
    {
        interactObject.GetComponent<ObjectHighlight>().DeactivateHighlight(interactObject.GetComponent<Ingredient_Net>().isCooked);
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
        bool highlightState = other.CompareTag("Ingredient") ? other.GetComponent<Ingredient_Net>().isCooked : true;
        other.GetComponent<ObjectHighlight>().ActivateHighlight(highlightState);
    }
    #endregion

    #region OnTriggerExit
    private void OnTriggerExit(Collider other)
    {
        if (isLocalPlayer)
        {
            if (ShouldDeactivateObjects())
            {
                //HandleBoardInteraction();
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
            interactObject.transform.GetChild(0).GetComponent<CuttingBoard_Net>().PauseSlider(true);
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
            bool highlightState = interactObject.CompareTag("Ingredient") ? interactObject.GetComponent<Ingredient_Net>().isCooked : true;
            interactObject.GetComponent<ObjectHighlight>().DeactivateHighlight(highlightState);
        }
    }

    private void OnHighlightCurrentObject()
    {
        if (interactObject != null && interactObject.GetComponent<ObjectHighlight>() != null)
        {
            bool highlightState = interactObject.CompareTag("Ingredient") ? interactObject.GetComponent<Ingredient_Net>().isCooked : true;
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
        //        //obj.GetComponent<Ingredient_Net>().HandleIngredient(obj.transform, obj.transform.GetComponent<Ingredient_Net>().type, true);
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

    #region Coin



    #endregion


}
