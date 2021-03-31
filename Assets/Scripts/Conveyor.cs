using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public Vector2 addVelocity = new Vector2(1f, 0f);
    public void UpdateContact(Character2DMovementController controller) {
        if (controller.transform.position.y > transform.position.y) {
            controller.velocity.x += addVelocity.x * Time.deltaTime;
        }
    }
}
