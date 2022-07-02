using System;
using UnityEngine;

namespace Grid
{
    public class TileCoordinates : MonoBehaviour
    {
        public Vector3 xOffset = new Vector3(0.5f, 0, 0.5f);
        public float yOffset = 0.5f;
        [SerializeField] private Vector3Int intCoordinates;
        
        internal Vector3Int GetTilePosition() => intCoordinates;

        private void Awake()
        {
            intCoordinates = ConvertPositionWithoutOffset(transform.localPosition);
        }

        private void OnEnable()
        {
            GetCoordinates();
        }
        
        public void GetCoordinates()
        {
            Debug.Log("OnEnable");
            intCoordinates = ConvertPositionWithoutOffset(transform.localPosition);
            Debug.Log("Coordinates: " + intCoordinates);
        }

        private Vector3Int ConvertPositionWithoutOffset(Vector3 transformPosition)
        {
            var withoutOffset = transformPosition - xOffset;
            var x = Mathf.RoundToInt(withoutOffset.x);
            var y = Mathf.RoundToInt(withoutOffset.y);
            var z = Mathf.RoundToInt(withoutOffset.z);
            return new Vector3Int(x, y, z);
        }
    }
}
