using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCheck : MonoBehaviour
{
    // 충돌 감지 시 유효한 객체인지 확인하고 적절하게 처리
    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidObject(other)) return; // 유효한 오브젝트인지 초기 필터링
        HandleInteraction(other); // 상호작용 처리
    }

    // 충돌한 오브젝트가 유효한지 검증
    private bool IsValidObject(Collider other)
    {
        // 플레이어 또는 접시와의 충돌은 무시
        if (other.CompareTag("Player") || other.CompareTag("Plate")) return false;

        // 해당 오브젝트가 플레이어의 현재 상호작용 대상이 아니어야 함
        var player = FindObjectOfType<PlayerInteractController>();
        var player2 = FindObjectOfType<Player2InteractController>();
        if (IsObjectHandledByPlayer(other, player) || IsObjectHandledByPlayer(other, player2))
            return false;

        // MeshCollider가 있고, 해당 위치에 다른 오브젝트가 없어야 함
        return other.GetComponent<MeshCollider>() != null && !transform.parent.parent.GetChild(0).GetComponent<ObjectHighlight>().onSomething;
    }

    // 플레이어가 이미 처리 중인 오브젝트인지 확인
    private bool IsObjectHandledByPlayer(Collider other, PlayerInteractController player)
    {
        return player.transform.childCount > 1 && player.transform.GetChild(1).GetChild(0) != null &&
               player.transform.GetChild(1).GetChild(0).childCount > 0 &&
               !player.transform.GetChild(1).GetChild(0).GetChild(0).Equals(other);
    }
    private bool IsObjectHandledByPlayer(Collider other, Player2InteractController player)
    {
        return player.transform.childCount > 1 && player.transform.GetChild(1).GetChild(0) != null &&
               player.transform.GetChild(1).GetChild(0).childCount > 0 &&
               !player.transform.GetChild(1).GetChild(0).GetChild(0).Equals(other);
    }

    // 충돌한 오브젝트에 대한 상호작용 처리
    private void HandleInteraction(Collider other)
    {
        Transform colliderParent = transform.parent.parent.GetChild(0);
        GameObject handleThing = other.gameObject;

        // 오브젝트 유형에 따른 처리 분기
        if (colliderParent.GetComponent<ObjectHighlight>().objectType == ObjectHighlight.ObjectType.CounterTop || colliderParent.GetComponent<ObjectHighlight>().objectType == ObjectHighlight.ObjectType.Board)
        {
            PlaceObjectOnSurface(colliderParent, handleThing); // 카운터톱 또는 보드 위에 오브젝트 배치
        }
        else if (colliderParent.GetComponent<ObjectHighlight>().objectType == ObjectHighlight.ObjectType.Craft)
        {
            HandleCraftInteraction(colliderParent, handleThing); // 크래프트 오브젝트 처리
        }
    }

    // 표면 위에 오브젝트 배치
    private void PlaceObjectOnSurface(Transform surface, GameObject obj)
    {
        //dsurface.GetComponent<ObjectHighlight>().onSomething = true;
        SetObjectOnDesk(obj);
        HandleIngredientAuto(surface, obj); // 자동 처리 로직
    }

    // 크래프트와의 상호작용
    private void HandleCraftInteraction(Transform craft, GameObject obj)
    {
        if (obj.CompareTag("Ingredient"))
        {
            //craft.GetComponent<ObjectHighlight>().onSomething = true;
            SetObjectOnDesk(obj);
            HandleIngredientAuto(craft.parent.parent, obj);  // 크래프트 상위 객체에서 처리
        }
        else if (obj.CompareTag("Plate"))
        {
            if (craft.GetComponent<ObjectHighlight>().objectType != ObjectHighlight.ObjectType.Board)
            {
                craft.GetComponent<ObjectHighlight>().onSomething = true;
                obj.transform.GetChild(0).GetComponent<Ingredient>().HandlePlayer(craft.parent.parent, craft.localPosition, false);
            }
        }
    }

    // 오브젝트를 표면에 배치
    private void SetObjectOnDesk(GameObject obj)
    {
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; // 움직임 제한
        obj.transform.GetChild(0).GetComponent<Ingredient>().isOnDesk = true;
    }

    // 오브젝트 자동 처리
    private void HandleIngredientAuto(Transform targetTransform, GameObject obj)
    {
        var handle = obj.transform.GetChild(0).GetComponent<Ingredient>();
        //handle.IngredientAuto(targetTransform, targetTransform.GetChild(1).localPosition, handle.type); // 자동 처리 로직 실행
    }
}
