using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable()]
public struct UIManagerParameters
{
    [Header("Answers Options")]
    [SerializeField] float margins;
    public float Margins { get { return margins; } }

    [Header("Resolution Screen Options")]
    [SerializeField] Color correctBGColor;
    public Color CorrectBGColor { get { return correctBGColor; } }
    [SerializeField] Color incorrectBGColor;
    public Color IncorrectBGColor { get { return incorrectBGColor; } }
    [SerializeField] Color finalBGColor;
    public Color FinalBGColor { get { return finalBGColor; } }
}
[Serializable()]
public struct UIElements
{
    [SerializeField] RectTransform answersContentArea;
    public RectTransform AnswersContentArea { get { return answersContentArea; } }

    [SerializeField] Text questionInfoTextObject;
    public Text QuestionInfoTextObject { get { return questionInfoTextObject; } }


    [SerializeField] Animator scoreTextObject;
    public Animator ScoreTextObject { get { return scoreTextObject; } }
    [SerializeField] Text scoreText;
    public Text ScoreText { get { return scoreText; } }

    [Space]

    [SerializeField] Animator resolutionScreenAnimator;
    public Animator ResolutionScreenAnimator { get { return resolutionScreenAnimator; } }

    [SerializeField] Image resolutionBG;
    public Image ResolutionBG { get { return resolutionBG; } }

    [SerializeField] Text resolutionStateInfoText;
    public Text ResolutionStateInfoText { get { return resolutionStateInfoText; } }

    [SerializeField] Text resolutionScoreText;
    public Text ResolutionScoreText { get { return resolutionScoreText; } }

    [Space]

    [SerializeField] Text highScoreText;

    public Text HighScoreText { get { return highScoreText; } }

    [SerializeField] CanvasGroup mainCanvasGroup;
    public CanvasGroup MainCanvasGroup { get { return mainCanvasGroup; } }
    [Space]
    [SerializeField] Image finishBg;
    public Image FinishBg { get { return finishBg; } }
    [SerializeField] RectTransform finishUIElements;
    public RectTransform FinishUIElements { get { return finishUIElements; } }

    [SerializeField] Text finishLevelTitle;
    public Text FinishLevelTitle { get { return finishLevelTitle; } }
    [SerializeField] Image[] finishStarGroup;
    public Image[] FinishStarGroup { get { return finishStarGroup; } }
    [SerializeField] Sprite finishStarActive;
    public Sprite FinishStarActive { get { return finishStarActive; } }


    [SerializeField] GameObject overlay;
    public GameObject Overlay { get { return overlay; } }

    [SerializeField] Button pauseButton;
    public Button PauseButton { get { return pauseButton; } }


}
public class UIManager : MonoBehaviour
{
    public enum ResolutionScreenType { Correct, Incorrect, Finish }

    [Header("Refernces")]
    [SerializeField] GameEvents events;


    [Header("UI Elements (Prefacts)")]
    [SerializeField] AnswerData answerPrefab;
    [SerializeField] UIElements uIElements = new UIElements();
    [Space]
    [SerializeField] UIManagerParameters parameters = new UIManagerParameters();
    [SerializeField] private AudioClip CorrectSound;
    [SerializeField] private AudioClip InCorrectSound;
    GameManager gameManager;
    List<AnswerData> currentAnswers = new List<AnswerData>();
    private int resStateParaHash = 0;

    private IEnumerator IE_DisplayTimedResolution = null;

    void OnEnable()
    {
        events.UpdateQuestionUI += UpdateQuestionUI;
        events.DisplayResolutionScreen += DisplayResolution;
        events.ScoreUpdated += UpdateScoreUI;
    }

    void OnDisable()
    {
        events.UpdateQuestionUI -= UpdateQuestionUI;
        events.DisplayResolutionScreen -= DisplayResolution;
        events.ScoreUpdated -= UpdateScoreUI;
    }
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    void Start()
    {
        UpdateScoreUI();
        resStateParaHash = Animator.StringToHash("ScreenState");
    }

    void DisplayResolution(ResolutionScreenType type, int score)
    {
        UpdateResUI(type, score);
        uIElements.ResolutionScreenAnimator.SetInteger(resStateParaHash, 2);
        uIElements.MainCanvasGroup.blocksRaycasts = false;

        if (type != ResolutionScreenType.Finish)
        {
            if (IE_DisplayTimedResolution != null)
            {
                StopCoroutine(IE_DisplayTimedResolution);
            }
            IE_DisplayTimedResolution = DisplayTimedResolution();
            StartCoroutine(IE_DisplayTimedResolution);
        }
    }
    IEnumerator DisplayTimedResolution()
    {
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        uIElements.ResolutionScreenAnimator.SetInteger(resStateParaHash, 1);
        uIElements.MainCanvasGroup.blocksRaycasts = true;
    }

    void UpdateResUI(ResolutionScreenType type, int score)
    {
        var highscore = PlayerPrefs.GetInt(events.SelectedQuestionType + "_" + GameUtility.SavePrefKey + events.isLevel.ToString());

        switch (type)
        {
            case ResolutionScreenType.Correct:
                UiSoundPlayer.Instance.PlaySFX(CorrectSound);
                uIElements.ResolutionBG.color = parameters.CorrectBGColor;
                uIElements.ResolutionStateInfoText.text = "Correct!";
                uIElements.ResolutionScoreText.text = "+" + score;
                break;
            case ResolutionScreenType.Incorrect:
                UiSoundPlayer.Instance.PlaySFX(InCorrectSound);
                uIElements.ResolutionBG.color = parameters.IncorrectBGColor;
                uIElements.ResolutionStateInfoText.text = "Wrong!";
                uIElements.ResolutionScoreText.text = "-" + score;
                break;
            case ResolutionScreenType.Finish:
                uIElements.ResolutionBG.color = parameters.FinalBGColor;
                uIElements.ResolutionStateInfoText.text = "Final Score";
                uIElements.ResolutionScoreText.text = highscore.ToString();
                StartCoroutine(CalculateScore());
                uIElements.FinishBg.gameObject.SetActive(true);
                uIElements.FinishUIElements.gameObject.SetActive(true);
                uIElements.HighScoreText.gameObject.SetActive(true);

                uIElements.HighScoreText.text = ((highscore > events.StartupHighscore) ? "<color=yellow>New </color>" + "Best: " + highscore : "Best: " + highscore);
                if (highscore > events.StartupHighscore)
                {
                    PlayerPrefs.SetInt(events.SelectedQuestionType + "_" + GameUtility.SavePrefKey + events.isLevel.ToString(), highscore);
                }

                var scorePreStar = events.currentLevelMaxScore / 3;
                var isLevelAllStartCompleted = PlayerPrefs.GetInt(events.SelectedQuestionType + "_" + GameUtility.SavePrefKey + events.isLevel.ToString() + "_State");
                var currentResult = 0;
                if (events.CurrentFinalScore == events.currentLevelMaxScore)
                {
                    currentResult = 3;
                }
                else
                {
                    if (events.CurrentFinalScore <= scorePreStar * 2 && events.CurrentFinalScore > scorePreStar)
                    {
                        currentResult = 2;
                    }
                    else
                    {
                        if (events.CurrentFinalScore <= scorePreStar)
                        {
                            currentResult = 1;
                        }
                    }
                }
                for (int i = 0; i < currentResult; i++)
                {
                    uIElements.FinishStarGroup[i].sprite = uIElements.FinishStarActive;
                }

                if (isLevelAllStartCompleted != 3)
                {
                    if (highscore == events.currentLevelMaxScore)
                    {
                        PlayerPrefs.SetInt(events.SelectedQuestionType + "_" + GameUtility.SavePrefKey + events.isLevel.ToString() + "_State", 3);


                        Debug.Log(uIElements.FinishStarGroup.Length);

                        if (events.isLevel >= PlayerPrefs.GetInt(events.SelectedQuestionType + "_" + "LevelState"))
                        {
                            PlayerPrefs.SetInt(events.SelectedQuestionType + "_" + "LevelState", events.isLevel + 1);
                        }
                    }
                    else
                    {
                        if (highscore <= scorePreStar * 2 && highscore > scorePreStar)
                        {
                            PlayerPrefs.SetInt(events.SelectedQuestionType + "_" + GameUtility.SavePrefKey + events.isLevel.ToString() + "_State", 2);
                            for (int i = 0; i < (uIElements.FinishStarGroup.Length - 1); i++)
                            {
                                uIElements.FinishStarGroup[i].sprite = uIElements.FinishStarActive;

                            }
                            if (events.isLevel >= PlayerPrefs.GetInt(events.SelectedQuestionType + "_" + "LevelState"))
                            {
                                PlayerPrefs.SetInt(events.SelectedQuestionType + "_" + "LevelState", events.isLevel + 1);
                            }
                        }
                        else
                        {
                            if (highscore <= scorePreStar && isLevelAllStartCompleted != 2)
                            {
                                PlayerPrefs.SetInt(events.SelectedQuestionType + "_" + GameUtility.SavePrefKey + events.isLevel.ToString() + "_State", 1);
                                for (int i = 0; i < (uIElements.FinishStarGroup.Length - 2); i++)
                                {
                                    uIElements.FinishStarGroup[i].sprite = uIElements.FinishStarActive;

                                }
                            }

                        }
                    }
                }



                uIElements.FinishLevelTitle.text = "Level " + events.isLevel.ToString();
                events.StartupHighscore = PlayerPrefs.GetInt(events.SelectedQuestionType + "_" + GameUtility.SavePrefKey + events.isLevel.ToString());
                gameManager.FinishedQuestions = new List<int>();
                //events.updateQuestionAnswer = null;
                break;
        }
    }


    void UpdateQuestionUI(Question question)
    {
        uIElements.QuestionInfoTextObject.text = question.Info;
        CreateAnswer(question);
    }


    IEnumerator CalculateScore()
    {
        var scoreValue = 0;
        if (events.CurrentFinalScore < 0)
        {

            while (scoreValue > events.CurrentFinalScore)
            {
                scoreValue--;
                uIElements.ResolutionScoreText.text = scoreValue.ToString();

                yield return null;
            }

        }
        else
        {
            if (events.CurrentFinalScore == 0)
            {
                uIElements.ResolutionScoreText.text = scoreValue.ToString();

            }
            else
            {
                while (scoreValue < events.CurrentFinalScore)
                {
                    scoreValue++;
                    uIElements.ResolutionScoreText.text = scoreValue.ToString();

                    yield return null;
                }
            }
        }

    }

    void CreateAnswer(Question question)
    {
        EraseAnswers();
        float offset = 0 - parameters.Margins;
        for (int i = 0; i < question.Answers.Length; i++)
        {
            AnswerData newAnswer = (AnswerData)Instantiate(answerPrefab, uIElements.AnswersContentArea);
            newAnswer.UpdateData(question.Answers[i].Info, i);
            newAnswer.Rect.anchoredPosition = new Vector2(0, offset);

            offset -= (newAnswer.Rect.sizeDelta.y + parameters.Margins);
            uIElements.AnswersContentArea.sizeDelta = new Vector2(uIElements.AnswersContentArea.sizeDelta.x, offset * -1);
            currentAnswers.Add(newAnswer);
        }
    }

    void EraseAnswers()
    {
        foreach (var answer in currentAnswers)
        {
            Destroy(answer.gameObject);
        }
        currentAnswers.Clear();
    }

    void UpdateScoreUI()
    {
        uIElements.ScoreTextObject.Play("ScoreScaleAnimation");
        uIElements.ScoreText.text = events.CurrentFinalScore.ToString();
    }

    public void PauseGame()
    {
        Debug.Log("[PauseGame]");
        events.PauseGame(uIElements.Overlay, uIElements.PauseButton);

    }


}
