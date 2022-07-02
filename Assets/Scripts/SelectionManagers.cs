using System;
using System.Collections.Generic;
using Tile;
using UnityEngine;
using DG.Tweening;
using UnityAtoms.BaseAtoms;

namespace DefaultNamespace
{
    public class SelectionManagers : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameController gameController;
        public LayerMask selectionLayerMask;
        [SerializeField] private Tile.Grid grid;
        [SerializeField] private Vector3Event tileCoordinateEvent;
        

        private bool showGizmoRay;
        private Vector3 inputPosition;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }
        
        public void HandleSelection(Vector3 mousePosition)
        {
            if (FindTarget(mousePosition, out var result))
            {
                Debug.Log("Selected " + result.name);
            }
        }
        
        public void HandleClick(Vector3 mousePosition)
        {
            Debug.Log("Clicked");
            Debug.Log(mousePosition);
            if (FindTarget(mousePosition, out var result))
            {
                Debug.Log("Tile Found ");
                RectTile selectedTile = result.GetComponent<RectTile>();
                if (selectedTile != null)
                {
                    Debug.Log("Tile is RectTile");
                    gameController.CubeDrop(selectedTile.TileCoords);
                }
                List<Vector3Int> neighbors = grid.GetNeighborTiles(selectedTile.TileCoords);
                foreach (var neighbor in neighbors)
                {
                    Debug.Log(neighbor);
                }
            }
        }
        
        private bool FindTarget(Vector3 mousePosition, out GameObject result)
        {
            var transformCam = mainCamera.transform;
            Ray ray = new Ray( transformCam.position,mousePosition - transformCam.position);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 3);
            inputPosition = mousePosition;
            showGizmoRay = true;
            DOVirtual.DelayedCall(2f, () => showGizmoRay = false);
            
            
            if (Physics.Raycast(ray, out var hit, 300, selectionLayerMask))
            {
                result = hit.collider.gameObject;
                return true;
            }
            
            result = null;
            return false;
        }
        
        private void OnDrawGizmos()
        {
            if (showGizmoRay)
            {
                //Gizmos.color = Color.red;
                //Gizmos.DrawRay(mainCamera.transform.position, (inputPosition - mainCamera.transform.position) * 300);
            }
        }
    }
}