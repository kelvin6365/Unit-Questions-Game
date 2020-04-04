using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    GameManager gameManager;
    Image image;
    float timeLeft;
    float totleTime;
    bool timerActive = false;
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        // UIManager.GetComponent<UIManager>().enabled = false;

    }
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    public void TimerStart(bool active)
    {
        if (active)
        {
            timerActive = true;

            totleTime = (float)gameManager.TimerTotal;
            timeLeft = totleTime;
            Debug.Log(totleTime + " " + timeLeft);
        }
        else
        {
            timerActive = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {

            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                image.fillAmount = timeLeft / totleTime;
            }
        }
    }
}
