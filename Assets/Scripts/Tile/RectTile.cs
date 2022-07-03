using System;
using DG.Tweening;
using Grid;
using NaughtyAttributes;
using UnityEngine;

namespace Tile
{
    public class RectTile : MonoBehaviour
    {
        [SerializeField] private TileCoordinates _tileCoordinates;
        [ShowNativeProperty] public Vector3Int TileCoords => _tileCoordinates.GetTilePosition();
        public bool isOccupied;
        [ReadOnly] public GameObject occupantCube;
        public Transform OccupantTransform => occupantCube.transform;
        public TileCoordinates TileCoordinates => _tileCoordinates;
        
        [SerializeField] private Transform t;
        private static float _heightGap = 0.5f;

        private void OnEnable()
        {
            t = transform;
            isOccupied = false;
        }

        public void SetCube(GameObject newCube)
        {
            newCube.transform.DOKill();
            occupantCube = newCube;
            newCube.transform.SetParent(t);
            newCube.transform.position = t.position + Vector3.up * 0.5f;
            newCube.transform.rotation = t.rotation;
            newCube.SetActive(true);
            isOccupied = true;
        }
        
        public void SetCube(GameObject newCube, Transform ancestorCube, float tweenDuration, bool isFirstCube = false, SpreadDirection spreadDirection = SpreadDirection.None)
        {
            occupantCube = newCube;
            newCube.transform.SetParent(t);
            newCube.transform.rotation = RotationAngle(spreadDirection);
            
            newCube.transform.position = isFirstCube ? t.transform.position + Vector3.up * _heightGap * 4 : ancestorCube.position /*+ Vector3.up * 0.75f*/;
            
            //newCube.transform.rotation = isFirstCube ? _transform.transform.rotation : ancestorCube.rotation;
            newCube.SetActive(true);
            isOccupied = true;

            if (!isFirstCube)
            {
                newCube.transform.DOLocalJump(Vector3.up * _heightGap, 1f, 1, tweenDuration);
                newCube.transform.DOLocalRotate(EndRotationAngle(spreadDirection), tweenDuration , RotateMode.FastBeyond360);
            }else
            {
                newCube.transform.rotation = Quaternion.identity;
                newCube.transform.DOLocalMove(Vector3.up * _heightGap, tweenDuration);
            }
            /*
            var currentScale = newCube.transform.localScale;
            newCube.transform.localScale = Vector3.zero;
            newCube.transform.DOScale(currentScale, tweenDuration);
            */
        }

        public void SetCubeFree()
        {
            occupantCube.transform.SetParent(null);
            occupantCube.SetActive(false);
            occupantCube = null;
        }

        private static Quaternion RotationAngle(SpreadDirection spreadDirection)
        {
            Quaternion euler;
            
            switch (spreadDirection)
            {
                case SpreadDirection.North:
                    euler = Quaternion.Euler(90, 0, 0);
                    break;
                case SpreadDirection.South:
                    euler = Quaternion.Euler(-90, 0, 0);
                    break;
                case SpreadDirection.West:
                    euler = Quaternion.Euler(90, 90, 0);
                    break;
                case SpreadDirection.East:
                    euler = Quaternion.Euler(90, -90, 0);
                    break;
                case SpreadDirection.None:
                    euler = Quaternion.Euler(90, -90, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(spreadDirection), spreadDirection, null);
            }
            
            return euler;
        }
        
        private static Vector3 EndRotationAngle(SpreadDirection spreadDirection)
        {
            Vector3 euler;
            
            switch (spreadDirection)
            {
                case SpreadDirection.North:
                    euler = new Vector3(360, 0, 0);
                    break;
                case SpreadDirection.South:
                    euler = new Vector3(0, 0, 0);
                    break;
                case SpreadDirection.West:
                    euler = new Vector3(360, 90, 0);
                    break;
                case SpreadDirection.East:
                    euler = new Vector3(0, -90, 0);
                    break;
                case SpreadDirection.None:
                    euler = new Vector3(0, 0, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(spreadDirection), spreadDirection, null);
            }
            
            return euler;
        }
    }
}