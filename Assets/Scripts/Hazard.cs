using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public PlayerController player;

    public void Awake() {
        if (!player) player = FindObjectOfType<PlayerController>();
    }

    public void UpdateContact(Character2DMovementController controller) {
        controller.Respawn();
    }
}
