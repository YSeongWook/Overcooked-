using UnityEngine;

public class CookingAppliance_Net : GameItem_Net
{
    public enum ApplianceType { ChoppingBoard, Stove, Oven, Fryer };
    public ApplianceType applianceType; // 기구 유형: 도마, 스토브, 오븐, 튀김기 등

    public enum CookingApplianceState { InUse, NotInUse }
    public CookingApplianceState currentState = CookingApplianceState.NotInUse;

    public override void Interact(PlayerInteractController_Net player)
    {
        //player.CookFood(this);
    }

    public void Cook(PlayerInteractController_Net player)
    {
        throw new System.NotImplementedException();
    }

    // 상태 변경 메소드
    public void ChangeState(CookingApplianceState newState)
    {
        currentState = newState;
        Debug.Log("State changed to: " + currentState);
    }
}
