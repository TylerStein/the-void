using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New VoidGeometry Tile", menuName = "Tilemap/Void Geometry Tile", order = 1)]
public class VoidGeometryTile : RuleTile<VoidGeometryTile.Neighbor> {
    //public bool useEdgeCollider = false;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
    }

    /// <summary>
    /// StartUp is called on the first frame of the running Scene.
    /// </summary>
    /// <param name="location">Position of the Tile on the Tilemap.</param>
    /// <param name="tilemap">The Tilemap the tile is present on.</param>
    /// <param name="instantiatedGameObject">The GameObject instantiated for the Tile.</param>
    /// <returns>Whether StartUp was successful</returns>
    //public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject instantiatedGameObject) {
    //    TileBase tile = tilemap.GetTile(location + Vector3Int.up);
    //    if (tile != null) {
    //        useEdgeCollider = false;
    //    } else {
    //        useEdgeCollider = true;
    //    }

    //    return base.StartUp(location, tilemap, instantiatedGameObject);
    //}
}
