using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character2DMovementSettings", menuName = "2D Movement Settings", order = 1)]
public class Character2DMovementSettings : ScriptableObject
{
    public float jumpForce = 50f;

    public float groundMoveForce = 10f;
    public float groundReverseForce = 100f;
    public float groundBrakeForce = 50f;
    public float groundMaxVelocity = 20f;

    public float airMoveForce = 12f;
    public float airReverseForce = 100f;
    public float airBrakeForce = 50f;
    public float airMaxVelocity = 12f;

    public Vector2 gravity = new Vector2(0f, -9.8f);
    public float gravityMinVelocity = -30f;
    public float boostJumpMaxVelocity = 30f;
    public float jumpMaxVelocity = 15f;
    public float jumpReturnSpeed = 15f;

    public float minStaticGroundDistance = 0.001f;
}
