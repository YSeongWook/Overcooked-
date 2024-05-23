using UnityEngine;

public class ObjectHighlight : MonoBehaviour
{
    // 플레이어 앞에 놓인 오브젝트가 활성화가 가능한 물건인지 확인
    public bool activeObject = false;
    public bool onSomething = false;

    // 오브젝트 MeshRenderer에 교체한 Highligt Material 배열
    [SerializeField] private Material[] hightlightMaterials;

    // 오브젝트 타입에 따른 변경
    [HideInInspector] public enum ObjectType { CounterTop, Craft, Return, Station, Sink, Ingredient, Board, Bin, Plate, Pot, Pan, Oven };
    // 현재 오브젝트 타입
    [SerializeField] public ObjectType objectType;

    public void ActivateHighlight(bool isCooked)
    {
        activeObject = true; // 모든 경우에서 canActive를 true로 설정
        Renderer rd;

        switch (objectType)
        {
            case ObjectType.CounterTop:
            case ObjectType.Board:
            case ObjectType.Bin:
            case ObjectType.Plate:
            case ObjectType.Pot:
            case ObjectType.Pan:
            case ObjectType.Oven:
                rd = transform.parent.GetComponent<MeshRenderer>();
                rd.material = hightlightMaterials[1];
                break;

            case ObjectType.Craft:
                rd = transform.parent.GetComponent<MeshRenderer>();
                rd.material = hightlightMaterials[1];
                rd = transform.parent.transform.parent.GetChild(1).GetComponent<MeshRenderer>();
                rd.material = hightlightMaterials[1];
                break;

            case ObjectType.Ingredient:
                rd = transform.parent.GetComponent<MeshRenderer>();
                rd.material = isCooked ? hightlightMaterials[3] : hightlightMaterials[1];
                break;

            case ObjectType.Station:
                Material[] originM = transform.GetChild(1).GetComponent<MeshRenderer>().materials;
                originM[0] = hightlightMaterials[2];
                originM[3] = hightlightMaterials[3];
                transform.GetChild(1).GetComponent<MeshRenderer>().materials = originM;
                break;

            case ObjectType.Return:
                rd = transform.parent.GetChild(0).GetComponent<MeshRenderer>();
                rd.material = hightlightMaterials[1];
                break;

            default:
                // 필요하다면 default 경우의 로직을 추가할 수 있습니다.
                break;
        }
    }

    public void DeactivateHighlight(bool isCooked)
    {
        activeObject = false; // 모든 경우에서 canActive를 false로 설정하기 때문에, 이를 switch 문 밖으로 이동
        Renderer rd;

        switch (objectType)
        {
            case ObjectType.CounterTop:
            case ObjectType.Board:
            case ObjectType.Bin:
            case ObjectType.Plate:
            case ObjectType.Pot:
            case ObjectType.Pan:
            case ObjectType.Oven:
                rd = transform.parent.GetComponent<MeshRenderer>();
                rd.material = hightlightMaterials[0];
                break;
            case ObjectType.Craft:
                rd = transform.parent.GetComponent<MeshRenderer>();
                rd.material = hightlightMaterials[0];
                rd = transform.parent.transform.parent.GetChild(1).GetComponent<MeshRenderer>();
                rd.material = hightlightMaterials[0];
                break;

            case ObjectType.Ingredient:
                rd = transform.parent.GetComponent<MeshRenderer>();
                rd.material = isCooked ? hightlightMaterials[2] : hightlightMaterials[0];
                break;

            case ObjectType.Station:
                Material[] originM = transform.GetChild(1).GetComponent<MeshRenderer>().materials;
                originM[0] = hightlightMaterials[0];
                originM[3] = hightlightMaterials[1];
                transform.GetChild(1).GetComponent<MeshRenderer>().materials = originM;
                break;

            case ObjectType.Return:
                rd = transform.parent.GetChild(0).GetComponent<MeshRenderer>();
                rd.material = hightlightMaterials[0];
                break;

            default:
                // 필요하다면 default 경우의 로직을 추가할 수 있습니다.
                break;
        }
    }
}
