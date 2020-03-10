using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{

    [SerializeField] public int coins;
    [SerializeField] private bool clearAllData;
    [SerializeField] public bool isPlaying;
    [SerializeField] public bool isPause;
    [SerializeField] public int isLevel = 0;
    [SerializeField] public int LevelState;
    [Space]
    [SerializeField] public int TotaleLevels;

    Question[] _questions = null;
    public Question[] Questions { get { return _questions; } }
    [SerializeField] GameEvents events = null;

    [SerializeField] Animator timerAnimtor = null;
    [SerializeField] Text timerText = null;
    [SerializeField] Color timerHalfWayOutColor = Color.yellow;
    [SerializeField] Color timerAlmostOutColor = Color.red;
    private Color timerDefaultColor = Color.black;

    private List<AnswerData> PickedAnswers = new List<AnswerData>();
    public List<int> FinishedQuestions = new List<int>();
    private int timerStateParaHash = 0;
    private int currentQuestion = 0;

    private IEnumerator IE_waitTillNextRound = null;
    private IEnumerator IE_StartTimer = null;

    private bool IsFinished
    {
        get
        {
            return (FinishedQuestions.Count < Questions.Length) ? false : true;
        }
    }

    public void EraseAnswers()
    {
        PickedAnswers = new List<AnswerData>();
    }

    public static GameManager instance = null;
    // Start is called before the first frame update
    void OnEnable()
    {
        events.updateQuestionAnswer += UpdateAnswers;

    }

    void OnDisable()
    {
        events.updateQuestionAnswer -= UpdateAnswers;
    }
    void Display()
    {
        EraseAnswers();
        var question = GetRandomQuestion();

        if (events.UpdateQuestionUI != null)
        {
            events.UpdateQuestionUI(question);
        }
        else
        {
            Debug.LogWarning("Wrong while trying to display new Question UI Data.");
        }
        if (question.UseTimer)
        {
            UpdateTimer(question.UseTimer);
        }
    }

    public void Accept()
    {
        UpdateTimer(false);

        Debug.Log("[Start Accept]");
        Debug.Log("[current question] " + Questions[currentQuestion].Info);
        foreach (var answer in Questions[currentQuestion].GetCorrectAnswers())
        {
            Debug.Log(" [Question currect Answer`s]" + Questions[currentQuestion].Answers[answer].Info);
        }


        // List<int> p = PickedAnswers.Select(x => x.AnswerIndex).ToList();
        foreach (var PickedAnswer in PickedAnswers)
        {
            Debug.Log("[Question PickedAnswer] " + PickedAnswer);
        }

        bool isCorrect = CheckAnswers();
        Debug.Log("[Question CheckAnswers] " + isCorrect);
        // Debug.Log(" [Answers]" + string.Join("",
        //  new List<int>(Questions[currentQuestion].GetCorrectAnswers())
        //  .ConvertAll(i => i.ToString())
        //  .ToArray())+" "+Questions[currentQuestion].Answers[]);

        FinishedQuestions.Add(currentQuestion);
        UpdateScore((isCorrect) ? Questions[currentQuestion].AddScore : -Questions[currentQuestion].AddScore);

        if (IsFinished)
        {
            SetHighscore();

        }
        var type
                  = (IsFinished)
                  ? UIManager.ResolutionScreenType.Finish
                  : (isCorrect) ? UIManager.ResolutionScreenType.Correct
                  : UIManager.ResolutionScreenType.Incorrect;


        if (events.DisplayResolutionScreen != null)
        {
            events.DisplayResolutionScreen(type, Questions[currentQuestion].AddScore);
        }


        //  AudioManager.Instance.PlaySound((isCorrect) ? "CorrectSFX" : "IncorrectSFX");

        if (type != UIManager.ResolutionScreenType.Finish)
        {
            if (IE_waitTillNextRound != null)
            {
                StopCoroutine(IE_waitTillNextRound);
            }
            IE_waitTillNextRound = WaitTillNextRound();
            StartCoroutine(IE_waitTillNextRound);
        }


    }

    public void UpdateTimer(bool state)
    {
        switch (state)
        {
            case true:
                IE_StartTimer = StartTimer();
                StartCoroutine(IE_StartTimer);

                //timerAnimtor.SetInteger(timerStateParaHash, 2);
                break;
            case false:
                if (IE_StartTimer != null)
                {
                    StopCoroutine(IE_StartTimer);
                }

                // timerAnimtor.SetInteger(timerStateParaHash, 1);
                break;
        }
    }
    IEnumerator StartTimer()
    {
        var totalTime = Questions[currentQuestion].Timer;
        var timeLeft = totalTime;

        timerText.color = timerDefaultColor;
        while (timeLeft > 0)
        {
            timeLeft--;

            //AudioManager.Instance.PlaySound("CountdownSFX");

            if (timeLeft < totalTime / 2 && timeLeft > totalTime / 4)
            {
                timerText.color = timerHalfWayOutColor;
            }
            if (timeLeft < totalTime / 4)
            {
                timerText.color = timerAlmostOutColor;
            }

            timerText.text = timeLeft.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        Accept();
    }

    IEnumerator WaitTillNextRound()
    {
        yield return new WaitForSeconds(GameUtillity.ResolutionDelayTime);
        Display();
    }

    private void SetHighscore()
    {
        var highscore = PlayerPrefs.GetInt(GameUtillity.SavePrefKey + isLevel.ToString());
        if (highscore < events.CurrentFinalScore)
        {
            PlayerPrefs.SetInt(GameUtillity.SavePrefKey + isLevel.ToString(), events.CurrentFinalScore);
        }
    }
    /// <summary>
    /// Function that is called update the score and update the UI.
    /// </summary>
    private void UpdateScore(int add)
    {
        if (add < 0)
        {
            events.CurrentFinalScore -= (add * -1);
        }
        else
        {
            events.CurrentFinalScore += add;
        }

        if (events.ScoreUpdated != null)
        {
            events.ScoreUpdated();
        }
    }

    Question GetRandomQuestion()
    {
        var randomIndex = GetRandomQuestionIndex();
        currentQuestion = randomIndex;
        return Questions[currentQuestion];
    }

    bool CheckAnswers()
    {
        Debug.Log("[CheckAnswers bool]" + CompareAnswers());
        if (!CompareAnswers())
        {
            return false;
        }
        return true;
    }

    bool CompareAnswers()
    {
        Debug.Log("[PickedAnswers.Count]" + PickedAnswers.Count);
        if (PickedAnswers.Count > 0)
        {
            List<int> c = Questions[currentQuestion].GetCorrectAnswers();
            List<int> p = PickedAnswers.Select(x => x.AnswerIndex).ToList();

            var f = c.Except(p).ToList();
            var s = p.Except(c).ToList();

            return !f.Any() && !s.Any();
        }
        return false;
    }
    int GetRandomQuestionIndex()
    {
        var random = 0;
        if (FinishedQuestions.Count < Questions.Length)
        {
            do
            {
                random = UnityEngine.Random.Range(0, Questions.Length);
            } while (FinishedQuestions.Contains(random) || random == currentQuestion);
        }
        return random;
    }
    void Start()
    {

        if (instance == null)
        {
            instance = this;

            //Display();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);

        }

    }

    public void UpdateAnswers(AnswerData newAnswer)
    {
        if (Questions[currentQuestion].Type == AnswerType.Single)
        {
            foreach (var answer in PickedAnswers)
            {
                if (answer != newAnswer)
                {
                    answer.Reset();
                }
            }
            PickedAnswers.Clear();
            PickedAnswers.Add(newAnswer);
        }
        else
        {
            bool alreadyPicked = PickedAnswers.Exists(x => x == newAnswer);
            if (alreadyPicked)
            {
                PickedAnswers.Remove(newAnswer);
            }
            else
            {
                PickedAnswers.Add(newAnswer);
                Debug.Log("[Add answer when multiple]" + Questions[currentQuestion].Answers[newAnswer.AnswerIndex].Info);
            }
        }
    }

    void Awake()
    {
        events.CurrentFinalScore = 0;
        events.StartupHighscore = 0;
        events.currentLevelMaxScore = 0;
        events.isLevel = 0;
        events.PauseGame += GamePasue;
        if (clearAllData)
        {
            Debug.Log("[Clear Data]");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("LevelState", 1);
        }

        var haveSave = PlayerPrefs.GetInt("LevelState") == 0 ? false : true;

        Debug.Log("[Save Check]" + haveSave);
        if (!haveSave)
        {
            PlayerPrefs.SetInt("LevelState", 1);
            LevelState = PlayerPrefs.GetInt("LevelState");
        }
        else
        {
            LevelState = PlayerPrefs.GetInt("LevelState");
        }
    }

    // Update is called once per frame
    public void GamePasue(GameObject overlay, Button pauseButton)
    {
        if (Time.timeScale == 1)
        {

            overlay.SetActive(true);
            pauseButton.interactable = false;
            Time.timeScale = 0;
            events.isPause = true;

            //showPaused();
        }
        else if (Time.timeScale == 0)
        {
            pauseButton.interactable = true;
            overlay.SetActive(false);
            Time.timeScale = 1;
            events.isPause = false;

            //	hidePaused();
        }
    }
    public void LoadQuestions()
    {
        events.CurrentFinalScore = 0;
        events.StartupHighscore = 0;
        events.currentLevelMaxScore = 0;
        events.isLevel = 0;
        _questions = null;
        var resource = "Questions/";
        string path = resource + isLevel.ToString();
        events.isLevel = isLevel;
        events.StartupHighscore = PlayerPrefs.GetInt("Level_" + isLevel.ToString());
        // Object[] objs = Resources.LoadAll(path, typeof(Question));
        // Debug.Log("[Have Questions in level]" + objs.Length + " " + PlayerPrefs.GetInt("Level_" + isLevel.ToString()));
        // if (objs.Length == 0)
        // {
        //     Debug.LogWarning("Wrong while trying to display new Question UI Data. 0 Question find in level " + isLevel.ToString());
        //     return;
        // }
        // var currentLevelMaxScore = 0;

        // _questions = new Question[objs.Length];
        // for (int i = 0; i < objs.Length; i++)
        // {

        //     _questions[i] = (Question)objs[i];
        //     currentLevelMaxScore += _questions[i].AddScore;
        // }
        // events.currentLevelMaxScore = currentLevelMaxScore;
        timerText = GameObject.Find("/MainCanvas/Content/QuestionBG/Timer/Text").GetComponent<Text>();
        var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(seed);
        Display();
    }

    public void RestoreGame()
    {

    }

}
