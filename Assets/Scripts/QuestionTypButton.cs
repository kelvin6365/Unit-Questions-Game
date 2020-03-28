using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionTypButton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameEvents events = null;
    void Start()
    {
        Debug.Log(this.gameObject.GetComponentInChildren<Text>().text);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClickQuestionTypeButton()
    {
        events.SelectedQuestionType = this.gameObject.GetComponentInChildren<Text>().text;

    }
}
