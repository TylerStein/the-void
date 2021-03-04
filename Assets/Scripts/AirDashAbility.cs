using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDashAbility : Character2DMovementAbility
{
    public override int SortOrder { get => sortOrder; }
    [SerializeField] public int sortOrder = 0;
    [SerializeField] private Character2DMovementController movementController;

    [SerializeField] public float dashDuration = 0.15f;
    [SerializeField] public float dashMaxVelocity = 30f;
    [SerializeField] public float dashForce = 50f;

    [SerializeField] private Vector2 moveInput = Vector2.zero;
    [SerializeField] private bool dashInput = false;
    [SerializeField] private bool dashInputDown = false;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool hasDashCharge = true;

    [SerializeField] private Vector2 currentMaxVelocity = Vector2.zero;
    [SerializeField] private float dashTimer = 0f;

    public override void SetInputState(InputState inputState) {
        moveInput = inputState.moveInput;

        if (dashInput) {
            dashInputDown = false;
        }

        if (inputState.dashIsDown) {
            if (!dashInput) {
                dashInputDown = true;
            }

            dashInput = true;
        } else {
            dashInputDown = false;
            dashInput = false;
        }
    }

    public override void UpdateTargetVelocities(float deltaTime, ref Vector2 targetVelocity, ref Vector2 changeSpeed, ref Vector2 minVelocity, ref Vector2 maxVelocity) {
        if (dashInputDown && movementController.isGrounded == false && isDashing == false && hasDashCharge) {
            isDashing = true;
            hasDashCharge = false;
            targetVelocity = moveInput.normalized * dashMaxVelocity;
            changeSpeed.x = dashForce;
            changeSpeed.y = dashForce;
            dashTimer = 0f;
            currentMaxVelocity = new Vector2(dashMaxVelocity, dashMaxVelocity);
        }
        
        if (isDashing) {
            maxVelocity.x = currentMaxVelocity.x;
            maxVelocity.y = currentMaxVelocity.y;
            minVelocity.x = -currentMaxVelocity.x;
            minVelocity.y = -currentMaxVelocity.y;
        }
    }

    public override void UpdatePostMovement(float deltaTime) {
        if (isDashing) {
            dashTimer += deltaTime;
            if (dashTimer >= dashDuration) {
                dashTimer = 0f;
                isDashing = false;
            }
        }
    }

    public override void OnGrounded(float deltaTime, ref Vector2 velocity, ref Vector3 position) {
        isDashing = false;
        hasDashCharge = true;
    }
}
