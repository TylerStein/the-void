using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class VoidGeometryCollisionGenerator : MonoBehaviour
{
    public Vector2 startOffset = new Vector2(-0.5f, 0.5f);
    public Vector2 endOffset = new Vector2(0.5f, 0.5f);

    private Tilemap tilemap;
    private GameObject colliderRoot;
  
    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();

        colliderRoot = new GameObject("VoidGeometryColliders");
        colliderRoot.transform.position = Vector3.zero;

        Vector3Int tilemapSize = tilemap.size;
        Vector3Int tilemapOrigin = tilemap.origin;

        TileBase tileBase;
        TileBase aboveTile;

        BoundsInt tilemapBounds = new BoundsInt();
        tilemapBounds.size = tilemapSize;
        tilemapBounds.position = tilemapOrigin;

        int invalidX = tilemapBounds.min.x - 1;
        int groupStartX = invalidX;

        for (int y = tilemapBounds.min.y; y <= tilemapBounds.max.y; y++) {
            for (int x = tilemapBounds.min.x; x <= tilemapBounds.max.x; x++) {
                Vector3Int pos = new Vector3Int(x, y, 0);
                tileBase = tilemap.GetTile(pos);
                aboveTile = tilemap.GetTile(pos + Vector3Int.up);
                bool useEdgeCollider = aboveTile == null;

                VoidGeometryTile voidTile = tileBase is VoidGeometryTile ? tileBase as VoidGeometryTile : null;
                if (groupStartX == invalidX && voidTile != null && useEdgeCollider == true) {
                    // start of valid collision row
                    groupStartX = x;
                } else if (groupStartX >= tilemapBounds.min.x && (voidTile == null || useEdgeCollider == false)) {
                    // end of valid collision row
                    CreateEdgeCollider(new Vector2(groupStartX, y), new Vector2(x - 1, y));
                    groupStartX = invalidX;
                }
            }

            if (groupStartX >= tilemapBounds.min.x) {
                CreateEdgeCollider(new Vector2(groupStartX, y), new Vector2(tilemapBounds.max.x, y));
                groupStartX = invalidX;
            }
        }
    }

    void CreateEdgeCollider(Vector2 from, Vector2 to) {
        var edge = colliderRoot.AddComponent<EdgeCollider2D>();
        edge.points = new Vector2[] {
            from + startOffset,
            to + endOffset,
        };
    }
}
