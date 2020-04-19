using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class KanjiHand : MonoBehaviour
{
    [SerializeField] private KanjiMananger kanjiMananger;
    
    [SerializeField] private Questioner questioner;
    //[SerializeField] private SkinnedMeshRenderer handRenderer;
    [SerializeField] private bool isRightHand;

    [SerializeField] private Material on;
    [SerializeField] private Material off;
    [SerializeField] private GameObject cube;

    // memo
    // 
    // music
    //   2   retro music pack
    //   2   sounds effect pack
    //   2.5 Whimsical Adventure Music Pack 1
    
    void Start()
    {
        this.OnTriggerEnterAsObservable()
            .Subscribe(x =>
            {
                // handRenderer.materials = new[] {on};
                Check(x.gameObject);
            }).AddTo(this);
        
        // this.OnTriggerExitAsObservable()
        //     .Subscribe(x => handRenderer.materials = new[] {off})
        //     .AddTo(this);

        // マウスクリックの設定は片手だけ
        if (isRightHand)
        {
            SetMouseForDebug();
        }

        // 初期化、後で実装考える
        for (var i = 0; i < movements.Length; ++i)
        {
            movements[i] = new Movement
            {
                Time = 0f, 
                Position = transform.position
            };
        }
    }

    void Update()
    {
        StorePosition();
        
        // for debug
        cube.transform.localScale = Vector3.one * (GetVirtualSpeed() * 0.1f);
    }

    private static readonly int MAX =60;
    private Movement[] movements = new Movement[MAX];
    private int index = 0;
    private void StorePosition()
    {
        movements[index] = new Movement()
        {
            Time = Time.deltaTime,
            Position = transform.position
        };
        index++;
        if (index >= MAX)
        {
            index = 0;
        }
    }

    private float GetVirtualSpeed()
    {
        var previous = index - 1;
        if (previous < 0) previous += MAX; // データはぐるぐる回りながら代入しているので、前に戻す

        var distance = (movements[index].Position - movements[previous].Position).magnitude;
        var deltaTime = movements.Sum(m => m.Time);
        return  distance / deltaTime;
    }

    private Vector3 GetTargetPosition()
    {
        var previous = index - 20;
        if (previous < 0) previous += MAX; // データはぐるぐる回りながら代入しているので、前に戻す
        
        return (movements[index].Position - movements[previous].Position);
    }

    private void Check(GameObject target)
    {
        Debug.Log($"target is {target.name}.");

        if (target.name.ToLower().StartsWith("answer_donut_"))
        {
            var answer = target.gameObject.GetComponent<AnswerDonut>();
            if (answer != null)
            {
                // todo: コメントを戻す
                // questioner.Check(answer);
            }
            
            answer.Bounce(GetTargetPosition(), GetVirtualSpeed());
        }

        if (target.name.ToLower().StartsWith("ui"))
        {
            if (kanjiMananger.Status.Value == KanjiMananger.GameStatus.Initialized)
            {
                kanjiMananger.ChangeStatus();
            }
        }
    }
    
    private void SetMouseForDebug()
    {
        Observable
            .EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ =>
            {
                Debug.Log("clicked.");
                var target = TryGetObject();
                if(target == null) Debug.Log("target is null.");
                if (target != null)
                {
                    Check(target);
                }
            });
    }
    private GameObject TryGetObject()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray.origin, ray.direction, out var hit, Mathf.Infinity) 
            ? hit.collider.gameObject 
            : null;
    }
}

public class Movement
{
    public float Time;
    public Vector3 Position;
}
