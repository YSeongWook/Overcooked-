using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    public Animator animator;
    private Rigidbody rb;

    [Header("Speed")]
    public float moveSpeed = 15f;
    public float turnSpeed = 10f;
    public float fallMultiplier = 2.5f; // 떨어질 때 속도 증가
    public float downforce = 1f;

    private Vector2 moveInput;
    private bool isDashing = false;
    private int puffCount = 5;

    [Header("State")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isOnStairs;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        puffCount++;

        // 이동 입력이 없을 때 애니메이터의 이동 파라미터를 false로 설정
        if (moveInput == Vector2.zero)
        {
            rb.velocity = Vector3.zero;
            animator.SetBool("isWalking", false);
            puffCount = 5;
        }
        else
        {
            animator.SetBool("isWalking", true);
            if (puffCount >= 5)
            {
                PlayerPuff.Instance.MovePuff(transform);
                puffCount = 0;
            }
        }
    }

    void FixedUpdate()
    {
        CheckGroundStatus();
        CheckStairStatus();

        if (!isDashing && moveInput != Vector2.zero)
        {
            MoveCharacter();
        }

        ApplyAdditionalGravity();
        HandleStairMovement();
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, downforce);
    }

    private void CheckStairStatus()
    {
        isOnStairs = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, downforce) && hit.collider.CompareTag("Stair");
    }

    private void MoveCharacter()
    {
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Vector3 movement = moveDir * moveSpeed;
        rb.velocity = movement;

        // 플레이어가 이동하는 방향을 바라보도록 회전
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void ApplyAdditionalGravity()
    {
        if (!isGrounded)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        }
    }

    private void HandleStairMovement()
    {
        if (isOnStairs)
        {
            if (moveInput != Vector2.zero)
            {
                rb.AddForce(Vector3.down * 5f, ForceMode.Acceleration);
            }
            else
            {
                rb.useGravity = false;
            }
        }
        else
        {
            rb.useGravity = true;
        }
    }

    public void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * downforce);
    }

    void OnCollisionEnter(Collision other)
    {
        // Draw a different colored ray for every normal in the collision
        foreach (var item in other.contacts)
        {
            Debug.DrawRay(item.point, item.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
        }
    }

    public void OnPickup()
    {
        // 요리 재료 줍기 또는 놓기 로직
        Debug.Log("Pickup or Place Item");
    }

    public void OnCook()
    {
        // 요리하기 또는 재료 던지기 로직
        Debug.Log("Cook or Throw Item");
    }

    public void OnDash()
    {
        // 대쉬 로직
        if (!isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        float dashTime = 0.5f;
        float dashSpeed = 25f;

        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            rb.velocity = new Vector3(moveInput.x, 0f, moveInput.y) * dashSpeed;
            yield return null;
        }

        isDashing = false;
    }
}
