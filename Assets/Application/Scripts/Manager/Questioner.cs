using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Random = UnityEngine.Random;

public class Questioner : MonoBehaviour
{
    [SerializeField] private AnswerGenerator answerGenerator;
    [SerializeField] private GameObject chicken;
    [SerializeField] private GameObject start;
    [SerializeField] private GameObject goal;


    public ReactiveDictionary<int, Result> History = new ReactiveDictionary<int, Result>();
    
    private static IList<Quiz> quizzes;

    private IList<GameObject> chickens = new List<GameObject>();
    private IDisposable generator;

    void Start()
    {
        quizzes = QuizzesLoader.Quizzes;
    }

    public void Reset()
    {
        foreach (var c in chickens)
        {
            Destroy(c);
        }

        chickens.Clear();
    }

    public void StartGenerate()
    {
        Generate();
        generator = Observable.Interval(TimeSpan.FromSeconds(6)).Subscribe(_ => Generate());
    }

    public void StopGenerate()
    {
        foreach (var c in chickens)
        {
            c.GetComponent<QuestionChicken>().Idle();
        }

        generator?.Dispose();
    }

    public async void Check(AnswerDonut answerDonut)
    {
        var result = new Result
        {
            Questions = chickens.Select(c => c.GetComponent<QuestionChicken>().Question.text).ToList(),
            Answer = answerDonut,
            IsCorrect = chickens.Any(c => c.GetComponent<QuestionChicken>().Answer.text == answerDonut.Yomi)
        };

        var message = result.IsCorrect ? "正解！" : "ブーーー";
        Debug.Log(message);

        if (result.IsCorrect)
        {
            var target = chickens.First(c => c.GetComponent<QuestionChicken>().Answer.text == answerDonut.Yomi);
            target.GetComponent<QuestionChicken>().Bye();
            answerGenerator.Reset();

            await UniTask.Delay(1000);
            
            chickens.Remove(target);
            Destroy(target);
        }

        History.Add(History.Values.Count, result);
    }

    private async void Generate()
    {
        var quiz = GetQuiz();
        var bird = Instantiate(chicken, start.transform.position, Quaternion.identity);
        var qChicken = bird.GetComponent<QuestionChicken>();
        qChicken.Target = goal.transform;
        qChicken.Question.text = quiz.Kanji;
        qChicken.Answer.text = quiz.Yomi;

        chickens.Add(bird);

        answerGenerator.SetAnswerDonuts(new List<Quiz> {quiz});
    }

    private Quiz GetQuiz()
    {
        var i = Random.Range(0, quizzes.Count - 1);
        return quizzes[i];
    }
}
