using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMasterController_Net : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> playerList;

    [Header("Current Player")]
    public GameObject currentPlayer;

    [Header("Player Input")]
    public InputActionAsset inputActionAsset;

    private void Awake()
    {
        currentPlayer = playerList[1];
        SwitchPlayerComponent();
    }

    public void SwitchPlayerComponent()
    {
        currentPlayer.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        PlayerInput oldPlayerInput = null;
        if (currentPlayer.GetComponent<PlayerInput>() != null)
            oldPlayerInput = currentPlayer.GetComponent<PlayerInput>();

        if (oldPlayerInput != null)
        {
            Destroy(oldPlayerInput);
        }

        // 현재 플레이어의 인덱스를 가져온다.
        int currentIndex = playerList.IndexOf(currentPlayer);

        // 다음 플레이어의 인덱스를 계산한다.
        int nextIndex = (currentIndex + 1) % playerList.Count;

        // 다음 플레이어를 currentPlayer로 설정한다.
        currentPlayer = playerList[nextIndex];

        PlayerInput playerInput = currentPlayer.AddComponent<PlayerInput>();

        // InputActionAsset 할당
        playerInput.actions = inputActionAsset;

        // 기본 맵 설정
        playerInput.defaultActionMap = "Player";

        // Behavior 설정 (예: Send Messages)
        playerInput.notificationBehavior = PlayerNotifications.SendMessages;

        // PlayerInput 활성화
        playerInput.ActivateInput();

        currentPlayer.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }
}
