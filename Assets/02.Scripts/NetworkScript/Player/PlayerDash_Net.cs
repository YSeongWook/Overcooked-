using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash_Net : NetworkBehaviour
{
    [Header("References")]
    public Transform orientation;
    private Rigidbody rb;

    [Header("Dashing")]
    public float dashForce;
    public float dashDuration;
    private bool isDashing = false;

    [Header("Settings")]
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (dashCdTimer > 0) dashCdTimer -= Time.deltaTime;
    }

    public void OnDash(InputValue inputButton)
    {
        if (inputButton.isPressed && !isDashing) Dash();
    }

    private void Dash()
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;

        isDashing = true;

        Vector3 forceToApply = transform.forward * dashForce;

        if (disableGravity) rb.useGravity = false;

        StartCoroutine(ExecuteDash(forceToApply));
    }

    private IEnumerator ExecuteDash(Vector3 force)
    {
        if (resetVel) rb.velocity = Vector3.zero;

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            float dashProgress = elapsedTime / dashDuration;
            float currentForce = dashForce * Mathf.Lerp(0.5f, 1f, dashProgress);
            rb.AddForce(force.normalized * currentForce, ForceMode.Impulse);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isDashing = false;

        if (disableGravity) rb.useGravity = true;
    }
}
