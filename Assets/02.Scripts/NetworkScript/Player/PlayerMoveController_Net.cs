using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController_Net : NetworkBehaviour
{
    public Animator animator;
    public Rigidbody rb;

    [Header("Speed")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashForce = 500f;
    [SerializeField] private float dashDuration = 0.5f; // 대쉬 지속 시간
    [SerializeField] private float dashCooldown = 0.5f; // 대쉬 쿨다운 시간

    private Vector2 moveInput;
    private bool isDashing = false;
    private float dashStartTime;
    private float lastDashTime;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        dashSpeed = moveSpeed * 2;
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

        /*
        // 대쉬가 끝난 경우
        if (isDashing && Time.time - dashStartTime >= dashDuration)
        {
            isDashing = false;
            rb.velocity = Vector3.zero; // 대쉬가 끝나면 플레이어의 속도를 0으로 설정하여 멈춤
        }
        */
    }

    void FixedUpdate()
    {
        if (!isDashing && moveInput.magnitude != 0)
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
            Vector3 movement = moveDir * moveSpeed;
            rb.velocity = movement;

            // 플레이어가 이동하는 방향을 바라보도록 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        if(isDashing)
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    /*
    // 현재 요리 중이어도 대쉬 가능, 카운터에서 멀어지면 채널링 해제되서 요리 중단
    public void OnDash(InputValue inputValue)
    {
        if (inputValue.isPressed && Time.time - lastDashTime >= dashCooldown)
        {
            isDashing = true;
            lastDashTime = Time.time;
            dashStartTime = Time.time;
            rb.velocity = Vector3.zero;
            rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
            StartCoroutine(ControlSpeed());
            Invoke("OffIsDash", 0.5f);
        }
    }

    IEnumerator ControlSpeed()
    {
        while (dashSpeed > moveSpeed)
        {
            dashSpeed -= Time.deltaTime * 20;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return null;
        dashSpeed = 2 * moveSpeed;
    }

    private void OffIsDash()
    {
        isDashing = false;
    }
    */
}
