using System.Collections.Generic;

public class Quiz
{
    public int Id;
    public string Kanji;
    public string Yomi;
}

public class Result
{
    public List<string> Questions;
    public AnswerDonut Answer;
    public bool IsCorrect;
}