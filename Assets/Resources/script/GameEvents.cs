using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GameEvents", menuName = "WhatIsItMeans/new GameEvents")]


public class GameEvents : ScriptableObject
{
    public delegate void UpdateQuestionUICallback(Question question);
    public UpdateQuestionUICallback UpdateQuestionUI;

    public delegate void UpdateQuestionAnswerCallBack(AnswerData pickedAnswer);
    public UpdateQuestionAnswerCallBack updateQuestionAnswer;

    public delegate void DisplayResolutionScreenCallBack(UIManager.ResolutionScreenType type, int score);
    public DisplayResolutionScreenCallBack DisplayResolutionScreen;

    public delegate void ScoreUpdatedCallback();
    public ScoreUpdatedCallback ScoreUpdated;

    public delegate void PauseGameCallBack(GameObject Overlay, Button PauseButton);
    public PauseGameCallBack PauseGame;

    public int CurrentFinalScore = 0;

    public int StartupHighscore = 0;

    public int currentLevelMaxScore = 0;
    public int isLevel = 0;
    public bool isPause = false;


}
