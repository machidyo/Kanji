using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class AnswerDonut : MonoBehaviour
{
    public string Yomi => quiz.Yomi;

    private Quiz quiz;
    private Text answer;
    
    public void SetAnswerBy(Quiz q)
    {
        quiz = q;
        answer = GetComponentInChildren<Text>();
        answer.text = q.Yomi;
    }
}
