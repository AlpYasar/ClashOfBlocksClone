using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "Levels", menuName = "Levels", order = 0)]
    public class Levels : ScriptableObject
    {
        [SerializeField] private List<Level> levels = new List<Level>();
        public int Count => levels.Count;

        private void Awake()
        {
            SetTileMapObjectToList();
        }
        
        [Button]
        private void SetTileMapObjectToList()
        {
            foreach (var level in levels)
            {
                level.tileMapGameObject = level.levelEnvironment.GetComponent<LevelParent>().tileMap;
            }
        }

        public Level GetLevel(int index)
        {
            return levels[index];
        }
    }
    
    [Serializable]
    public class Level
    {
        [ShowAssetPreview(128, 128)] public GameObject levelEnvironment;
        public GameObject tileMapGameObject;
        public List<Vector3Int> hostileCubePositions;
        public int playerCubeCount;
    }
}