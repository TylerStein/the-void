using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Character2DMovementState
{
    [SerializeField] public Vector3 position;
    [SerializeField] public Vector2 velocity;
    [SerializeField] public bool isGrounded;
    [SerializeField] public Collider2D groundContact;
    [SerializeField] public Vector2 moveInput;
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

    [SerializeField] public Vector2 moveInput = Vector2.zero;
    [SerializeField] public bool jumpInput = false;

    [SerializeField] public bool dashInput = false;
    [SerializeField] public bool lastDashInput = false;

    [SerializeField] public bool isGroundDashing = false;
    [SerializeField] public bool isAirDashing = false;

    [SerializeField] public bool lastJumpInput = false;
    [SerializeField] public float maxJumpVelocity = 0f;

    [SerializeField] public float lastDashDir = 0f;
    [SerializeField] public bool isReversing = false;
    [SerializeField] public ContactFilter2D colliderContactFilter;
    [SerializeField] public Collider2D[] overlapColliders = new Collider2D[3];
    [SerializeField] public RaycastHit2D[] raycastHits = new RaycastHit2D[3];

    [SerializeField] public bool disableRespawn = false;
    [SerializeField] public float lastHitAngle = 0f;

    [SerializeField] public bool debugMovement = false;
    [SerializeField] public float debugMovementDuration = 1.5f;

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
     * Mechanic: Ground Dash
     *  Activate: Dash Button (Gamepad East / Keyboard Shift)
     *  Requires: Not already dashing, moving in a direction, on the ground
     *  Affords:  The player's target x velocity is increased
     *  Ends:     The player releases dash or leaves the ground
     * 
     * Mechanic: Air Dash
     *  Activate: Dash button (Gamepad East / Keyboard Shift)
     *  Requires: Not already dashing, moving in a direction, in the air
     *  Affords:  The player's velocity is boosted in the aimed direction
     *  Ends:     The player lands
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

    public Vector2 MoveInput { get => moveInput; set => moveInput = value; }
    public float MoveInputX { get => moveInput.x; set => moveInput.x = value; }
    public float MoveInputY { get => moveInput.y; set => moveInput.y = value; }

    public bool JumpInput { get => jumpInput; set => jumpInput = value; }

    public void BeginJump() {
        //
        jumpInput = true;
    }

    public void EndJump() {
        jumpInput = false;
        lastJumpInput = false;
    }

    public void BeginDash() {
        dashInput = true;
    }

    public void EndDash() {
        dashInput = false;
        lastDashInput = false;
    }

    public void Move(Vector2 input) {
        moveInput = input;
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
        if (worldBounds.Contains(transform.position) == false && !disableRespawn) {
            respawnEvent.Invoke();
            transform.position = respawn.position;
            velocity = Vector2.zero;
        } else {
            UpdateMovement(Time.deltaTime);
        }
    }

    public void UpdateMovement(float deltaTime) {
        UpdatePhysics_Forces(deltaTime);
      //  UpdatePhysics_ClampVelocity(deltaTime);
        UpdatePhysics_Collision(deltaTime);
    }

    private void UpdatePhysics_Forces(float deltaTime) {
        // Vertical movement
        velocity += movementSettings.gravity * deltaTime;
        if (isGrounded) {
            if (jumpInput && !lastJumpInput) {
                isJumping = true;
                maxJumpVelocity = movementSettings.boostJumpMaxVelocity;
                velocity.y = movementSettings.jumpForce;
            }
        }

        if (!jumpInput) {
            lastJumpInput = false;
        } else {
            lastJumpInput = true;
        }

        if (!isAirDashing && !lastDashInput && dashInput && !isGrounded) {
            // Air Dash
            isAirDashing = true;
            lastDashInput = true;

            velocity = moveInput * movementSettings.airDashForce;
            return;
        }

        // Horizontal movement
        float brakeForce = isGrounded ? movementSettings.groundBrakeForce : movementSettings.airBrakeForce;
        if (moveInput.x != 0) {
            float maxVelocity = isGrounded ? movementSettings.groundMaxVelocity : movementSettings.airMaxVelocity;
            float reverseForce = isGrounded ? movementSettings.groundReverseForce : movementSettings.airReverseForce;
            float targetMove = maxVelocity * moveInput.x;
            isReversing = Mathf.Sign(moveInput.x) != Mathf.Sign(velocity.x);

            float moveSpeed = isReversing ? reverseForce : brakeForce;
            velocity.x = Mathf.MoveTowards(velocity.x, targetMove, moveSpeed * deltaTime);
        } else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0f, brakeForce * deltaTime);
            isReversing = false;
        }
    }
    
    private void UpdatePhysics_ClampVelocity(float deltaTime) {
        float maxVelocity = isGrounded ? movementSettings.groundMaxVelocity : movementSettings.airMaxVelocity;

        if (isAirDashing) {

        } else if (isGroundDashing) {

        } else if (isGrounded) {
            // Clamp ground move
            velocity.x = Mathf.Clamp(velocity.x, -maxVelocity, maxVelocity);
        } else {
            // Clamp normal air move
            velocity.x = Mathf.Clamp(velocity.x, -movementSettings.airMaxVelocity, movementSettings.airMaxVelocity);
        }

        if (!isJumping || !jumpInput) {
            maxJumpVelocity = Mathf.MoveTowards(maxJumpVelocity, movementSettings.jumpMaxVelocity, deltaTime * movementSettings.jumpReturnSpeed);
        }

        velocity.y = Mathf.Clamp(velocity.y, movementSettings.gravityMinVelocity, maxJumpVelocity);
    }

    private void UpdatePhysics_Collision(float deltaTime) {
        Vector3 move = velocity * Time.deltaTime;
        int hitCount = collider.Cast(move.normalized, colliderContactFilter, raycastHits, move.magnitude);
        isGrounded = false;
        for (int i = 0; i < hitCount; i++) {
            if (raycastHits[i].collider == collider) continue;

            float hitAngle = Vector2.Angle(raycastHits[i].normal, Vector2.up);
            lastHitAngle = hitAngle;

            // Move up through ground
            if (raycastHits[i].collider is EdgeCollider2D) {
                if (hitAngle > 90f && velocity.y > 0f) {
                    continue;
                }
            }

            // Hit something solid
            transform.position = raycastHits[i].centroid;
            if (hitAngle < 90f) {
                // Hit the ground, make sure we're not trying to jump
                if (velocity.y <= 0f) {
                    isGrounded = true;
                    isJumping = false;
                    isAirDashing = false;
                    velocity.y = 0f;
                    move.y = 0f;
                }
            }
        }

        if (debugMovement) {
            Debug.DrawLine(transform.position, transform.position + move, Color.yellow, debugMovementDuration);
        }
        transform.position += move;
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
