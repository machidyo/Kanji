using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AnswerDonut : MonoBehaviour
{
    public string Yomi => quiz.Yomi;

    private Quiz quiz;
    private Text answer;

    private float bouncingDuration = 0.1f;
    private bool isBouncing = false;
    private Vector3 originalPos;
    private Vector3 originalScale;
    private Vector3 maxScale;

    void Start()
    {
        originalPos = transform.position;
        originalScale = transform.localScale;
        maxScale = originalScale * 1.2f;
    }
    
    public void SetAnswerBy(Quiz q)
    {
        quiz = q;
        answer = GetComponentInChildren<Text>();
        answer.text = q.Yomi;
    }

    public void Bounce()
    {
        if(isBouncing) return;
        isBouncing = true;
        GetComponent<BoxCollider>().enabled = false;

        DOTween.Sequence()
            .Append(transform.DOMove(originalPos - (Vector3.up * 0.1f), bouncingDuration))
            .Join(transform.DOScale(maxScale, bouncingDuration))
            .Append(transform.DOMove(originalPos, bouncingDuration))
            .Join(transform.DOScale(originalScale * 0.8f, bouncingDuration))
            .Append(transform.DOScale(originalScale, bouncingDuration * 0.2f))
            .AppendCallback(OnFinishedBounding)
            .Play();
    }

    private void OnFinishedBounding()
    {
        // 予期せぬ移動やサイズ変更が発生していた場合に元に戻す
        if (transform.position != originalPos)
        {
            transform.DOMove(originalPos, bouncingDuration);
        }
        if (transform.localScale != originalScale)
        {
            transform.DOScale(originalScale, bouncingDuration);
        }
        
        GetComponent<BoxCollider>().enabled = true;
        isBouncing = false;
    }
}
