using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityAtoms.BaseAtoms;
using NaughtyAttributes;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField, BoxGroup("Texts")] private TextMeshProUGUI levelText;
    [SerializeField, BoxGroup("Texts")] private TextMeshProUGUI greenPerText;
    [SerializeField, BoxGroup("Texts")] private TextMeshProUGUI redPerText;
    [SerializeField, BoxGroup("Buttons")] private GameObject restartButton;
    [SerializeField, BoxGroup("Buttons")] private GameObject continueButton;
    [SerializeField, BoxGroup("Buttons")] private GameObject tryAgainButton;
    [SerializeField, BoxGroup("Cube Count")] private TextMeshProUGUI cubeCountText;
    [SerializeField, BoxGroup("Cube Count")] private GameObject cubeImage;
    [SerializeField, BoxGroup("Atom Variable")] private IntVariable greenCount;
    [SerializeField, BoxGroup("Atom Variable")] private IntVariable redCount;
    
    private Vector3 textsScaleSize;

    private void Awake()
    {
        textsScaleSize = greenPerText.transform.localScale;
    }

    [Button]
    public void OpenAndSetPercentages()
    {
        greenPerText.gameObject.SetActive(true);
        redPerText.gameObject.SetActive(true);
        
        greenPerText.text = "0 %";
        redPerText.text = "0 %";
        
        var total = greenCount.Value + redCount.Value;
        var greenPercent = (float) greenCount.Value / total * 100;
        var redPercent = (float) redCount.Value / total * 100;
        
        DOVirtual.Float(0, greenPercent, 1, (float value) =>
        {
            greenPerText.text = $"{value:0.#} %";
        });
        
        DOVirtual.Float(0, redPercent, 1, (float value) =>
        {
            redPerText.text = $"{value:0.#} %";
        }).OnComplete(() =>
        {
            if (greenPercent > redPercent)
            {
                OpenContinueButton();
                greenPerText.transform.DOPunchScale(Vector3.one * 0.5f, 1f, 4, 0.2f).SetDelay(1f).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                OpenTryAgainButton();
                redPerText.transform.DOPunchScale(Vector3.one * 0.5f, 1f, 4, 0.2f).SetDelay(1f).SetLoops(-1, LoopType.Yoyo);
            }
        });
    }
    
    public void SetLevelText(int level)
    {
        levelText.text = "Level " + level;
    }
    
    public void OpenRestartButton()
    {
        restartButton.SetActive(true);
    }
    
    public void SetCubeCount(int count)
    {
        DOVirtual.DelayedCall(0.75f, () =>
        {
            if (count == 0)
            {
                cubeImage.SetActive(false);
                cubeCountText.text = "";
            }
            else
            {
                cubeImage.SetActive(true);
                cubeCountText.text = count.ToString();
            }
        });
    }
    
    public void SetDisActiveContinueButtonsAndTexts()
    {
        continueButton.SetActive(false);
        tryAgainButton.SetActive(false);
        restartButton.SetActive(false);
        greenPerText.gameObject.SetActive(false);
        redPerText.gameObject.SetActive(false);
        
        
        greenPerText.transform.DOKill();
        greenPerText.transform.localScale = textsScaleSize;
        redPerText.transform.DOKill();
        redPerText.transform.localScale = textsScaleSize;
    }

    private void OpenContinueButton() => continueButton.SetActive(true);

    public void OpenTryAgainButton() => tryAgainButton.SetActive(true);

    public void SetActiveRestartButton(bool active)
    {
        restartButton.SetActive(active);
    }
}
