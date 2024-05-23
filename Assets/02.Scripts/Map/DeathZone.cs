using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그를 가진 오브젝트가 트리거에 닿았는지 확인
        if (other.CompareTag("Player"))
        {
            PlayerDeath(other.gameObject);
        } 
        else
        {
            Destroy(other.gameObject);
        }
    }

    private void PlayerDeath(GameObject player)
    {
        // 플레이어 사망 처리
        Debug.Log("플레이어 사망");
        player.SetActive(false);
        // 개인 플레이(2개 캐릭터 컨트롤 시) 사망하면 캐릭터 전환
        // 플레이어 스폰 위치에 5초 카운트 UI 활성화
        // 연기 생성 후 캐릭터 스폰
            
    }
}
