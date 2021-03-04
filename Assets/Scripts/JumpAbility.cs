using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAbility : Character2DMovementAbility
{
    public override int SortOrder { get => sortOrder; }
    [SerializeField] public int sortOrder = 0;
    [SerializeField] private Character2DMovementController movementController;

    [SerializeField] public float boostJumpMaxVelocity = 30f;
    [SerializeField] public float jumpMaxVelocity = 15f;
    [SerializeField] public float jumpReturnSpeed = 15f;
    [SerializeField] public float jumpForce = 50f;

    [SerializeField] private bool jumpInput = false;
    [SerializeField] private bool jumpInputDown = false;
    [SerializeField] private bool isBoosting = false;
    [SerializeField] private bool isJumping = false;

    public override void SetInputState(InputState inputState) {
        if (jumpInput) {
            jumpInputDown = false;
        }

        if (inputState.jumpIsDown) {
            if (!jumpInput) {
                jumpInputDown = true;
            }

            jumpInput = true;
        } else {
            jumpInputDown = false;
            jumpInput = false;
            isBoosting = false;
        }
    }

    public override void UpdatePreMovement(float deltaTime) {
        if (movementController.isGrounded) {
            isBoosting = false;
            isJumping = false;
        }
    }

    public override void UpdateTargetVelocities(float deltaTime, ref Vector2 targetVelocity, ref Vector2 changeSpeed, ref Vector2 minVelocity, ref Vector2 maxVelocity) {
        if (jumpInputDown && movementController.isGrounded && isJumping == false) {
            isJumping = true;
            isBoosting = true;
            changeSpeed.y = jumpForce;
            targetVelocity.y = boostJumpMaxVelocity;
        } else if (isJumping) {
            if (isBoosting) {
                maxVelocity.y = boostJumpMaxVelocity;
            } else {
                maxVelocity.y = jumpMaxVelocity;
            }
        }
    }

    public override void OnGrounded(float deltaTime, ref Vector2 velocity, ref Vector3 position) {
        isJumping = false;
        isBoosting = false;
    }
}
