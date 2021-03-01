using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Events;

[System.Serializable]
public struct Character2DMovementState
{
    [SerializeField] public Vector3 position;
    [SerializeField] public Vector2 velocity;
    [SerializeField] public bool isGrounded;
    [SerializeField] public Collider2D groundContact;
    [SerializeField] public float moveInput;
    [SerializeField] public bool jumpInput;
    [SerializeField] public bool lastJumpInput;
    [SerializeField] public bool isReversing;
    [SerializeField] public ContactFilter2D colliderContactFilter;
    [SerializeField] public Collider2D[] overlapColliders;
}

[RequireComponent(typeof(Collider2D))]
public class Character2DMovementController : MonoBehaviour
{
    public Character2DMovementSettings movementSettings;
    public Bounds worldBounds = new Bounds();
    public Transform respawn;

    [SerializeField] public Vector2 velocity = Vector2.zero;

    [SerializeField] public bool isGrounded = false;
    [SerializeField] public bool isJumping = false;
    [SerializeField] public Collider2D groundContact = null;

    [SerializeField] public float moveInput = 0f;
    [SerializeField] public bool jumpInput = false;
    [SerializeField] public bool lastJumpInput = false;

    [SerializeField] public bool isReversing = false;
    [SerializeField] public ContactFilter2D colliderContactFilter;
    [SerializeField] public Collider2D[] overlapColliders = new Collider2D[3];
    [SerializeField] public RaycastHit2D[] raycastHits = new RaycastHit2D[3];

    [SerializeField] public bool disableRespawn = false;
    [SerializeField] public int overlapFrames = 0;

    [SerializeField] public float lastHitAngle = 0f;

    private new Transform transform;
    private new BoxCollider2D collider;
    private new Rigidbody2D rigidbody;

    public UnityEvent respawnEvent = new UnityEvent();

    /**
     * Mechanic: Jump After Ledge
     * Rule: The player can jump if they are no longer grounded within a grace period
     *       The grace period only applies if the player did not jump to leave the grounded state
     *       
     * Mechanic: Hold to Jump Further
     * Rule: The player can hold down the jump button to keep an increase cap on their airborne velocity
     *       Releasing the jump button eases the max velocity down to the regular cap
     *       
     * Mechanic: Air Dash
     * Rule: The player can tap the dash button to get a brief increase of airborne velocity if not currently grounded
     *       The velocity cap decreases back to normal to complete the dash, or resets once grounded
     * ++++: When dashing into a ledge, above a certain height the player will "pop" upward to prevent collision
     * 
     * Mechanic: Ground Dash
     * Rule: The player can hold the dash button to get an increase of ground velocity
     *       Ramps up in a short peridod, heightened velocity stays with jump until landing (or the dash continues)
     *       Slowing down takes longer than normal while dashing for a challenge
     */

    /**
    * Ground Check Logic
    * 
    * apply gravity
    * set isGrounded false
    * 
    * project movement with velocity
    * for each hit
    *   if hit is below
    *       set isGrounded true
    *       set velocity y 0
    * 
    */ 

    public float MoveInput { get => moveInput; set => moveInput = value; }
    public bool JumpInput { get => jumpInput; set => jumpInput = value; }

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
        int count = Physics2D.OverlapBox(collider.bounds.center, collider.bounds.size, 0f, colliderContactFilter, overlapColliders);
        if (count > 1) {
            Debug.Log("CheckOverlap Collided with " + overlapColliders[0].name, this);
            return true;
        }

        return false;
    }

    private void Awake() {
        transform = GetComponent<Transform>();
        collider = GetComponent<BoxCollider2D>();

        // colliderLayerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = Physics2D.GetLayerCollisionMask(gameObject.layer);
        contactFilter.useTriggers = false;
        colliderContactFilter = contactFilter;
    }

    private void Update() {
        // transform.position = frameEndPosition;

        if (worldBounds.Contains(transform.position) == false && !disableRespawn) {
            respawnEvent.Invoke();
            transform.position = respawn.position;
            velocity = Vector2.zero;
        } else {
            UpdateMovement(Time.deltaTime);
        }
    }

    private void FixedUpdate() {
       // UpdateMovement(Time.fixedDeltaTime);
    }

    private void UpdatePhysics_Jumping(float deltaTime) {
        velocity += movementSettings.gravity * deltaTime;
        if (velocity.y < movementSettings.gravityMinVelocity) {
            velocity.y = movementSettings.gravityMinVelocity;
        }

        if (isGrounded) {
            // Jump input
            if (jumpInput && !isJumping) {
                isJumping = true;
                velocity.y = movementSettings.jumpForce;
            } else {
                isJumping = false;
            }

        }

        // Jump max upwards velocity
        if (isJumping) {
            velocity.y = Mathf.Min(velocity.y, movementSettings.boostJumpMaxVelocity);
        } else {
            velocity.y = Mathf.Min(velocity.y, movementSettings.jumpMaxVelocity);
        }
    }

    private void UpdatePhysics_Movement(float deltaTime) {
        // Horizontal movement
        if (moveInput != 0) {
            float targetMove = movementSettings.groundMaxVelocity * moveInput;
            isReversing = Mathf.Sign(moveInput) != Mathf.Sign(velocity.x);
            float moveSpeed = isReversing ? movementSettings.groundReverseForce : movementSettings.groundMoveForce;

            velocity.x = Mathf.MoveTowards(velocity.x, targetMove, moveSpeed * deltaTime);
        } else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, movementSettings.groundBrakeForce * deltaTime);
            isReversing = false;
        }

        //isReversing = false;
        //if (moveInput != 0) {
        //    float moveSign = Mathf.Sign(moveInput);
        //    float velSign = Mathf.Sign(velocity.x);

        //    if (moveSign > 0 && velSign < 0) {
        //        isReversing = true;
        //        velocity.x = Mathf.MoveTowards(velocity.x, movementSettings.groundMaxVelocity * moveInput, movementSettings.groundReverseForce * deltaTime);
        //    } else if (moveSign < 0 && velSign > 0) {
        //        isReversing = true;
        //        velocity.x = Mathf.MoveTowards(velocity.x, movementSettings.groundMaxVelocity * moveInput, movementSettings.groundReverseForce * deltaTime);
        //    } else {
        //        isReversing = false;
        //        velocity.x = Mathf.MoveTowards(velocity.x, movementSettings.groundMaxVelocity * moveInput, movementSettings.groundMoveForce * deltaTime);
        //    }
        //} else {
        //    velocity.x = Mathf.MoveTowards(velocity.x, 0f, movementSettings.groundBrakeForce * deltaTime);
        //}

        //if (velocity.x > 0f) {
        //    velocity.x = Mathf.Min(velocity.x, movementSettings.groundMaxVelocity);
        //} else {
        //    velocity.x = Mathf.Max(velocity.x, -movementSettings.groundMaxVelocity);
        //}
    }

    private void UpdatePhysics_Collision(float deltaTime) {
        Vector3 move = velocity * deltaTime;
        int hitCount = collider.Cast(move, raycastHits, move.magnitude);
        Debug.DrawRay(transform.position, move, Color.blue);
        
        isGrounded = false;

        // Check velocity based collider cast hit
        Vector3 lastPosition = transform.position;
        for (int i = 0; i < hitCount; i++) {
            if (raycastHits[i].collider == collider) continue;
            Debug.DrawLine(lastPosition, raycastHits[i].point, Color.magenta);
            transform.position = raycastHits[i].centroid;

            // Check overlap details for collider based hit
            Collider2D[] hits = Physics2D.OverlapBoxAll(raycastHits[i].centroid, collider.size, 0f);
            if (hits.Length == 0) {
                Debug.Log("Hits length 0"); 
                continue;
            }

            foreach (Collider2D hit in hits) {
                if (hit == collider) continue;

                ColliderDistance2D colliderDistance = hit.Distance(collider);
                Collider2D hitCollider;
                float hitAngle;

                if (colliderDistance.isOverlapped) {
                    hitCollider = hit;
                    transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
                    hitAngle = Vector2.Angle(colliderDistance.normal, Vector2.up);
                    Debug.DrawLine(colliderDistance.pointA, colliderDistance.pointB, Color.red);
                } else {
                    hitCollider = raycastHits[i].collider;
                    hitAngle = Vector2.Angle(raycastHits[i].normal, Vector2.up);
                }

                lastHitAngle = hitAngle;
                if (hit is EdgeCollider2D) {
                    if (hitAngle > 90f && velocity.y > 0f) {
                        continue;
                    }
                }

                if (hitAngle < 90f && velocity.y <= 0f) {
                    isGrounded = true;
                    velocity.y = 0f;
                }
            }
        }

        //if (hasCollision) {
        //    overlapFrames++;
        //    if (overlapFrames > 1) {
        //        Debug.Log("OverlapFrames " + overlapFrames);
        //    }
        //} else {
        //    overlapFrames = 0;
        //}
    }

    public void UpdateMovement(float deltaTime) {
        UpdatePhysics_Jumping(deltaTime);
        UpdatePhysics_Movement(deltaTime);
        UpdatePhysics_Collision(deltaTime);
        transform.position += (Vector3)velocity * deltaTime;
    }

    public Character2DMovementState ToState() {
        return new Character2DMovementState() {
            position = transform.position,
            velocity = velocity,
            isGrounded = isGrounded,
            groundContact = groundContact,
            moveInput = moveInput,
            jumpInput = jumpInput,
            lastJumpInput = lastJumpInput,
            isReversing = isReversing,
            colliderContactFilter = colliderContactFilter,
            overlapColliders = overlapColliders,
        };
    }

    public void FromState(Character2DMovementState state) {
        transform.position = state.position;
        velocity = state.velocity;
        isGrounded = state.isGrounded;
        groundContact = state.groundContact;
        moveInput = state.moveInput;
        jumpInput = state.jumpInput;
        lastJumpInput = state.lastJumpInput;
        isReversing = state.isReversing;
        colliderContactFilter = state.colliderContactFilter;
        overlapColliders = state.overlapColliders;
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
    }
}
