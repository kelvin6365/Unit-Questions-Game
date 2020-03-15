using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changeScene : MonoBehaviour
{
    [SerializeField] GameEvents events = null;
    public string sceneName;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);

    }

    public void ExitGameToLevel()
    {
        gameManager.UpdateTimer(false);
        Time.timeScale = 1;
        events.isPause = false;
        events.CurrentFinalScore = 0;
        events.StartupHighscore = 0;
        events.currentLevelMaxScore = 0;
        events.isLevel = 0;
        gameManager.isLevel = 0;
        gameManager.FinishedQuestions = new List<int>();
        SceneManager.LoadScene("Level");
    }

    public void LoadLevelScene(int Level)
    {
        gameManager.isLevel = Level;
        Debug.Log("[LoadLevelScene]" + Level);
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadSceen()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}
