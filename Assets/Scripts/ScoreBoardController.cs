using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreBoardController : MonoBehaviour
{
    private int numPLayers;
    [SerializeField]
    private TextMeshProUGUI[] playersTag;

    [SerializeField]
    private TextMeshProUGUI[] playersNames;

    [SerializeField]
    private TextMeshProUGUI[] playersPoints;

    [SerializeField]
    private RectTransform borderRectTransform;

    [SerializeField]
    private RectTransform scoreboardRectTransform;

    [SerializeField]
    private GameObject restartButton;

    [SerializeField]
    private GameObject menuButton;


    [SerializeField]
    private float[] heightsName;


    public void StartScoreBoard(List<Player> players)
    {
        var sorted = players.OrderBy(x => x.GetPoints()).ToList();
        numPLayers = sorted.Count;
        for (int i = 0; i < sorted.Count; i++)
        {
            playersNames[i].text = sorted[sorted.Count - i - 1].gameObject.name;
            playersPoints[i].text = sorted[sorted.Count - i - 1].GetPoints().ToString();
        }
        StartAnimation();
    }

    private void StartAnimation()
    {
        OpenBorder();
        OpenScoreBoard();
    }

    private void OpenBorder()
    {
        borderRectTransform.LeanScaleX(12f, 0.5f);
    }

    private void OpenScoreBoard()
    {
        scoreboardRectTransform.LeanScaleY(12f, 0.5f).setOnComplete(AddTag)/*.setEaseInExpo()*/;
    }

    private void AddTag()
    {
        EnableThings();
        var sequencer = LeanTween.sequence();
        for (int i = 0; i < numPLayers - 1; i++)
        {
            sequencer.append(playersTag[i].rectTransform.LeanMoveX(-70f, 0.5f));
        }
        sequencer.append(playersTag[numPLayers - 1].rectTransform.LeanMoveX(-70f, 0.5f).setOnComplete(AddNames));
    }

    private void AddNames()
    {
        var sequencer = LeanTween.sequence();
        for (int i = numPLayers - 1; i > 0; i--)
        {
            sequencer.append(playersNames[i].rectTransform.LeanMoveY(heightsName[i], 0.5f));
        }
        sequencer.append(playersNames[0].rectTransform.LeanMoveY(heightsName[0], 0.5f).setOnComplete(AddPoints));
    }

    private void EnableThings()
    {
        foreach (var item in playersTag)
        {
            item.gameObject.SetActive(true);
        }

        foreach (var item in playersNames)
        {
            item.gameObject.SetActive(true);
        }

        foreach (var item in playersPoints)
        {
            item.gameObject.SetActive(true);
        }
        
    }

    private void AddPoints()
    {
        var sequencer = LeanTween.sequence();
        for (int i = 0; i < numPLayers - 1; i++)
        {
            sequencer.append(playersPoints[i].rectTransform.LeanMoveX(65f, 0.5f));
        }
        sequencer.append(playersPoints[numPLayers - 1].rectTransform.LeanMoveX(65f, 0.5f).setOnComplete(AddRestartAndCloseButton));
    }

    private void AddRestartAndCloseButton()
    {
        restartButton.SetActive(true);
        menuButton.SetActive(true);
    }
}
