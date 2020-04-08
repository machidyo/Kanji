using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using UniRx.Async;

public class KanjiMananger : MonoBehaviour
{
    // * [ ] 答えが正解が出ている漢字に合わせて変わるようにする
    // * [ ] 初期化でコイン、ひよこの削除
    // * [ ] スコアがマイナスになるのを防ぐ
    // * [ ] 効果音の追加
    
    // * [ ] 答えの数を12に増やして、自分の周りに出す
    // * [ ] ニワトリの数を増やす
    // * [ ] 手で回答したときのフィードバックを視覚的に返す
    // * [ ] 正解してニワトリが消えるモーションを付ける
    // * [ ] コインが貯まるボックスを作る
    // * [ ] 文字をかっこよくする

    public enum GameStatus
    {
        Initialized,
        Start,
        Gaming,
        Finished
    }

    [SerializeField] private Questioner questioner;
    [SerializeField] private AnswerGenerator answerGenerator;

    [SerializeField] private Text HeadUi;
    
    [SerializeField] private Text StatusText;
    [SerializeField] private Text TimerText;
    [SerializeField] private Text ScoreText;

    [SerializeField] private GameObject resultGenerator;
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject chick;
    [SerializeField] private GameObject goal;

    public ReactiveProperty<int> Timer = new ReactiveProperty<int>();
    public ReactiveProperty<int> Score = new ReactiveProperty<int>();
    public ReactiveProperty<GameStatus> Status = new ReactiveProperty<GameStatus>();

    private List<GameObject> coins = new List<GameObject>();
    private List<GameObject> chicks = new List<GameObject>();
    private List<IDisposable> subscribers = new List<IDisposable>();

    void Start()
    {
        Init();
    }

    public void ChangeStatus()
    {
        switch (Status.Value)
        {
            case GameStatus.Initialized:
                CountDown();
                return;
            case GameStatus.Start:
                StartGame();
                return;
            case GameStatus.Finished:
                Init();
                return;
        }
    }

    private void Init()
    {
        Status.DistinctUntilChanged()
            .Subscribe(s => StatusText.text = s.ToString())
            .AddTo(this);
        Timer.DistinctUntilChanged()
            .Subscribe(t => TimerText.text = t <= 0 ? "ゲームしゅうりょう" : t.ToString())
            .AddTo(this);
        Score.DistinctUntilChanged()
            .Subscribe(s => ScoreText.text = s.ToString())
            .AddTo(this);

        Reset();
    }

    private async void CountDown()
    {
        HeadUi.text = "3";
        var temp = Observable.Interval(TimeSpan.FromSeconds(1))
            .Subscribe(x => HeadUi.text = (2 - x).ToString());
        Status.Value = GameStatus.Start;

        await UniTask.Delay(3000);

        temp.Dispose();
        HeadUi.text = "はじまったよ！！！";
        ChangeStatus();

        await UniTask.Delay(3000);

        HeadUi.gameObject.SetActive(false);
    }

    private void StartGame()
    {
        Observable.Interval(TimeSpan.FromSeconds(1))
            .Where(_ => Timer.Value > 0)
            .Subscribe(_ => Timer.Value--)
            .AddTo(subscribers);
        Timer
            .Where(t => t <= 0)
            .Subscribe(_ => StopGame())
            .AddTo(subscribers);
        goal.OnCollisionEnterAsObservable()
            .Where(c => c.collider.name.ToLower().StartsWith("questionchicken"))
            .Subscribe(_ => StopGame())
            .AddTo(subscribers);
        questioner.History
            .ObserveAdd()
            .Subscribe(x =>
            {
                if (x.Value.IsCorrect)
                {
                    coins.Add(Instantiate(coin, resultGenerator.transform.position, Quaternion.identity));
                    Score.Value++;
                }
                else
                {
                    chicks.Add(Instantiate(chick, resultGenerator.transform.position, Quaternion.identity));
                    Score.Value--;
                }
            })
            .AddTo(subscribers);

        questioner.StartGenerate();
        Status.Value = GameStatus.Gaming;
    }

    private async void StopGame()
    {
        questioner.StopGenerate();

        foreach (var subscriber in subscribers)
        {
            subscriber.Dispose();
        }

        HeadUi.text = Score.Value + "てん　でした！";
        HeadUi.gameObject.SetActive(true);

        Status.Value = GameStatus.Finished;

        await UniTask.Delay(5000);
        
        ChangeStatus();
    }

    private void Reset()
    {
        Timer.Value = 30;
        Score.Value = 0;
        ClearAndDestory(coins);
        ClearAndDestory(chicks);
        HeadUi.text = "はじめる　を押してね";
        HeadUi.gameObject.SetActive(true);
        
        questioner.Reset();
        answerGenerator.Reset();
        
        Status.Value = GameStatus.Initialized;
    }

    private void ClearAndDestory(List<GameObject> targets)
    {
        foreach (var t in targets)
        {
            Destroy(t);
        }
        chicks.Clear();
    }
}