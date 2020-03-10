using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubmitButton : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        this.gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                gameManager.GetComponent<GameManager>().Accept();
            });
    }



    // Update is called once per frame
    void Update()
    {

    }
}
