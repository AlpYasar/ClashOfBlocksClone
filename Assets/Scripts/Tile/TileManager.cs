using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tile
{
    public class TileManager : MonoBehaviour
    {
        
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private int tileCount = 0;
        private Tilemap tilemap;

        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
