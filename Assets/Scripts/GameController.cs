using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using PoolSystem;
using Tile;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField] private ObjectsPool objectPool;
    [SerializeField] private Tile.Grid currentGrid;
    [SerializeField] private GameManager gameManager;
    [SerializeField, BoxGroup("Cubes")] private GameObject playersCube;
    [SerializeField, BoxGroup("Cubes")] private GameObject enemiesCube;
    [SerializeField, BoxGroup("Atoms Variable")] private IntVariable playerCubeCount;
    [SerializeField, BoxGroup("Atoms Variable")] private IntVariable enemyCubeCount;
    [SerializeField, BoxGroup("Tween"), ShowIf("isTweenActive")] private float tweenDuration = 1f;
    [SerializeField, ReadOnly] private List<SpreadingTile> newPlayerTileCoords = new List<SpreadingTile>();
    [SerializeField, ReadOnly] private List<SpreadingTile> newEnemyTileCoords = new List<SpreadingTile>();
    [SerializeField, BoxGroup("Demo Enemy Pos")] private List<Vector3Int> enemyPositions = new List<Vector3Int>();
    [SerializeField, BoxGroup("Events")] private VoidEvent onLevelEnd;
    [SerializeField, BoxGroup("Events")] private VoidEvent onLevelStarted;
    [SerializeField, BoxGroup("Events")] private IntEvent onNewPlayerCubeCount;
    
    private int _playerCubeCountToDrop;
    private Transform tileTransform;
    private Coroutine spreadCoroutine;
    private Coroutine mexicoCoroutine;
    private List<Sequence> _mexicoWaveSequence = new List<Sequence>();

    private const CubeType PlayerCube = CubeType.Player;
    private const CubeType EnemyCube = CubeType.Enemy;
    
    private void Start()
    {
        DOVirtual.DelayedCall(Time.deltaTime, () =>
        {
            SetEnemyCubes(enemyPositions);
        });
        
        _playerCubeCountToDrop = 2;
        onNewPlayerCubeCount.Raise(_playerCubeCountToDrop);
    }

    public void SetNewLevel(int playerCubeToDropCount, List<Vector3Int> enemyCoords)
    {
        _playerCubeCountToDrop = playerCubeToDropCount;
        playerCubeCount.Value = 0;
        enemyCubeCount.Value = 0;
        DOVirtual.DelayedCall(Time.deltaTime, () =>
        {
            SetEnemyCubes(enemyCoords);
        });
        
        newPlayerTileCoords.Clear();
        newEnemyTileCoords.Clear();
        onNewPlayerCubeCount.Raise(_playerCubeCountToDrop);
        spreadCoroutine = null;
    }


    public void SetEnemyCubes(List<Vector3Int> coords)
    {
        foreach (var coord in coords)
        {
            RectTile tile = currentGrid.GetRectTile(coord);
            Debug.Log("coords + " +  coord + "has tile and " + tile.TileCoords);
            var cube = objectPool.GetFromPool(enemiesCube);
            enemyCubeCount.Value++;
            tile.SetCube(cube);
            Debug.Log("Enemy cube placed at " + coord);
            SetNewTiles(tile, false);
        }
    }

    public void SelectedCoords(Vector3 coords)
    {
        //Convert the vector3 to vector3int
        Vector3Int tileCoords = new Vector3Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y), Mathf.RoundToInt(coords.z));
        //CubeDrop(tileCoords);
    }
    
    public void CubeDrop(Vector3Int position, bool isFirst = true)
    {
        
        if (_playerCubeCountToDrop <= 0) return;
        
        RectTile tile = currentGrid.GetRectTile(position);

        if (tile == null || tile.isOccupied) return;
        var playerCube = objectPool.GetFromPool(playersCube);
        tile.SetCube(playerCube, null , tweenDuration, isFirst);

        playerCubeCount.Value++;
        _playerCubeCountToDrop--;
        onNewPlayerCubeCount.Raise(_playerCubeCountToDrop);
        gameManager.OpenRestartButton();
            
        SetNewTiles(tile, true);

        if (_playerCubeCountToDrop == 0 &&isFirst && spreadCoroutine == null)
        {
            onLevelStarted.Raise();
            spreadCoroutine = StartCoroutine(SpreadCubes());
        }
    }

    private void MakeCube(SpreadingTile spreadingTile, CubeType cubeType)
    {
        RectTile tile = currentGrid.GetRectTile(spreadingTile.coords);
        bool isPlayerCube = cubeType == PlayerCube;
        if (tile != null && !tile.isOccupied)
        {
            var cube = objectPool.GetFromPool( isPlayerCube ? playersCube : enemiesCube);
            tile.SetCube(cube, spreadingTile.cube, tweenDuration, false, spreadingTile.direction);

            if (isPlayerCube)
                playerCubeCount.Value++;
            else
                enemyCubeCount.Value++;
            
            SetNewTiles(tile, isPlayerCube);
        }
    }

    private void SetNewTiles(RectTile currentTile, bool isPlayerCube)
    {
        List<SpreadingTile> newTiles = isPlayerCube ? newPlayerTileCoords : newEnemyTileCoords;
        foreach (var coords in currentGrid.GetNeighborTiles(currentTile.TileCoords))
        {
            if (!newTiles.Exists(x => x.coords == coords) && !currentGrid.GetRectTile(coords).isOccupied)
            {
                SpreadDirection direction = GetSpreadDirection(coords, currentTile.TileCoords);
                var spreadingTile = new SpreadingTile(coords, currentTile.occupantCube.transform, direction);
                newTiles.Add(spreadingTile);
            }
        }
    }

    private IEnumerator SpreadCubes()
    {
        yield return new WaitForSeconds(tweenDuration);
        
        while (newPlayerTileCoords.Count > 0 || newEnemyTileCoords.Count > 0)
        {
            var countForIteration = newPlayerTileCoords.Count;
            for (var index = countForIteration - 1; index >= 0; index--)
            {
                var newTile = newPlayerTileCoords[0];
                newPlayerTileCoords.RemoveAt(0);
                
                MakeCube(newTile, CubeType.Player);
            }
            
            countForIteration = newEnemyTileCoords.Count;
            for (var index = countForIteration - 1; index >= 0; index--)
            {
                var newTile = newEnemyTileCoords[0];
                newEnemyTileCoords.RemoveAt(0);
                
                MakeCube(newTile, CubeType.Enemy);
            }

            yield return new WaitForSeconds(tweenDuration);
        }

        if (spreadCoroutine != null)
        {
            StopCoroutine(spreadCoroutine);
            spreadCoroutine = null;
        }
        
        LevelEnded();
    }

    public void StopCoroutines()
    {
        if (spreadCoroutine != null)
        {
            StopCoroutine(spreadCoroutine);
            spreadCoroutine = null;
        }
        
        if (mexicoCoroutine != null)
        {
            StopCoroutine(mexicoCoroutine);
            mexicoCoroutine = null;
        }
    }
    private void LevelEnded()
    {
        mexicoCoroutine = StartCoroutine(DoMexicoWave());
        onLevelEnd.Raise();
    }

    private SpreadDirection GetSpreadDirection(Vector3Int newCoords, Vector3Int oldCoords)
    {
        SpreadDirection direction;
        
        if (newCoords - oldCoords == new Vector3Int(0, 0, 1))
        {
            direction = SpreadDirection.North;
        }else if (newCoords - oldCoords == new Vector3Int(0, 0, -1))
        {
            direction = SpreadDirection.South;
        }else if (newCoords - oldCoords == Vector3Int.right)
        {
            direction = SpreadDirection.West;
        }else if (newCoords - oldCoords == Vector3Int.left)
        {
            direction = SpreadDirection.East;
        }else
        {
            direction = SpreadDirection.None;
        }
        
        return direction;
    }
    
    [Button]
    private void StopSpreadCoroutine()
    {
        if (spreadCoroutine != null)
        {
            StopCoroutine(spreadCoroutine);
        }
    }

    private IEnumerator DoMexicoWave()
    {
        yield return new WaitForSeconds(1f);
        var waitTime = new WaitForSeconds(Time.fixedDeltaTime*2);
        foreach (var element in currentGrid.TileDict)
        {
            var tween = element.Value.OccupantTransform.DOLocalJump(Vector3.zero, 0.5f, 1, Random.Range(2f, 1.5f))
                .SetRelative(true).SetDelay(0.5f).SetLoops(-1, LoopType.Yoyo);
            _mexicoWaveSequence.Add(tween);
            yield return waitTime;
        }
    }
    
    public void StopMexicoWave()
    {
        if (mexicoCoroutine != null)
        {
            StopCoroutine(mexicoCoroutine);
        }
        
        foreach (var tween in _mexicoWaveSequence)
        {
            tween.Kill();
        }
        _mexicoWaveSequence.Clear();
    }

    [System.Serializable]
    private class SpreadingTile
    {
        public Vector3Int coords;
        public Transform cube;
        public SpreadDirection direction;
        
        public SpreadingTile(Vector3Int coords, Transform cube, SpreadDirection direction = SpreadDirection.None)
        {
            this.coords = coords;
            this.cube = cube;
            this.direction = direction;
        }
    }
}

public enum SpreadDirection
{
    North,
    South,
    West,
    East,
    None
}

public enum CubeType
{
    Player,
    Enemy
}
