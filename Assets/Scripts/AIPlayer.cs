using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Projection Loop:
 *  - Step Movement with Forward Movement
 *  - Check if can jump:
 *      - Step Jump with Forward Movement
 *      - Check if successful landing
 *  - Jump if should jump else continue
 */

public class AIPlayer : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Character2DMovementController movement;
    public Animator playerAnimator;

    public Vector2 jumpCheckOffset = new Vector2(1f, 0f);
    public float jumpCheckDistance = 3f;

    //public bool isJumping = false;
    public bool enableProjection = false;

    public int projectionIterations = 32;
    public float projectionTimestep = 0.16f;

    public Character2DMovementState saveState;
    //public Vector2[] projectionPoints = new Vector2[projectionIteractions];

    private void Update() {
        //if (projectionPoints.Length != projectionIteractions) {
        //    projectionPoints = new Vector2[projectionIteractions];
        //}

        // AI Stuff
        if (enableProjection) {
            bool canJump = movement.isGrounded && movement.velocity.y <= 0f;

            bool doMove, doJump;
            ProjectMovement_v1(out doMove, out doJump);

            if (doMove) {
                movement.Move(Vector2.right);
            }

            if (canJump && doJump) {
                movement.BeginJump();
                playerAnimator.SetBool("jumping", true);
            } else {
                movement.EndJump();
                playerAnimator.SetBool("jumping", false);
            }
        }

        // Animation stuff
        if (movement.MoveInputX != 0) {
            spriteRenderer.flipX = movement.MoveInputX < 0f;
            playerAnimator.SetBool("moving", true);
        } else {
            playerAnimator.SetBool("moving", false);
        }
    }

    private void ProjectMovement_v1(out bool doMove, out bool doJump) {
        saveState = movement.ToState();
        movement.disableRespawn = true;

        doMove = true;
        doJump = false;

        Vector3 lastPosition = movement.transform.position;
        for (int i = 0; i < projectionIterations; i++) {
            bool canJump = movement.velocity.y <= 0f && movement.isGrounded;
            bool projectJump = false;

            if (canJump) {
                Vector3 lastJumpPosition = movement.transform.position;
                Character2DMovementState jumpSave = movement.ToState();
                for (int j = 0; j < projectionIterations; j++) {
                    if (j == 0) {
                        movement.BeginJump();
                    } else if (movement.velocity.y <= 0f && movement.isGrounded) {
                        movement.EndJump();
                        projectJump = true;
                        break;
                    }

                    movement.Move(Vector2.right);
                    movement.UpdateMovement(projectionTimestep);

                    Debug.DrawLine(lastJumpPosition, movement.transform.position, Color.yellow, Time.deltaTime);
                    lastJumpPosition = movement.transform.position;
                }
                movement.FromState(jumpSave);
            }

            if (projectJump) {
                if (i == 0) {
                    doJump = true;
                    break;
                } else {
                    movement.BeginJump();
                }
            }

            movement.Move(Vector2.right);
            movement.UpdateMovement(projectionTimestep);

            Debug.DrawLine(lastPosition, movement.transform.position, Color.white, Time.deltaTime);
            lastPosition = transform.position;
        }

        movement.FromState(saveState);
        movement.disableRespawn = false;
    }

    private bool ProjectJump(ref int currentIterations, int maxIterations) {
        Character2DMovementState saveState = movement.ToState();

        bool safeJump = false;
        bool didJump = false;

        Vector3 lastPosition = movement.transform.position;
        for (int i = currentIterations; i < maxIterations; i++) {
            if (movement.isGrounded) {
                if (didJump) {
                    safeJump = true;
                    break;
                } else {
                    movement.BeginJump();
                }
            }

            if (!didJump) {
                movement.BeginJump();
            }


            Move(1f);
            movement.UpdateMovement(projectionTimestep);

            Debug.DrawLine(lastPosition, movement.transform.position, Color.yellow, Time.deltaTime);
            lastPosition = transform.position;
        }


        movement.FromState(saveState);
        return safeJump;
    }

    public bool CheckShouldJump() {
        return !Physics2D.Raycast((Vector2)transform.position + jumpCheckOffset, Vector2.down, jumpCheckDistance);
    }

    public void Move(float value) {
        movement.MoveInputX = value;
        if (movement.MoveInputX != 0) {
            spriteRenderer.flipX = movement.MoveInputX < 0f;
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
