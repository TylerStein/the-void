using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public enum EInputSnap
{
    NONE = 0,
    DPAD = 1,
}

[System.Serializable]
public enum EDirection
{
    N = 0,
    NE = 1,
    E = 2,
    SE = 3,
    S = 4,
    SW = 5,
    W = 6,
    NW = 7
}

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Character2DMovementController movement;
    public Animator playerAnimator;
    public InputState lastInputState;
    public EInputSnap inputSnapMode;

    public EDirection inputDirection = EDirection.N;
    public float inputAngle = 0f;

    private void Update() {
        playerAnimator.SetBool("moving", movement.MoveInputX != 0);
        movement.SetInputState(lastInputState);
        if (movement.MoveInputX != 0) {
            spriteRenderer.flipX = movement.MoveInputX < 0f;
        }
    }

    public void OnMove(InputAction.CallbackContext value) {
        lastInputState.rawMoveInput = value.ReadValue<Vector2>();
        lastInputState.moveInput = GetSnapInput(lastInputState.rawMoveInput);
    }

    public Vector2 GetSnapInput(Vector2 source) {
        inputAngle = Vector2.SignedAngle(Vector2.up, lastInputState.rawMoveInput);
        switch (inputSnapMode) {
            case EInputSnap.NONE: return source;
            case EInputSnap.DPAD:
                float mag = source.magnitude;

                if (inputAngle > -22.5f && inputAngle < 22.5f) {
                    inputDirection = EDirection.N;
                    return Vector2.up * mag;
                } else if (inputAngle > -67.5f && inputAngle < -22.5f) {
                    inputDirection = EDirection.NE;
                    return new Vector2(0.5f, 0.5f) * mag;
                } else if (inputAngle > -112.5f && inputAngle < -67.5f) {
                    inputDirection = EDirection.E;
                    return Vector2.right * mag;
                } else if (inputAngle > -157.5f && inputAngle < -112.5f) {
                    inputDirection = EDirection.SE;
                    return new Vector2(0.5f, -0.5f) * mag;
                } else if ((inputAngle > -180f && inputAngle < -157.5f) || (inputAngle > 157.5f && inputAngle < 180f)) {
                    inputDirection = EDirection.S;
                    return Vector2.down * mag;
                } else if (inputAngle > 112.5 && inputAngle < 157.5) {
                    inputDirection = EDirection.SW;
                    return new Vector2(-0.5f, -0.5f) * mag;
                } else if (inputAngle > 67.5 && inputAngle < 112.5f) {
                    inputDirection = EDirection.W;
                    return Vector2.left * mag;
                } else if (inputAngle > 22.5f && inputAngle < 67.5f) {
                    inputDirection = EDirection.NW;
                    return new Vector2(-0.5f, 0.5f) * mag;
                } else {
                    return source;
                }
            default: return source;
        }
    }

    public void OnJump(InputAction.CallbackContext value) {
        if (value.started) {
            lastInputState.jumpIsDown = true;
            playerAnimator.SetBool("jumping", true);
        } else if (value.canceled) {
            lastInputState.jumpIsDown = false;
            playerAnimator.SetBool("jumping", false);
        }
    }

    public void OnDash(InputAction.CallbackContext value) {
        if (value.started) {
            lastInputState.dashIsDown = true;
        } else if (value.canceled) {
            lastInputState.dashIsDown = false;
        }
    }
}
