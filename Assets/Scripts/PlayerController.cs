using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Character2DMovementController movement;

    public void OnMove(InputAction.CallbackContext value) {
        movement.MoveInput = value.ReadValue<Vector2>().x;
    }

    public void OnJump(InputAction.CallbackContext value) {
        if (value.started) {
            movement.BeginJump();
        } else if (value.canceled) {
            movement.EndJump();
        }
    }
}
