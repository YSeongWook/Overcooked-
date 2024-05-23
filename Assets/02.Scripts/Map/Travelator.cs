using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Travelator : MonoBehaviour
{
    public float speed = 1.0f; // 트레블레이터의 기본 이동 속도
    public bool moveRight = true; // 오른쪽으로 이동할지 여부. false이면 왼쪽으로 이동

    private Vector3 moveDirection;
    private Dictionary<Rigidbody, Coroutine> activeCoroutines = new Dictionary<Rigidbody, Coroutine>(); // 현재 이동 중인 오브젝트와 코루틴

    private void Start()
    {
        // 이동 방향 설정
        moveDirection = moveRight ? Vector3.right : Vector3.left;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 트레블레이터 위에 올라간 오브젝트가 Rigidbody를 가지고 있는지 확인
        if (other.attachedRigidbody != null)
        {
            if (!activeCoroutines.ContainsKey(other.attachedRigidbody))
            {
                Coroutine moveCoroutine;

                if (other.CompareTag("Player"))
                {
                    // 플레이어의 경우 별도 처리
                    moveCoroutine = StartCoroutine(MovePlayer(other.attachedRigidbody));
                }
                else
                {
                    moveCoroutine = StartCoroutine(MoveObject(other.attachedRigidbody));
                }

                activeCoroutines[other.attachedRigidbody] = moveCoroutine;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 트레블레이터를 벗어난 오브젝트가 Rigidbody를 가지고 있는지 확인
        if (other.attachedRigidbody != null && activeCoroutines.ContainsKey(other.attachedRigidbody))
        {
            StopCoroutine(activeCoroutines[other.attachedRigidbody]);
            activeCoroutines.Remove(other.attachedRigidbody);

            if (other.CompareTag("Player"))
            {
                other.attachedRigidbody.velocity = Vector3.zero;
            }
            else
            {
                other.attachedRigidbody.velocity = Vector3.zero;
            }
        }
    }

    private IEnumerator MoveObject(Rigidbody obj)
    {
        while (true)
        {
            // Rigidbody의 위치를 이동 방향으로 업데이트
            obj.MovePosition(obj.position + new Vector3(moveDirection.x * speed * Time.deltaTime, 0, 0));
            yield return null; // 다음 프레임까지 대기
        }
    }

    private IEnumerator MovePlayer(Rigidbody playerRb)
    {
        while (true)
        {
            // 플레이어의 속도와 관계없이 트레블레이터 방향으로 이동
            playerRb.MovePosition(playerRb.position + new Vector3(moveDirection.x * speed * Time.deltaTime, 0, 0));
            yield return null; // 다음 프레임까지 대기
        }
    }

    private void OnDrawGizmos()
    {
        // 이동 방향을 표시하기 위한 Gizmo 화살표 그리기
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + moveDirection);
        Gizmos.DrawSphere(transform.position + moveDirection, 0.1f);
    }
}
