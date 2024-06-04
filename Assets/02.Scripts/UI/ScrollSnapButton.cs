using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollSnapButton : MonoBehaviour
{
    public SimpleScrollSnap snap;
    // 킬꺼
    public GameObject RandomSelectMap;
    public Sprite selectImg;
    public Sprite notselectImg;
    public Image backImage;
    public GameObject MapObject;
    public GameObject ReadyBtn;
    
    string name;

    public void Start()
    {
        snap.GetComponent<SimpleScrollSnap>();
    }

    public void Update()
    {
        if (snap.OnPanelSelectedGetName() == MapObject.name)
        {
            backImage.sprite = selectImg;
        }
        else
        {
            backImage.sprite = notselectImg;
        }
    }

    public void SnapMapCancle()
    {
        OverNetworkRoomPlayer[] players = FindObjectsOfType<OverNetworkRoomPlayer>();

        if (players == null)
            return;

        int _index = 0;

        foreach (var player in players)
        {
            if (player.isLocalPlayer == true)
                _index = player.index;
        }

        RandomSelectMap.transform.GetChild(_index).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
        RandomSelectMap.transform.GetChild(_index).transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
        RandomSelectMap.transform.GetChild(_index).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
        RandomSelectMap.transform.GetChild(_index).transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);

        // 선택화면 오브젝트를 키고
        snap.gameObject.SetActive(true);
        // 랜덤맵을 끄고
        RandomSelectMap.gameObject.SetActive(false);
        // 버튼을 비활성화 시키고
        ReadyBtn.gameObject.SetActive(false);
    }

    public void SnapMapClick()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        name = clickObject.name;
        if (snap.OnPanelSelectedGetName() == name)
        {
            if (name == "Wizard")
            {
                UIManager.Instance.sceneType = EnumTypes.SceneType.BattleMap;
                UIManager.Instance.mapType = EnumTypes.MapType.StageWizard;
                // 선택하면 오브젝트를 끄고
                //snap.gameObject.SetActive(false);
                ////// 버튼을 활성화 시키고
                //ReadyBtn.gameObject.SetActive(true);
                ////// 랜덤맵을 염
                //RandomSelectMap.gameObject.SetActive(true);
                //FindOverNetworkRoomPlayerAndCheckMap(name);
            }
            else if (name == "Mine")
            {
                UIManager.Instance.sceneType = EnumTypes.SceneType.BattleMap;
                UIManager.Instance.mapType = EnumTypes.MapType.StageMine;
                //// 선택하면 오브젝트를 끄고
                //snap.gameObject.SetActive(false);
                ////// 버튼을 활성화 시키고
                //ReadyBtn.gameObject.SetActive(true);
                ////// 랜덤맵을 염
                //RandomSelectMap.gameObject.SetActive(true);
                //FindOverNetworkRoomPlayerAndCheckMap(name);
            }
            else 
            {
                return;
            }
            UIManager.Instance.EnterLoadingMapUI();
        }
    }

    public void FindOverNetworkRoomPlayerAndCheckMap(string mapName)
    {
        OverNetworkRoomPlayer[] players = FindObjectsOfType<OverNetworkRoomPlayer>();

        if (players == null)
            return;

        int _index = 0;

        foreach (var player in players)
        {
            if(player.isLocalPlayer == true)
                _index = player.index;
        }


        switch (mapName)
        {
            case "Wizard":
                // Image
                RandomSelectMap.transform.GetChild(_index).transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
                // Text
                RandomSelectMap.transform.GetChild(_index).transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
                break;
            case "Mine":
                // Image
                RandomSelectMap.transform.GetChild(_index).transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                // Text
                RandomSelectMap.transform.GetChild(_index).transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
                break;
            default :
                break;
        }
        
    }

}
