using UnityEngine;

// 게임 내 아이템의 기본을 정의하는 추상 클래스
public abstract class GameItem_Net : MonoBehaviour
{
    // 모든 게임 아이템이 가져야 할 기본 속성
    public string itemName;
    //public Sprite icon;

    // IInteractable 인터페이스 구현을 강제함
    public abstract void Interact(PlayerInteractController_Net player);
}