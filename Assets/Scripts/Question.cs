
using UnityEngine;
using System.Collections.Generic;
using System;
public enum AnswerType { Multi, Single }

[Serializable()]
public class Answer
{
    public string Info = string.Empty;

    public bool IsCorrect = false;


    public Answer() { }

}


[Serializable()]
public class Question
{
    // public string QuestionType = null;
    public string Info = null;
    public Answer[] Answers = null;
    public bool UseTimer = false;
    public int Timer = 0;

    public AnswerType Type = AnswerType.Single;
    public int AddScore = 0;

    public Question()
    {

    }

    public List<int> GetCorrectAnswers()
    {
        List<int> CorrectAnswers = new List<int>();
        for (int i = 0; i < Answers.Length; i++)
        {
            if (Answers[i].IsCorrect)
            {
                CorrectAnswers.Add(i);
            }
        }
        return CorrectAnswers;
    }

}
