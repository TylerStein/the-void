using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BridgeLevelComponent : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public new EdgeCollider2D collider;

    public Vector2 colliderStartOffset = new Vector2(-0.5f, 0.4f);
    public Vector2 colliderEndOffset = new Vector2(0.5f, 0.4f);

    public Vector2Int startPosition;
    public Vector2Int endPosiiton;

    public List<GameObject> spawnedObjects = new List<GameObject>();

    public bool rebuild = false;

    public void Update() {
        if (rebuild) {
            rebuild = false;
            Build();
        }
    }

    public Vector2Int GetAdjustedEndPosition(Vector2Int targetPosition) {
        if (targetPosition.y != startPosition.y) targetPosition.y = startPosition.y;
        return targetPosition;
    }

    public void Build() {
        Clear();

        collider = gameObject.AddComponent<EdgeCollider2D>();
        collider.points = new Vector2[] {
            startPosition + colliderStartOffset,
            endPosiiton + colliderEndOffset,
        };
    }

    public void Clear() {
        foreach (GameObject obj in spawnedObjects) {
            DestroyImmediate(obj);
        }

        spawnedObjects.Clear();
        if (collider) DestroyImmediate(collider);
    }

    public void OnDrawGizmosSelected() {
        Vector3 start = transform.position + new Vector3(startPosition.x, startPosition.y);
        Vector3 end = transform.position + new Vector3(endPosiiton.x, endPosiiton.y);

        Gizmos.DrawWireCube(start, Vector3.one);
        Gizmos.DrawWireCube(end, Vector3.one);
        Gizmos.DrawLine(start, end);
    }
}
