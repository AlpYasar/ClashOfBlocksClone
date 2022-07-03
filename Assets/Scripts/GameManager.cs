using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager Instance => instance;
    private static GameManager instance;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private GameController gameController;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int levelIndex = 0; // 0 is the first level
    [SerializeField] private int levelCount = 1;
    
    private void Awake()
    {
        //Singleton
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        
        //Set DoTween cappacity
        DOTween.SetTweensCapacity(750,500);
        //Level Index For demo
        levelManager.LevelIndex = 0;
        levelCount = levelManager.Levels.Count;
    }

    [Button]
    public void RestartLevel()
    {
        levelManager.RestartLevel();
        uiManager.SetDisActiveContinueButtonsAndTexts();
        uiManager.SetActiveRestartButton(false);
    }
    
    private void LoadNextLevel()
    {
        levelManager.NextLevel();
    }
    
    public void LoadLevel(int level)
    {
        levelManager.LoadLevel(level);
    }
    
    [Button()]
    public void NextLevel()
    {
        levelIndex++;
        LoadNextLevel();
        uiManager.SetDisActiveContinueButtonsAndTexts();
        uiManager.SetActiveRestartButton(false);
        uiManager.SetLevelText(levelIndex + 1);
    }
    
    public void OpenRestartButton()
    {
        uiManager.SetActiveRestartButton(true);
    }
}

    
