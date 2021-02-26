using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Character2DMovementController movement;
    public Animator playerAnimator;

    public Vector2 jumpCheckOffset = new Vector2(1f, 0f);
    public float jumpCheckDistance = 3f;

    //public bool isJumping = false;
    public bool enableProjection = false;

    public const int projectionIteractions = 32;
    public float projectionTimestep = 0.16f;

    public Character2DMovementState saveState;
    public Vector2[] projectionPoints = new Vector2[projectionIteractions];

    private void Update() {
        // playerAnimator.SetBool("moving", movement.MoveInput != 0);
        saveState = movement.ToState();

        if (enableProjection) {
            movement.disableRespawn = true;

            Vector3 lastPosition = movement.transform.position;
            for (int i = 0; i < projectionIteractions; i++) {
                movement.UpdateMovement(projectionTimestep);

                Move(1f);
                if (CheckShouldJump()) {
                    Jump(true);
                } else {
                    Jump(false);
                }

                Debug.DrawLine(lastPosition, movement.transform.position, Color.white, Time.deltaTime);
                lastPosition = transform.position;
            }


            movement.disableRespawn = false;
        }

        movement.FromState(saveState);

        Move(1f);
        if (CheckShouldJump()) {
            Jump(true);
        } else {
            Jump(false);
        }
    }

    public bool CheckShouldJump() {
        return !Physics2D.Raycast((Vector2)transform.position + jumpCheckOffset, Vector2.down, jumpCheckDistance);
    }

    public void Move(float value) {
        movement.MoveInput = value;
        if (movement.MoveInput != 0) {
            spriteRenderer.flipX = movement.MoveInput < 0f;
        }
    }

    public void Jump(bool started) {
        if (started) {
            playerAnimator.SetBool("jumping", true);
            movement.BeginJump();
        } else {
            playerAnimator.SetBool("jumping", false);
            movement.EndJump();
        }
    }
}
