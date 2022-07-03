using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tile
{
   
    public class Grid : MonoBehaviour
    {
        [SerializeField] private GameObject currentTileMapGameObject;
        Dictionary<Vector3Int, RectTile> rectTileDict = new Dictionary<Vector3Int, RectTile>();
        Dictionary<Vector3Int, List<Vector3Int>> tileNeighboursDict = new Dictionary<Vector3Int, List<Vector3Int>>();
        public Dictionary<Vector3Int, RectTile> TileDict => rectTileDict;
        
        private void Start()
        {
            SetDictionary();
        }

        public void NewLevel(GameObject newTileMapGameObject)
        {
            currentTileMapGameObject = newTileMapGameObject;
            SetDictionary();
        }
        
        private void SetDictionary()
        {
            rectTileDict.Clear();
            tileNeighboursDict.Clear();

            var tileCount = 0;
            foreach (var rectTile in currentTileMapGameObject.transform.GetComponentsInChildren<RectTile>())
            {
                tileCount++;
                
                rectTile.TileCoordinates.GetCoordinates();
                rectTileDict.Add(rectTile.TileCoords, rectTile);
                //tileNeighboursDict.Add(rectTile.TileCoords, GetNeighborTiles(rectTile.TileCoords));
            }
        }
        
        public RectTile GetRectTile(Vector3Int tileCoords)
        {
            rectTileDict.TryGetValue(tileCoords, out var rectTile);
            return rectTile;
        }
        
        public List<Vector3Int> GetNeighborTiles(Vector3Int tileCoords)
        {
            if (!rectTileDict.ContainsKey(tileCoords))
                return new List<Vector3Int>();
            
            if (tileNeighboursDict.ContainsKey(tileCoords))
                return tileNeighboursDict[tileCoords];
            
            tileNeighboursDict.Add(tileCoords, new List<Vector3Int>());

            foreach (var direction in Direction.GetDirections)
            {
                Debug.Log("Looking neighbour for " + tileCoords + " in  " + direction);
                if (rectTileDict.ContainsKey(tileCoords + direction))
                    tileNeighboursDict[tileCoords].Add(tileCoords + direction);
            }
            
            return tileNeighboursDict[tileCoords];
        }
    }

    public static class Direction
    {
        private static readonly List<Vector3Int> Directions = new List<Vector3Int>
        {
            new Vector3Int(0,0, 1), //North
            new Vector3Int(0,0, -1), //South
            new Vector3Int(1,0, 0), //East
            new Vector3Int(-1,0, 0), //West
        };

        public static IEnumerable<Vector3Int> GetDirections => Directions;
    }
    

}