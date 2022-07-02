﻿using System.Collections;
using System.Collections.Generic;
using Scriptable;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using UnityAtoms.BaseAtoms;

public class LevelManager : MonoBehaviour
{
    [SerializeField, BoxGroup("Levels"), HorizontalLine(color: EColor.Blue)] private Levels levels;
    [SerializeField, BoxGroup("Level")] private Level currentLevel;
    [SerializeField, BoxGroup("Level")] private GameObject currentLevelObject;
    [SerializeField, BoxGroup("Level")] private GameObject oldLevelObject;
    [SerializeField, BoxGroup("Level")] private int levelIndex; //0 is the first level
    [SerializeField, BoxGroup("Atoms")] private BoolVariable clickIsPermitted; //Wait tween to finish before allowing click
    [SerializeField, BoxGroup("Tween")] private float tweenDuration;
    [SerializeField] private GameController gameController; //0 is the first level
    [SerializeField] private Tile.Grid grid;
    

    [Button]
    public void GetNextLevel()
    {
        levelIndex++;
        oldLevelObject = currentLevelObject;
        currentLevel = levels.GetLevel(levelIndex);
        currentLevelObject = Instantiate(currentLevel.levelEnvironment, new Vector3(0, 0, 45), Quaternion.identity);
        currentLevelObject.SetActive(true);
        clickIsPermitted.Value = false;
        DOVirtual.DelayedCall(Time.deltaTime*2, () =>
        {
            //Tween the old level out and the new level in
            oldLevelObject.transform.DOMoveZ(-45, tweenDuration).SetRelative(true).OnComplete(() =>
            {
                gameController.StopMexicoWave();
                oldLevelObject.SetActive(false);
                clickIsPermitted.Value = true;
            });
            currentLevelObject.transform.DOMove(Vector3.zero, tweenDuration);
        
            grid.NewLevel(currentLevel.tileMapGameObject);
            gameController.SetNewLevel(currentLevel.playerCubeCount, currentLevel.hostileCubePositions);
        });
        

    }
}
