using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnswerGenerator : MonoBehaviour
{
    [SerializeField] private int maxAnswerDonut;
    [SerializeField] private List<GameObject> donuts;
    [SerializeField] private GameObject correct;

    private IList<AnswerParameter> parameters = new List<AnswerParameter>();
    private IList<GameObject> answerDonuts = new List<GameObject>();

    private static IList<Quiz> answers;

    void Start()
    {
        Init();
        Reset();
    }

    private void Init()
    {
        answers = QuizzesLoader.Quizzes;
    }

    public　void Reset()
    {
        foreach (var donut in answerDonuts)
        {
            Destroy(donut);
        }

        answerDonuts.Clear();

        parameters.Clear();
        for (var i = 0; i < maxAnswerDonut; i++)
        {
            parameters.Add(new AnswerParameter
            {
                Id = i,
                GeneratePosition = new Vector3(5f, 0.5f, -4.4f + -0.2f * i),
                IsUsing = false
            });
        }
    }

    public void SetAnswerDonuts(List<Quiz> quizzes)
    {
        Reset();

        // correct
        foreach (var quiz in quizzes)
        {
            SetAnswerDonut(quiz, true);
        }

        // wrong
        var count = maxAnswerDonut - quizzes.Count;
        for (var i = 0; i < count; i++)
        {
            var q = GetQuiz();
            SetAnswerDonut(q, false);
        }
    }

    private void SetAnswerDonut(Quiz quiz, bool isCorrect)
    {
        var d = GetDonutGameObject();
        var p = GetGeneratePosition();

        var donut = Instantiate(d, p, Quaternion.identity);
        var answer = donut.GetComponent<AnswerDonut>();
        answer.SetAnswerBy(quiz);
        answer.IsCorrect = isCorrect;
        answerDonuts.Add(donut);
    }

    private GameObject GetDonutGameObject()
    {
        var i = Random.Range(0, donuts.Count - 1);
        return donuts[i];
    }

    private Vector3 GetGeneratePosition()
    {
        var notUsingIds = parameters
            .Where(p => !p.IsUsing)
            .Select(p => p.Id)
            .ToArray();
        var i = Random.Range(0, notUsingIds.Count() - 1);
        var id = notUsingIds[i];

        var para = parameters.First(p => p.Id == id);
        para.IsUsing = true;
        return para.GeneratePosition;
    }

    private Quiz GetQuiz()
    {
        var i = Random.Range(0, answers.Count - 1);
        return answers[i];
    }
}

class AnswerParameter
{
    public int Id;
    public Vector3 GeneratePosition;
    public bool IsUsing;
}