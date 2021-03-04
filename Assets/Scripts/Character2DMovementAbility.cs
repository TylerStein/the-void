using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character2DMovementAbility : MonoBehaviour
{
    public virtual int SortOrder { get => 0; }

    public virtual void SetInputState(InputState inputState) { }
    public virtual void UpdatePreMovement(float deltaTime) { }
    public virtual void UpdateTargetVelocities(float deltaTime, ref Vector2 targetVelocity, ref Vector2 changeSpeed, ref Vector2 minVelocity, ref Vector2 maxVelocity) { }
    public virtual void UpdateVelocity(float deltaTime, ref Vector2 velocity, ref Vector2 minVelocity, ref Vector2 maxVelocity) { }
    public virtual void UpdatePostMovement(float deltaTime) { }
    public virtual void OnGrounded(float deltaTime, ref Vector2 velocity, ref Vector3 position) { }
    public virtual void OnFalling(float deltaTime, ref Vector2 velocity, ref Vector3 position) { }
}
