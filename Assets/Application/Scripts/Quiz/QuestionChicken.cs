using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class QuestionChicken : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    public Transform Target;
    public Text Answer;
    public Text Question;

    private Animator animator;
    private IDisposable checker;
    private IDisposable move;

    void Start()
    {
        animator = GetComponent<Animator>();
        Idle();

        checker = Observable.Interval(TimeSpan.FromSeconds(0.1f))
            .Where(_ => (Target.position - transform.position).sqrMagnitude < 4)
            .First()
            .Subscribe(_ => Run());
        
        Walk();
        audioSource.Play();
    }

    private void OnDestroy()
    {
        checker.Dispose();
        move.Dispose();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.name.ToLower().StartsWith("goal_doughnut"))
        {
            checker.Dispose();
            Eat();
            Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ => Idle()).AddTo(this);
        }
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

    private void StartAnimation(string kind)
    {
        Idle();

        animator.SetBool(kind, true);
    }
}