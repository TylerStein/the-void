using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(Collider2D))]
public class Character2DMovementController : MonoBehaviour
{
    public Character2DMovementSettings movementSettings;
    private new Transform transform;

    public Vector2 velocity = Vector2.zero;
    public Vector2 force = Vector2.zero;
    public Vector2 moveInput = Vector2.zero;

    public float ground = 0f;

    private void Awake() {
        transform = GetComponent<Transform>();
    }

    private void OnDisable() {
        //
    }

    private void Update() {
        force.x += moveInput.x * movementSettings.groundMoveForce * Time.deltaTime;

        if (force.x == 0f && velocity.x != 0f) {
            velocity.x = 0f;
        }

        force += movementSettings.gravity * Time.deltaTime;

        Vector3 projectedVelocity = velocity + force * Time.deltaTime;
        force = Vector2.zero;

        projectedVelocity.x = Mathf.Clamp(projectedVelocity.x, -movementSettings.maxVelocity.x, movementSettings.maxVelocity.x);
        projectedVelocity.y = Mathf.Clamp(projectedVelocity.y, -movementSettings.maxVelocity.y, movementSettings.maxVelocity.y);

        Vector3 projectedPosition = transform.position + projectedVelocity;
        if (projectedPosition.y < ground) {
            projectedPosition.y = ground;
            projectedVelocity.y = 0f;
        }

        velocity = projectedVelocity;
        transform.position = projectedPosition;
    }

    public void Jump(InputAction.CallbackContext context) {
        force.y = movementSettings.jumpForce;
    }

    public void Move(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }
}
