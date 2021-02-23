using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character2DMovementSettings", menuName = "2D Movement Settings", order = 1)]
public class Character2DMovementSettings : ScriptableObject
{
    public float groundMoveForce = 1f;
    public float jumpForce = 3f;

    public Vector2 maxVelocity = new Vector2(10f, 10f);

    public Vector2 gravity = new Vector2(0f, -9.8f);
}
