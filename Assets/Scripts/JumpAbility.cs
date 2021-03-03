using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAbility : MonoBehaviour
{
    [SerializeField] private Character2DMovementController movementController;

    [SerializeField] private bool jumpInput;
    [SerializeField] private bool lastJumpInput;
    [SerializeField] private bool isJumping;

    public void UpdatePlayerInput(InputState inputState) {
        // TODO: Set jumpInput
    }

    public void OnGrounded() {

    }

    public void OnFalling() {

    }
}
