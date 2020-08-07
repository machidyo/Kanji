using System;
using System.Security.Cryptography;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AnswerDonut : MonoBehaviour
{
    [SerializeField] private GameObject hitPuff;
    [SerializeField] private GameObject correctExplosion;
    
    public string Yomi => quiz.Yomi;
    public bool IsCorrect { get; set; } = false;

    private Quiz quiz;
    private Text answer;

    private float bouncingDuration = 1.0f;
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag.Equals("Hand"))
        {
            if (IsCorrect)
            {
                Instantiate(correctExplosion, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(hitPuff, other.transform.position, Quaternion.identity);
            }
        }
    }

    public void SetAnswerBy(Quiz q)
    {
        quiz = q;
        answer = GetComponentInChildren<Text>();
        answer.text = q.Yomi;
    }

    public void Bounce(Vector3 target, float speed)
    {
        if(isBouncing) return;
        isBouncing = true;
        GetComponent<BoxCollider>().enabled = false;

        if (Mathf.Abs(speed) < 0.01f) speed = 0.01f; // speed が 0 だと duration のところで 0 割り算になるのでとりあえず防ぐ
        var duration = bouncingDuration / (speed * 10);
        Debug.Log("duration = " + duration);
        DOTween.Sequence()
            .Append(transform.DOMove(originalPos + target, duration))
            .Join(transform.DOScale(maxScale, duration))
            .Append(transform.DOMove(originalPos, duration))
            .Join(transform.DOScale(originalScale * 0.8f, duration))
            .Append(transform.DOScale(originalScale, duration * 0.2f))
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
