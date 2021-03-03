using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Character2DMovementController movement;
    public Animator playerAnimator;

    private void Update() {
        playerAnimator.SetBool("moving", movement.MoveInputX != 0);
    }

    public void OnMove(InputAction.CallbackContext value) {
        movement.MoveInput = value.ReadValue<Vector2>();
        if (movement.MoveInputX != 0) {
            spriteRenderer.flipX = movement.MoveInputX < 0f;
        }
    }

    public void OnJump(InputAction.CallbackContext value) {
        if (value.started) {
            playerAnimator.SetBool("jumping", true);
            movement.BeginJump();
        } else if (value.canceled) {
            playerAnimator.SetBool("jumping", false);
            movement.EndJump();
        }
    }

    public void OnDash(InputAction.CallbackContext value) {
        if (value.started) {
            movement.BeginDash();
        } else if (value.canceled) {
            movement.EndDash();
        }
    }
}
