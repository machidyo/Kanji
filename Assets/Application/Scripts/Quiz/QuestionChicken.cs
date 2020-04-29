using System;
using DG.Tweening;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

public class QuestionChicken : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject appearance;
    [SerializeField] private GameObject tornado;

    public Transform Target;
    public Text Answer;
    public Text Question;

    private IDisposable checker;
    private IDisposable move;

    void Start()
    {
        Init();   
        Appearance();
    }
    
    private void OnDestroy()
    {
        checker.Dispose();
        move.Dispose();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag.Equals("Goal"))
        {
            checker.Dispose();
            Eat();
            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => Idle()).AddTo(this);
        }
    }

    private void Init()
    {
        Idle();
        checker = Observable.Interval(TimeSpan.FromSeconds(0.1f))
            .Where(_ => (Target.position - transform.position).sqrMagnitude < 4)
            .First()
            .Subscribe(_ => Run());
    }

    private async void Appearance()
    {
        Instantiate(appearance, transform.position, Quaternion.Euler(90f, 0f, 90f));

        await UniTask.Delay(500);

        audioSource.Play();
        Walk();
    }
    
    public void Idle()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        animator.SetBool("Eat", false);

        move?.Dispose();
    }

    private void Walk()
    {
        Move("Walk", 0.1f);
    }

    private void Run()
    {
        Move("Run", 0.05f);
    }

    private void Eat()
    {
        StartAnimation("Eat");
    }
    
    private void Move(string kind, float speed)
    {
        StartAnimation(kind);

        var duration = 0.1f;
        move = Observable.Interval(TimeSpan.FromSeconds(duration)).Subscribe(_ =>
        {
            var tra = transform;
            var add = tra.rotation * new Vector3(0, 0, speed);
            transform.DOMove(tra.position + add, duration);
            transform.DOLookAt(Target.position, duration, AxisConstraint.Y);
        });
    }

    public void Bye()
    {
        Idle();

        var pos = transform.position;
        Instantiate(tornado, pos, Quaternion.Euler(-90f, 90f, 0f));
        var end = pos + new Vector3(0, 5, 0);
        transform.DOMove(end, 5f);
        DOTween.Sequence()
            .Append(transform.DORotate(new Vector3(0, 360, 0), 0.1f, RotateMode.FastBeyond360))
            .Append(transform.DORotate(new Vector3(0, 360, 0), 0.1f, RotateMode.FastBeyond360))
            .Append(transform.DORotate(new Vector3(0, 360, 0), 0.1f, RotateMode.FastBeyond360))
            .Append(transform.DORotate(new Vector3(0, 360, 0), 0.1f, RotateMode.FastBeyond360))
            .Append(transform.DORotate(new Vector3(0, 360, 0), 0.1f, RotateMode.FastBeyond360))
            .Append(transform.DORotate(new Vector3(0, 360, 0), 0.1f, RotateMode.FastBeyond360))
            .Append(transform.DORotate(new Vector3(0, 360, 0), 0.1f, RotateMode.FastBeyond360))
            .Append(transform.DORotate(new Vector3(0, 360, 0), 0.1f, RotateMode.FastBeyond360))
            .Append(transform.DORotate(new Vector3(0, 360, 0), 0.1f, RotateMode.FastBeyond360));
    }

    private void StartAnimation(string kind)
    {
        Idle();
        animator.SetBool(kind, true);
    }
}