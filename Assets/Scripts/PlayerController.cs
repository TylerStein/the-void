using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Character2DMovementController movement;
    public Animator playerAnimator;
    public InputState lastInputState;

    private void Update() {
        playerAnimator.SetBool("moving", movement.MoveInputX != 0);
        movement.SetInputState(lastInputState);
        if (movement.MoveInputX != 0) {
            spriteRenderer.flipX = movement.MoveInputX < 0f;
        }
    }

    public void OnMove(InputAction.CallbackContext value) {
        lastInputState.moveInput = value.ReadValue<Vector2>();
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
