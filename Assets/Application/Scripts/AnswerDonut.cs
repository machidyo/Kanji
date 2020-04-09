using DG.Tweening;
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

    private Vector3 max = Vector3.one * 0.6f;
    private float duration = 0.1f;

    public void Bounce()
    {
        var pos = transform.position;
        var scale = transform.localScale;
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(pos - (Vector3.up * 0.1f), duration));
        sequence.Join(transform.DOScale(max, duration));
        sequence.Append(transform.DOMove(pos, duration));
        sequence.Join(transform.DOScale(scale * 0.8f, duration));
        sequence.Append(transform.DOScale(scale, duration * 0.2f));
    }
}