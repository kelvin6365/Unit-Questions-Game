using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelGenerator : MonoBehaviour
{
    [SerializeField] public Text Title = null;
    [SerializeField] public GameObject LevelPrefab;

    [SerializeField] public GameObject LevelUpdatePrefab;
    [SerializeField] RectTransform LevelArea;
    [SerializeField] GameEvents events = null;

    // Start is called before the first frame update
    GameManager gameManager;
    // Start is called before the first frame update
    void Awake()
    {

        gameManager = FindObjectOfType<GameManager>();
    }
    void Start()
    {
        Title.text = events.SelectedQuestionType;
        //
        for (int i = 1; i <= gameManager.TotaleLevels; i++)
        {
            GenLevels(i);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenLevels(int id)
    {

        GameObject newLevelPrefab = (GameObject)Instantiate(LevelPrefab, LevelArea);
        newLevelPrefab.GetComponent<LevelItem>().LoadLevel(id, events.SelectedQuestionType);
        if (id == gameManager.TotaleLevels)
        {
            GameObject newLevelUpdatePrefab = (GameObject)Instantiate(LevelUpdatePrefab, LevelArea);
        }

    }
}
