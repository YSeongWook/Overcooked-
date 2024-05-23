using Mirror;
using System;
using UnityEngine;

public class CustomNetworkManager : NetworkRoomManager
{
    [Header("Server/Client Settings")]
    public string networkAddress_local = "localhost";

    [Header("UI Elements")]
    public GameObject[] playerSlots; // 플레이어 슬롯 UI 배열

    #region StartServerManually / StartClientManually / StopBattle
    public void StartServerManually()
    {
        Debug.Log("StartServerManually called");
        StartServer();
    }

    public void StartHostManually()
    {
        Debug.Log("StartHostManually called");
        StartHost();
        //StartClient();
    }

    public void StartClientManually()
    {
        Debug.Log("StartClientManually called");
        networkAddress = networkAddress_local;
        StartClient();
    }

    public void StopBattle()
    {
        Debug.Log("StopBattle called");
        //base.StopHost();

        if (mode == Mirror.NetworkManagerMode.Host)
        {
            StopHost();
        }
        else if (mode == Mirror.NetworkManagerMode.ClientOnly)
        {
            StopClient();
            index--;
        }

        // 슬롯 상태 초기화
        UpdatePlayerSlots();
    }
    #endregion

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("OnStartServer called");
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("OnClientConnect called");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //base.OnServerAddPlayer(conn);
        Debug.Log($"OnServerAddPlayer called for connection ID: {conn.connectionId}");

        // Player Prefab이 제대로 설정되었는지 확인
        //if (playerPrefab == null)
        //{
        //    Debug.LogError("Player Prefab is not assigned in NetworkManager");
        //    return;
        //}

        // Player Prefab을 인스턴스화하여 네트워크에 추가
        //GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        //NetworkServer.AddPlayerForConnection(conn, player);

        //UpdatePlayerSlots();
    }
    
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log($"OnServerDisconnect called for connection ID: {conn.connectionId}");
        index--;
        Debug.Log($"OnServerDisconnect / index : {index}");
        //UpdatePlayerSlots();
    }


    private void UpdatePlayerSlots()
    {
        Debug.Log("UpdatePlayerSlots called");
        // 모든 플레이어 슬롯을 초기화 (비활성화)
        foreach (GameObject slot in playerSlots)
        {
            //slot.transform.GetChild(0).gameObject.SetActive(false);
        }

        // 활성화된 플레이어 수에 따라 슬롯 활성화
        Debug.Log($"playerSlots.Length : {playerSlots.Length}");
        Debug.Log($"NetworkServer.connections.Count : {NetworkServer.connections.Count}");
        foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
        {
            if (index < playerSlots.Length)
            {
                Debug.Log("플레이어 접속 업데이트");
                OnRoomServerConnect(conn);
                index++;
            }
        }
        index = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    public int index = 0;
    public override void OnRoomServerConnect(NetworkConnectionToClient conn)
    {
        base.OnRoomServerConnect(conn);

        var player = Instantiate(spawnPrefabs[0]);
        NetworkServer.Spawn(player, conn);
        player.transform.SetParent(playerSlots[index].transform);
        index++;
        Debug.Log($"OnRoomServerConnect / index : {index}");
        RectTransform imageRectTransform = player.GetComponent<RectTransform>();
        imageRectTransform.anchoredPosition = new Vector2(0,40);
        
        
        //foreach (NetworkConnectionToClient connn in NetworkServer.connections.Values)
        //{
        //    if (index < playerSlots.Length)
        //    {
        //        Debug.Log("플레이어 접속 업데이트");
        //        var player = Instantiate(spawnPrefabs[0]);
        //        NetworkServer.Spawn(player, conn);
        //        player.transform.SetParent(playerSlots[index].transform);
        //        index++;
        //    }
        //}
        //index = 0;
    }

}




