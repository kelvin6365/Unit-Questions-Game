using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
// using UnityEngine.SceneManagement;
using System.IO;
using System.Xml.Serialization;
using EasyMobile;
using Coffee.UIExtensions;
public class GameManager : MonoBehaviour
{

    [SerializeField] public GameObject MainUITransitionEffect;
    [SerializeField] public int coins;
    [SerializeField] private bool clearAllData;
    [SerializeField] public bool isPlaying;
    [SerializeField] public bool isPause;
    [SerializeField] public int isLevel = 0;
    [SerializeField] public int LevelState;
    [Space]
    [SerializeField] public QuestionTypeLevels[] QuestionTypeLevels;

    [SerializeField] public int TotaleLevels;
    [Space]

    private Data data = new Data();
    // Question[] _questions = null;
    // public Question[] Questions { get { return _questions; } }
    [SerializeField] GameEvents events = null;

    [SerializeField] Animator timerAnimtor = null;
    [SerializeField] Text timerText = null;
    [SerializeField] Color timerHalfWayOutColor = Color.yellow;
    [SerializeField] Color timerAlmostOutColor = Color.red;
    private Color timerDefaultColor = Color.black;

    private List<AnswerData> PickedAnswers = new List<AnswerData>();
    public List<int> FinishedQuestions = new List<int>();

    private int currentQuestion = 0;
    private int timerStateParaHash = 0;
    private IEnumerator IE_waitTillNextRound = null;
    private IEnumerator IE_StartTimer = null;
    private bool isCoroutineExecuting = false;

    private bool IsFinished
    {
        get
        {
            return (FinishedQuestions.Count < data.Questions.Length) ? false : true;
        }
    }

    public void Translation()
    {
        MainUITransitionEffect.GetComponent<UITransitionEffect>().Hide();
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
        events.PauseGame += GamePasue;

    }

    void OnDisable()
    {
        events.updateQuestionAnswer -= UpdateAnswers;
        events.PauseGame -= GamePasue;
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

        UpdateTimer(question.UseTimer);

    }

    public void Accept()
    {
        UpdateTimer(false);

        Debug.Log("[Start Accept]");
        Debug.Log("[current question] " + data.Questions[currentQuestion].Info);
        foreach (var answer in data.Questions[currentQuestion].GetCorrectAnswers())
        {
            Debug.Log(" [Question currect Answer`s]" + data.Questions[currentQuestion].Answers[answer].Info);
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
        UpdateScore((isCorrect) ? data.Questions[currentQuestion].AddScore : -data.Questions[currentQuestion].AddScore);

        if (IsFinished)
        {
            SetHighscore();

        }
        var type
                  =
                //   (IsFinished)
                //   ? UIManager.ResolutionScreenType.Finish
                //   : 
                (isCorrect) ? UIManager.ResolutionScreenType.Correct
                  : UIManager.ResolutionScreenType.Incorrect;


        if (events.DisplayResolutionScreen != null)
        {
            events.DisplayResolutionScreen(type, data.Questions[currentQuestion].AddScore);
        }


        //  AudioManager.Instance.PlaySound((isCorrect) ? "CorrectSFX" : "IncorrectSFX");

        if (!IsFinished)
        {
            if (IE_waitTillNextRound != null)
            {
                StopCoroutine(IE_waitTillNextRound);
            }
            if (!IsFinished)
                IE_waitTillNextRound = WaitTillNextRound();
            StartCoroutine(IE_waitTillNextRound);
        }
        if (IsFinished)
        {

            StartCoroutine(ExecuteAfterTime(UIManager.ResolutionScreenType.Finish));
        }
    }

    public void UpdateTimer(bool state)
    {
        Debug.Log("[UpdateTimer]");
        if (!timerAnimtor)
        {
            timerAnimtor = GameObject.Find("/MainCanvas/Content/QuestionBG/Timer").GetComponent<Animator>();
        }
        switch (state)
        {
            case true:
                IE_StartTimer = StartTimer();
                StartCoroutine(IE_StartTimer);

                timerAnimtor.SetInteger(timerStateParaHash, 2);

                break;
            case false:
                if (IE_StartTimer != null)
                {
                    StopCoroutine(IE_StartTimer);
                }

                timerAnimtor.SetInteger(timerStateParaHash, 0);
                break;
        }
    }
    IEnumerator StartTimer()
    {
        var totalTime = data.Questions[currentQuestion].Timer;
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


    IEnumerator ExecuteAfterTime(UIManager.ResolutionScreenType type)
    {

        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        events.DisplayResolutionScreen(type, data.Questions[currentQuestion].AddScore);

    }

    IEnumerator WaitTillNextRound()
    {
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        Display();
    }

    private void SetHighscore()
    {
        var highscore = PlayerPrefs.GetInt(events.SelectedQuestionType + "_" + GameUtility.SavePrefKey + isLevel.ToString());
        if (highscore < events.CurrentFinalScore)
        {
            PlayerPrefs.SetInt(events.SelectedQuestionType + "_" + GameUtility.SavePrefKey + isLevel.ToString(), events.CurrentFinalScore);
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
        return data.Questions[currentQuestion];
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
            List<int> c = data.Questions[currentQuestion].GetCorrectAnswers();
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
        if (FinishedQuestions.Count < data.Questions.Length)
        {
            do
            {
                random = UnityEngine.Random.Range(0, data.Questions.Length);
            } while (FinishedQuestions.Contains(random) || random == currentQuestion);
        }
        return random;
    }
    void Start()
    {
        MainUITransitionEffect.GetComponent<UITransitionEffect>().Hide();
        if (instance == null)
        {
            instance = this;

            Advertising.ShowBannerAd(BannerAdPosition.Bottom);
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
        if (data.Questions[currentQuestion].Type == AnswerType.Single)
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
                Debug.Log("[Add answer when multiple]" + data.Questions[currentQuestion].Answers[newAnswer.AnswerIndex].Info);
            }
        }
    }

    void Awake()
    {

        events.SelectedQuestionType = null;
        events.CurrentFinalScore = 0;
        events.StartupHighscore = 0;
        events.currentLevelMaxScore = 0;
        events.isLevel = 0;

        if (clearAllData)
        {
            Debug.Log("[Clear Data]");
            PlayerPrefs.DeleteAll();
            // PlayerPrefs.SetInt("LevelState", 1);
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
            // overlay.transform.parent.gameObject.GetComponent<CanvasGroup>
            Time.timeScale = 1;
            events.isPause = false;

            //	hidePaused();
        }
    }

    void LoadData(int levelId, string QuestionType)
    {
        var path = GameUtility.FileName + levelId + "_" + QuestionType;
        Debug.Log(GameUtility.FileName + levelId + "  " + Resources.Load(path));

        XmlSerializer serializer = new XmlSerializer(typeof(Data));
        string _xml = Resources.Load(path).ToString();
        // data = Data.FetchInGame(path);
        StringReader reader = new StringReader(_xml);
        Data items = serializer.Deserialize(reader) as Data;
        reader.Close();
        Debug.Log(items);
        data = items;
        var currentLevelMaxScore = 0;

        for (int i = 0; i < data.Questions.Length; i++)
        {
            Debug.Log(data.Questions[i].Info);
            currentLevelMaxScore += data.Questions[i].AddScore;
        }
        events.currentLevelMaxScore = currentLevelMaxScore;

        timerStateParaHash = Animator.StringToHash("TimerState");
        var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(seed);
        Display();
    }
    public void LoadQuestions()
    {
        events.CurrentFinalScore = 0;
        events.StartupHighscore = 0;
        events.currentLevelMaxScore = 0;
        events.isLevel = 0;
        // _questions = null;
        // var resource = "Questions/" + events.SelectedQuestionType + "/";
        // string path = resource + isLevel.ToString();
        events.isLevel = isLevel;
        events.StartupHighscore = PlayerPrefs.GetInt(events.SelectedQuestionType + "_" + "Level_" + isLevel.ToString());
        timerText = GameObject.Find("/MainCanvas/Content/QuestionBG/Timer/Text").GetComponent<Text>();
        var haveSave = PlayerPrefs.GetInt(events.SelectedQuestionType + "_" + "LevelState") == 0 ? false : true;

        Debug.Log("[Save Check]" + haveSave);
        if (!haveSave)
        {
            PlayerPrefs.SetInt(events.SelectedQuestionType + "_" + "LevelState", 1);
            LevelState = PlayerPrefs.GetInt(events.SelectedQuestionType + "_" + "LevelState");
        }
        else
        {
            LevelState = PlayerPrefs.GetInt(events.SelectedQuestionType + "_" + "LevelState");
        }

        LoadData(events.isLevel, events.SelectedQuestionType);
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

    }

    public void RestoreGame()
    {

    }

    public void GetSelectedTypeData(string type)
    {
        int SelectedTypeTotalLevels = 0;
        for (var i = 0; i < QuestionTypeLevels.Length; i++)
        {
            if (QuestionTypeLevels[i].QuestionType == type)
            {
                SelectedTypeTotalLevels = QuestionTypeLevels[i].TotleLevels;
            }
        }
        TotaleLevels = SelectedTypeTotalLevels;

    }

}
