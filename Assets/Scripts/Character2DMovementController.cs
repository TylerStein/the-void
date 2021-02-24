using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(Collider2D))]
public class Character2DMovementController : MonoBehaviour
{
    public Character2DMovementSettings movementSettings;

    [SerializeField] private Vector2 velocity = Vector2.zero;

    [SerializeField] private bool isGrounded = false;
    [SerializeField] private Collider2D groundContact = null;

    [SerializeField] private float moveInput = 0f;
    [SerializeField] private bool jumpInput = false;
    [SerializeField] private bool lastJumpInput = false;

    [SerializeField] private bool isReversing = false;
    [SerializeField] private LayerMask colliderLayerMask;
    [SerializeField] private Collider2D[] overlapColliders = new Collider2D[1];

    private new Transform transform;
    private new BoxCollider2D collider;

    public float MoveInput { get => moveInput; set => moveInput = value; }
    public bool JumpInput { get => jumpInput; set {
            jumpInput = value;
            if (!jumpInput) lastJumpInput = false;
    } }

    public void BeginJump() {
        //
        jumpInput = true;
    }

    public void EndJump() {
        jumpInput = false;
        lastJumpInput = false;
    }

    public void Move(float horizontal) {
        moveInput = horizontal;
    }

    public void TeleportTo(Vector3 position) {
        if (CheckOverlap(position, false) == false) {
            transform.position = position;
        }
    }

    public bool CheckOverlap(Vector3 position, bool useTriggers = false) {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = colliderLayerMask;
        contactFilter.useTriggers = useTriggers;

        int count = Physics2D.OverlapBox(collider.bounds.center, collider.bounds.size, 0f, contactFilter, overlapColliders);
        if (count > 1) {
            Debug.Log("CheckOverlap Collided with " + overlapColliders[0].name, this);
            return true;
        }

        return false;
    }

    private void Awake() {
        transform = GetComponent<Transform>();
        collider = GetComponent<BoxCollider2D>();

        colliderLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
    }

    private void Update() {
        if (!isGrounded) {
            velocity += movementSettings.gravity * Time.deltaTime;
            if (velocity.y < movementSettings.gravityMinVelocity) {
                velocity.y = movementSettings.gravityMinVelocity;
            }
        }

        isReversing = false;
        if (moveInput != 0) {
            if (Mathf.Sign(moveInput) != Mathf.Sign(velocity.x)) {
                isReversing = true;
                velocity.x = Mathf.MoveTowards(velocity.x, movementSettings.groundMaxVelocity * moveInput, movementSettings.groundReverseForce * Time.deltaTime);
            } else {
                velocity.x = Mathf.MoveTowards(velocity.x, movementSettings.groundMaxVelocity * moveInput, movementSettings.groundMoveForce * Time.deltaTime);
            }
        } else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, movementSettings.groundBrakeForce * Time.deltaTime);
        }

        if (jumpInput && isGrounded) {
            if (!lastJumpInput) {
                velocity.y = movementSettings.jumpForce;
            }
            lastJumpInput = true;
        }

        transform.position += (Vector3)(velocity * Time.deltaTime);

        isGrounded = false;
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, collider.size, 0f);
        foreach (Collider2D hit in hits) {
            if (hit == collider) continue;

            ColliderDistance2D colliderDistance = hit.Distance(collider);
            if (colliderDistance.isOverlapped) {
                Debug.DrawLine(colliderDistance.pointA, colliderDistance.pointB, Color.red);
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                float hitAngle = Vector2.Angle(colliderDistance.normal, Vector2.up);
                if (hitAngle < 90f && velocity.y <= 0f) {
                    isGrounded = true;
                    velocity.y = 0f;
                }
            }
        }
    }

    public void OnDrawGizmos() {
        if (collider != null) {
            //
        }
    }
}
