using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] GameObject UIManager;
    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        // UIManager.GetComponent<UIManager>().enabled = false;

    }
    void Start()
    {
        // UIManager.GetComponent<UIManager>().enabled = true;
        Debug.Log("[Level] " + gameManager.isLevel);
        gameManager.LoadQuestions();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
