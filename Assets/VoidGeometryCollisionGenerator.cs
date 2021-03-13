using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class VoidGeometryCollisionGenerator : MonoBehaviour
{
    public bool regenerate = false;

    public Vector2 topStartOffset = new Vector2(0f, 1f);
    public Vector2 topEndOffset = new Vector2(1f, 1f);

    public Vector2 botStartOffset = new Vector2(0f, -1f);
    public Vector2 botEndOffset = new Vector2(1f, -1f);

    public Vector2 leftStartOffset = new Vector2(0f, 0f);
    public Vector2 leftEndOffset = new Vector2(0f, 0f);

    public Vector2 rightStartOffset = new Vector2(0f, 0f);
    public Vector2 rightEndOffset = new Vector2(0f, 0f);

    private Tilemap tilemap;
    private GameObject colliderRoot;

    private void Update() {
        if (regenerate) {
            regenerate = false;
            Generate();
        }
    }

    // Start is called before the first frame update
    void Start() {
        tilemap = GetComponent<Tilemap>();
        Generate();
    }

    void Generate() {
        if (colliderRoot) {
            Destroy(colliderRoot);
        }

        colliderRoot = new GameObject("VoidGeometryColliders");
        colliderRoot.transform.position = Vector3.zero;

        Vector3Int tilemapSize = tilemap.size;
        Vector3Int tilemapOrigin = tilemap.origin;

        TileBase tileBase = null;

        TileBase aboveTile = null;
        TileBase belowTile = null;

        TileBase leftTile = null;
        TileBase rightTile = null;

        BoundsInt tilemapBounds = new BoundsInt();
        tilemapBounds.size = tilemapSize;
        tilemapBounds.position = tilemapOrigin;

        int invalidX = tilemapBounds.min.x - 1;
        int invalidY = tilemapBounds.min.y - 1;

        int groupStartTop = invalidX;
        int groupStartBot = invalidX;

        int groupStartLeft = invalidY;
        int groupStartRight = invalidY;

        // Vertical Colliders
        Vector3Int pos = new Vector3Int(tilemapBounds.min.x, tilemapBounds.min.y, 0);
        for (int x = tilemapBounds.min.x; x <= tilemapBounds.max.x; x++) {
            pos.x = x;

            for (int y = tilemapBounds.min.y; y <= tilemapBounds.max.y; y++) {
                pos.y = y;
                tileBase = tilemap.GetTile(pos);
                bool useLeft = false;
                bool useRight = false;

                if (tileBase != null && tileBase is VoidGeometryTile) {
                    useLeft = (tileBase as VoidGeometryTile).useLeftCollider;
                    if (useLeft) {
                        leftTile = tilemap.GetTile(pos + Vector3Int.left);
                        if (leftTile != null) useLeft = false;
                    }

                    useRight = (tileBase as VoidGeometryTile).useRightCollider;
                    if (useRight) {
                        rightTile = tilemap.GetTile(pos + Vector3Int.right);
                        if (rightTile != null) useRight = false;
                    }
                }

                if (groupStartLeft == invalidY && useLeft) {
                    // Start left vertical group
                    groupStartLeft = y;
                } else if (groupStartLeft != invalidY && !useLeft) {
                    // End left vertical group
                    CreateEdgeCollider(
                        new Vector2(x, groupStartLeft) + leftStartOffset,
                        new Vector2(x, y) + leftEndOffset
                    );
                    groupStartLeft = invalidY;
                }

                if (groupStartRight == invalidY && useRight) {
                    // Start right vertical group
                    groupStartRight = y;
                } else if (groupStartRight != invalidY && !useRight) {
                    // End right vertical group
                    CreateEdgeCollider(
                        new Vector2(x, groupStartRight) + rightStartOffset,
                        new Vector2(x, y) + rightEndOffset
                    );
                    groupStartRight = invalidY;
                }
            }

            if (groupStartLeft != invalidY) {
                // End left vertical group
                CreateEdgeCollider(
                    new Vector2(x, groupStartLeft) + leftStartOffset,
                    new Vector2(x, tilemapBounds.max.y) + leftEndOffset
                );
                groupStartLeft = invalidY;
            }

            if (groupStartRight != invalidY) {
                // End right vertical group
                CreateEdgeCollider(
                    new Vector2(x, groupStartRight) + rightStartOffset,
                    new Vector2(x, tilemapBounds.max.y) + rightEndOffset
                );
                groupStartRight = invalidY;
            }
        }



        // Horizontal Colliders
        pos = new Vector3Int(tilemapBounds.min.x, tilemapBounds.min.y, 0);
        for (int y = tilemapBounds.min.y; y <= tilemapBounds.max.y; y++) {
            pos.y = y;

            for (int x = tilemapBounds.min.x; x <= tilemapBounds.max.x; x++) {
                pos.x = x;
                tileBase = tilemap.GetTile(pos);
                bool useTop = false;
                bool useBot = false;

                if (tileBase != null && tileBase is VoidGeometryTile) {
                    useTop = (tileBase as VoidGeometryTile).useTopCollider;
                    if (useTop) {
                        aboveTile = tilemap.GetTile(pos + Vector3Int.up);
                        if (aboveTile != null) useTop = false;
                    }

                    useBot = (tileBase as VoidGeometryTile).useBottomCollider;
                    if (useBot) {
                        belowTile = tilemap.GetTile(pos + Vector3Int.down);
                        if (belowTile != null) useBot = false;
                    }
                }

                if (groupStartTop == invalidX && useTop) {
                    // Start above horizontal group
                    groupStartTop = x;
                } else if (groupStartTop != invalidX && !useTop) {
                    // End above horizontal group
                    CreateEdgeCollider(
                        new Vector2(groupStartTop, y) + topStartOffset,
                        new Vector2(x, y) + topEndOffset
                    );
                    groupStartTop = invalidX;
                }

                if (groupStartBot == invalidX && useBot) {
                    // Start below horizontal group
                    groupStartBot = x;
                } else if (groupStartBot != invalidX && !useBot) {
                    // End below horizontal group
                    CreateEdgeCollider(
                        new Vector2(groupStartBot, y) + botStartOffset,
                        new Vector2(x, y) + botEndOffset
                    );
                    groupStartBot = invalidX;
                }
            }


            if (groupStartTop != invalidX) {
                // End top horizontal group
                CreateEdgeCollider(
                    new Vector2(groupStartTop, y) + topStartOffset,
                    new Vector2(tilemapBounds.max.x, y) + topEndOffset
                );
                groupStartTop = invalidX;
            }

            if (groupStartBot != invalidX) {
                // End bottom horizontal group
                CreateEdgeCollider(
                    new Vector2(groupStartBot, y) + botStartOffset,
                    new Vector2(tilemapBounds.max.x, y) + botEndOffset
                );
                groupStartBot = invalidX;
            }
        }
    }

    void CreateEdgeCollider(Vector2 from, Vector2 to) {
        var edge = colliderRoot.AddComponent<EdgeCollider2D>();
        edge.points = new Vector2[] { from, to };
    }

    //void CreateBoxCollider(Vector2 from, Vector2 to) {
    //    var box = colliderRoot.AddComponent<BoxCollider2D>();
    //    Vector2 diff = to - from;

    //    box.size = new Vector2(Mathf.Abs(to.x - from.x) + boxSize.x, boxSize.y);
    //    box.offset = from + new Vector2(diff.x * 0.5f + boxSize.x * 0.5f, boxSize.y * 0.5f);

    //    Debug.DrawLine(from, to + Vector2.right * boxSize.x, Color.yellow, 60f);
    //}
}
