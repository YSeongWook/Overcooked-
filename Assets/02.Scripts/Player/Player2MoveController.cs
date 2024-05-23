using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player2MoveController : MonoBehaviour
{
    public Animator animator;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float turnSpeed = 10f;

    private Vector2 moveInput;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        // 이동 입력이 없을 때 애니메이터의 이동 파라미터를 false로 설정
        if (moveInput.magnitude == 0)
        {
            rb.velocity = Vector3.zero;
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }
    }

    void FixedUpdate()
    {
        if(moveInput.magnitude != 0)
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
            Vector3 movement = moveDir * moveSpeed;
            rb.velocity = movement;

            // transform.forward = moveDir;

            // 플레이어가 이동하는 방향을 바라보도록 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    public void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    public void OnDash(InputValue inputValue)
    {
        // 일정 거리
        
    }
}
