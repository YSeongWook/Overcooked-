using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class OvercookedRoomManager : NetworkRoomManager
{
    [Header("Player Parent Object")]
    public List<GameObject> playerBox;

    public void CreateRoom()
    {
        StartHost();
    }

    public void OnClickEnterGameRoomButton()
    {
        StartClient();
    }

    public void ExitGameRoom() 
    {
        if (mode == Mirror.NetworkManagerMode.Host)
        {
            StopHost();
            // 호스트가 종료하면 정상적으로 로그찍힘 확인
            //Debug.Log("ExitGameRoom / StopHost");
        }
        else if (mode == Mirror.NetworkManagerMode.ClientOnly)
        {
            StopClient();
        }
        else if (mode == Mirror.NetworkManagerMode.Offline)
        {
            StopHost();
            StopClient();
            Debug.Log("둘다 조져");
        }
    }

    // 플레이어가 추가될때 호출됨.
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log($"OnServerAddPlayer called for connection ID: {conn.connectionId}");
    }

    // Host가 게임에서 나가면 Client가 튕기는 로직
    public override void OnStopClient()
    {
        base.OnStopClient();
        //Debug.Log("OnStopClient() Start");
        //Debug.Log($"NetworkServer.connections : {NetworkServer.connections.Count}");

        // 모든 클라이언트를 Offline Scene으로 전환
        //foreach (var conn in NetworkServer.connections.Values)
        //{
        //if (conn.identity != null && conn.identity.isClient)
        //{
        //NetworkServer.Destroy(conn.identity.gameObject);
        //conn.Disconnect();
        // 게임룸 나갔을때 실행되는 로직
        ExitGameRoom();
        // Alret창에서 나가기 클릭했을때 실행되는 로직
        UIManager.Instance.AgreeExitLobby();
        //}
        //}
    }

    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);

        var player = Instantiate(spawnPrefabs[0]);
        NetworkServer.Spawn(player, conn);

        Debug.Log($"NetworkServer.connections.Count : {NetworkServer.connections.Count}");

        int index = 0;
        foreach (NetworkConnectionToClient connn in NetworkServer.connections.Values)
        {
            if (playerBox[index].transform.GetChild(0).name.Equals("Player Name"))
            {
                player.transform.SetParent(playerBox[index].transform);
                index++;
            }
        }
        index = 0;

        RectTransform imageRectTransform = player.GetComponent<RectTransform>();
        imageRectTransform.anchoredPosition = new Vector2(0, 40);
    }
}