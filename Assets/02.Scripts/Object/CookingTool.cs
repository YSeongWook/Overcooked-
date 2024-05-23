using UnityEngine;
using static CookingAppliance;

public class CookingTool : GameItem
{
    public enum ToolType { Pan, Pot, Frybasket, Plate, Extinguisher };
    public ToolType toolType; // 도구 유형: 프라이팬, 냄비, 튀김망, 접시, 소화기? 등

    public enum CookingToolState { InUse, NotInUse }
    public CookingToolState currentState = CookingToolState.NotInUse;


    public override void Interact(PlayerInteractController player)
    {
        // 작업대와의 상호작용 로직 구현
        Debug.Log("Player has interacted with a craft station.");
        // 예를 들어, 아이템을 제작하거나 기능을 활성화하는 로직
        //player.ActivateCraft(this.gameObject);
    }

    // 상태 변경 메소드
    public void ChangeState(CookingToolState newState)
    {
        currentState = newState;
        Debug.Log("State changed to: " + currentState);
    }
}
