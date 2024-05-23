using DG.Tweening.Core.Easing;
using System;
using UnityEngine;
using UnityEngine.XR;

public class Ingredient : GameItem
{
    public bool isCooked = false;
    public bool isOnDesk = true;
    // 오직 피자만을 위한 bool 값 / 피자 완성을 위한 값
    public bool pizzazIsCooked = false;

    public enum IngredientType { Fish, Shrimp, Plate, Lettuce, Tomato, Cucumber, Chicken, Potato, Pot, Pan, Tortilla, SeaWeed, Rice, Pepperoni, Cheese, Dough, Meat, PizzaTomato, SushiRice, SushiFish, SushiCucumber };
    public IngredientType type; // 재료 유형: 채소, 고기 등
    
    public enum IngredientState { Raw, Cooking, Cooked }
    public IngredientState currentState = IngredientState.Raw;

    [SerializeField] private Mesh cookedIngredient;
    [SerializeField] private Material cookedFish;

    public Vector3 fishLocalPos = new Vector3(0, 0.138f, 0.08f);
    public Vector3 shrimpLocalPos = new Vector3(-0.365000874f, -0.0890001357f, -0.423000485f);
    public Vector3 lettuceLocalPos = new Vector3(0, 0.418000013f, 0);

    public override void Interact(PlayerInteractController player)
    {
        // 기본 상호작용: 재료를 주움
        //Pickup(player);
    }

    // 상태 변경 메소드
    public void ChangeState(IngredientState newState)
    {
        currentState = newState;
        Debug.Log("State changed to: " + currentState);
    }

    #region HandleIngredient
    // Ingredient 핸들링과 관련된 로직
    public void HandleIngredient(Transform something, IngredientType handle, bool isActive)
    {
        if (handle == IngredientType.Pot || handle == IngredientType.Pan)
        {
            BoxCollider collider = transform.GetComponent<BoxCollider>();
            collider.isTrigger = isActive;
        }
        else
        {
            MeshCollider collider = transform.parent.GetComponent<MeshCollider>();
            collider.isTrigger = isActive;
        }

        Vector3 localPosition;
        Quaternion localRotation;

        switch (handle)
        {
            case IngredientType.Fish:
                localPosition = fishLocalPos;
                localRotation = Quaternion.identity;
                break;
            case IngredientType.Shrimp:
                localPosition = isCooked ? Vector3.zero : shrimpLocalPos;
                localRotation = isCooked ? Quaternion.identity : Quaternion.Euler(new Vector3(35.029995f, -1.04264745e-06f, 326.160004f));
                break;
            case IngredientType.Lettuce:
                localPosition = lettuceLocalPos;
                localRotation = Quaternion.identity;
                break;
            case IngredientType.Tortilla:
                localPosition = Vector3.zero;
                localRotation = Quaternion.Euler(new Vector3(0, 90f, 0));
                break;
            case IngredientType.SeaWeed:
                localPosition = Vector3.zero;
                localRotation = Quaternion.Euler(new Vector3(90f, 0, 0));
                break;
            case IngredientType.Pot:
            case IngredientType.Pan:
            case IngredientType.Tomato:
            case IngredientType.PizzaTomato:
            case IngredientType.Cucumber:
            case IngredientType.Chicken:
            case IngredientType.Potato:
            case IngredientType.Rice:
            case IngredientType.Pepperoni:
            case IngredientType.Meat:
            case IngredientType.Dough:
            case IngredientType.Cheese:
            case IngredientType.SushiRice:
            case IngredientType.SushiFish:
            case IngredientType.SushiCucumber:
                localPosition = Vector3.zero;
                localRotation = Quaternion.identity;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(handle), $"Unsupported handle type: {handle}");
        }

        Transform parentTransform;
        if (handle == IngredientType.Pot || handle == IngredientType.Pan)
        {
            parentTransform = transform;
            parentTransform.localPosition = localPosition;
            parentTransform.localRotation = localRotation;
        }
        else 
        {
            parentTransform = transform.parent;
            parentTransform.localPosition = localPosition;
            parentTransform.localRotation = localRotation;
        }

        if (isActive && handle == IngredientType.Pot)
        {
            parentTransform.SetParent(something);
            parentTransform.localRotation = Quaternion.identity;
            parentTransform.localPosition = new Vector3(-0.409999996f, 0, 3.610047f);
        }
        else if (isActive && handle == IngredientType.Pan)
        {
            parentTransform.SetParent(something);
            parentTransform.localRotation = Quaternion.identity;
            parentTransform.localPosition = new Vector3(-0.409999996f, 0.39f, 3.84000003f);
        }
        else if (isActive)
        {
            parentTransform.parent.SetParent(something);
            parentTransform.parent.localRotation = Quaternion.identity;
            parentTransform.parent.localPosition = new Vector3(-0.409999996f, 0, 1.84000003f);
        }
    }

    // Player와 관련된 간단한 핸들링
    public void HandlePlayer(Transform something, Vector3 target, bool isOn)
    {
        Transform myTransform = transform;
        myTransform.SetParent(something);
        myTransform.localRotation = Quaternion.identity;
        myTransform.localPosition = isOn ? new Vector3(-0.4f, 0.24f, 2.23f) : target;
    }

    // Mesh 변경 로직
    public void ChangeMesh(IngredientType handType)
    {
        MeshFilter meshFilter = transform.parent.GetComponent<MeshFilter>();
        MeshCollider meshCollider = transform.parent.GetComponent<MeshCollider>();
        meshFilter.mesh = cookedIngredient;
        meshCollider.sharedMesh = cookedIngredient;

        switch (handType)
        {
            case IngredientType.Fish:
            case IngredientType.Tomato:
            case IngredientType.Shrimp:
            case IngredientType.Lettuce:
            case IngredientType.Potato:
            case IngredientType.PizzaTomato:
            case IngredientType.Dough:
            case IngredientType.Pepperoni:
            case IngredientType.Cheese:
            case IngredientType.Meat:
            case IngredientType.Chicken:
            case IngredientType.Rice:
                ApplyMaterialAndAdjustPosition(handType);
                break;
            default:
                Vector3 positionAdjustment = new Vector3(0, -0.0025f, 0);
                AdjustPosition(transform.parent.parent, positionAdjustment);
                break;
        }
    }

    private void ApplyMaterialAndAdjustPosition(IngredientType handType)
    {
        MeshRenderer meshRenderer = transform.parent.GetComponent<MeshRenderer>();
        meshRenderer.material = cookedFish; // Example for applying material
        Vector3 positionAdjustment = new Vector3(0, 0, 0);
        //Vector3 positionAdjustment = new Vector3(0, CalculatePositionAdjustment(handType), 0);
        AdjustPosition(transform.parent.parent, positionAdjustment);
    }

    private float CalculatePositionAdjustment(IngredientType handType)
    {
        switch (handType)
        {
            case IngredientType.Fish:
                return -0.0017f;
            case IngredientType.Tomato:
                return -0.017f;
            case IngredientType.Shrimp:
                return -0.0013f;
            case IngredientType.Lettuce:
                return -0.0028f;
            default:
                return -0.0025f;
        }
    }

    private void AdjustPosition(Transform targetTransform, Vector3 adjustment)
    {
        targetTransform.localPosition += adjustment;
    }

    // 사용되지 않는 코드의 주석 처리 및 정리
    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("deadZone"))
        //{
        //    HandleDeadZoneInteraction(type);
        //}
    }

    private void HandleDeadZoneInteraction(IngredientType handType)
    {
        GameObject parent = transform.parent.parent.gameObject;
        if (handType != IngredientType.Plate)
        {
            //parent.GetComponent<DestroyIngredient>().DestroySelf();
        }
        else // Plate handling
        {
            Destroy(gameObject);
            //GameManager.instance.PlateReturn();
        }
    }

    // 각 재료에 대한 자동 배치를 수행하는 메서드
    public void IngredientAuto(Transform parent, Vector3 target, IngredientType handle)
    {
        // 콜라이더의 트리거 비활성화
        transform.parent.GetComponent<MeshCollider>().isTrigger = false;

        // 위치와 회전 설정
        SetPositionAndRotation(handle);

        // 부모 설정 및 최종 위치, 회전 적용
        transform.parent.parent.SetParent(parent);
        transform.parent.parent.localRotation = Quaternion.identity;
        transform.parent.parent.localPosition = target;

        // 특정 조건에서 회전 또는 추가 위치 조정
        AdjustPositionOrRotationBasedOnCookedState(handle);
    }

    // 위치와 회전을 설정하는 메서드
    private void SetPositionAndRotation(IngredientType handle)
    {
        switch (handle)
        {
            case IngredientType.Fish:
                SetTransform(fishLocalPos, Quaternion.identity);
                break;
            case IngredientType.Shrimp:
                if (!isCooked)
                    SetTransform(shrimpLocalPos, Quaternion.Euler(35.029995f, -1.04264745e-06f, 326.160004f));
                else
                    SetTransform(Vector3.zero, Quaternion.identity);
                break;
            case IngredientType.Lettuce:
                SetTransform(lettuceLocalPos, Quaternion.identity);
                break;
            case IngredientType.Tomato:
            case IngredientType.Cucumber:
            case IngredientType.Chicken:
            case IngredientType.Potato:
                SetTransform(Vector3.zero, Quaternion.identity);
                break;
        }
    }

    // 위치와 회전을 동시에 설정
    private void SetTransform(Vector3 position, Quaternion rotation)
    {
        transform.parent.localPosition = position;
        transform.parent.localRotation = rotation;
    }

    // 조리 상태에 따라 위치 또는 회전을 조정하는 메서드
    private void AdjustPositionOrRotationBasedOnCookedState(IngredientType handle)
    {
        if (handle == IngredientType.Shrimp && !isCooked)
        {
            transform.parent.parent.localRotation = Quaternion.Euler(0, 244.302612f, 0);
        }
        else if (handle == IngredientType.Fish && isCooked)
        {
            AdjustYPosition(-0.00224f);
        }
        else if (handle == IngredientType.Lettuce)
        {
            AdjustYPosition(isCooked ? -0.5f : -0.095f);
        }
        else if (handle == IngredientType.Tomato)
        {
            AdjustYPosition(isCooked ? -0.21f : -0.08f);
        }
    }

    // Y 위치를 조정
    private void AdjustYPosition(float offsetY)
    {
        Vector3 currentPosition = transform.parent.parent.localPosition;
        transform.parent.parent.localPosition = new Vector3(currentPosition.x, currentPosition.y + offsetY, currentPosition.z);
    }
    #endregion

    #region IngredientHandleOff
    public void IngredientHandleOff(Transform parent, Vector3 target, IngredientType handle)
    {
        transform.parent.GetComponent<MeshCollider>().isTrigger = false;

        //SetLocalPositionAndRotation(handle);

        transform.parent.parent.SetParent(parent);
        transform.parent.parent.localPosition = target;
        transform.parent.parent.localRotation = Quaternion.identity;

        //AdjustParentPositionAndRotation(handle);
    }

    private void SetLocalPositionAndRotation(IngredientType handle)
    {
        Vector3 localPosition = Vector3.zero;
        Quaternion localRotation = Quaternion.identity;

        switch (handle)
        {
            case IngredientType.Fish:
                localPosition = fishLocalPos;
                break;
            case IngredientType.Shrimp:
                localPosition = shrimpLocalPos;
                localRotation = !isCooked ? Quaternion.Euler(new Vector3(35.029995f, -1.04264745e-06f, 326.160004f)) : Quaternion.identity;
                break;
            case IngredientType.Lettuce:
                localPosition = lettuceLocalPos;
                break;
            case IngredientType.Tomato:
                localPosition = new Vector3(0, 0, 0);
                break;
            case IngredientType.Cucumber:
            case IngredientType.Chicken:
                break;
            case IngredientType.Potato:
                transform.parent.GetChild(0).localRotation = Quaternion.identity;
                break;
        }

        transform.parent.localPosition = localPosition;
        transform.parent.localRotation = localRotation;
    }

    private void AdjustParentPositionAndRotation(IngredientType handle)
    {
        Vector3 positionOffset = Vector3.zero;
        Quaternion rotationOffset = Quaternion.identity;

        switch (handle)
        {
            case IngredientType.Lettuce:
                positionOffset = isCooked ? new Vector3(0,0,0) : new Vector3(0, 0, 0);
                break;
            case IngredientType.Shrimp:
                if (!isCooked)
                {
                    rotationOffset = Quaternion.Euler(new Vector3(0, 244.302612f, 0));
                }
                else
                {
                    positionOffset = new Vector3(0, -0.0008f, 0);
                }
                break;
            case IngredientType.Fish:
                if (isCooked)
                {
                    positionOffset = new Vector3(0, -0.00146f, 0);
                }
                break;
            case IngredientType.Tomato:
                //positionOffset = new Vector3(0, isCooked ? -0.21f : -0.08f, 0);
                //positionOffset = new Vector3(0, isCooked ? -0.21f : 0, 0);
                break;
            default:
                positionOffset = new Vector3(0, -0.0025f, 0);
                break;
        }

        transform.parent.parent.localPosition += positionOffset;
        transform.parent.parent.localRotation = rotationOffset * transform.parent.parent.localRotation;
    }
    #endregion
    
    public void PlayerHandleOff(Transform parent, Vector3 target)
    {
        transform.SetParent(parent);
        transform.rotation = Quaternion.identity;
        transform.localPosition = target;
    }
}
