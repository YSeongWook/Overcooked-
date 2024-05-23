using UnityEngine;
using static CookingAppliance;

public class ETCTool : GameItem
{
    public enum ETCToolType { Sink, WasteBasket, Station, PlateTable };
    public ETCToolType toolType; // 도구 유형: 싱크대, 쓰레기통, 서빙스테이션, 접시스폰테이블 등

    public enum ETCToolState { InUse, NotInUse }
    public ETCToolState currentState = ETCToolState.NotInUse;

    public override void Interact(PlayerInteractController player)
    {
        // 작업대와의 상호작용 로직 구현
        Debug.Log("Player has interacted with a craft station.");
        // 예를 들어, 아이템을 제작하거나 기능을 활성화하는 로직
        //player.ActivateCraft(this.gameObject);
    }

    // 상태 변경 메소드
    public void ChangeState(ETCToolState newState)
    {
        currentState = newState;
        Debug.Log("State changed to: " + currentState);
    }
}
