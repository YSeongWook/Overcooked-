using System.Collections;
using UnityEngine;

public class WizardPortal : MonoBehaviour
{
    public Transform targetPortal;
    public bool isTeleport;
    public float currentMoveSpeed;
    public float currentDashForce;

    private void Start()
    {
         isTeleport = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&!isTeleport)
        {
            isTeleport=true;
            StartCoroutine(TeleportPlayer(other.transform));
        }
    }

    private IEnumerator TeleportPlayer(Transform player)
    {
        // 플레이어 스케일 줄이기]
        
        float scaleDuration = 0.5f;
        Vector3 originalScale = player.localScale;
        Vector3 targetScale = Vector3.zero;
        PlayerDash dash = player.GetComponent<PlayerDash>();
        currentDashForce = dash.dashForce;
        dash.dashForce = 0;

        PlayerMoveController moveController = player.GetComponent<PlayerMoveController>();
        currentMoveSpeed = moveController.moveSpeed;
        moveController.moveSpeed= 0;
        yield return ScaleOverTime(player, originalScale, targetScale, scaleDuration);

        // 플레이어 위치 이동
        player.position = targetPortal.position;

        // 플레이어 스케일 다시 키우기
        yield return ScaleOverTime(player, targetScale, originalScale, scaleDuration);
        isTeleport = false;
        dash.dashForce = currentDashForce;
        moveController.moveSpeed = currentMoveSpeed;
    }

    private IEnumerator ScaleOverTime(Transform obj, Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            obj.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.localScale = endScale;
    }
}

