using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class KanjiHand : MonoBehaviour
{
    [SerializeField] private KanjiMananger kanjiMananger;
    
    [SerializeField] private Questioner questioner;
    [SerializeField] private SkinnedMeshRenderer handRenderer;
    [SerializeField] private bool isRightHand;

    [SerializeField] private Material on;
    [SerializeField] private Material off;
    [SerializeField] private GameObject cube;

    // memo
    // ffmpeg -i Moving.mp4 -r 10 -vf scale=320:-1 Moving.gif
    // music
    //   2   retro music pack
    //   2   sounds effect pack
    //   2.5 Whimsical Adventure Music Pack 1
    
    void Start()
    {
        this.OnTriggerEnterAsObservable()
            .Subscribe(x =>
            {
                handRenderer.materials = new[] {on};
                Check(x.gameObject);
            }).AddTo(this);
        
        this.OnTriggerExitAsObservable()
            .Subscribe(x => handRenderer.materials = new[] {off})
            .AddTo(this);

        // マウスクリックの設定は片手だけ
        if (isRightHand)
        {
            SetMouseForDebug();
        }
    }

    void Update()
    {
        // なぜかずっと 0 になっているみたいなので、そこの修正から
        cube.transform.localScale = GetMovement() * 0.1f;
    }

    private static readonly int MAX =60;
    private List<Vector3> cashPositions = new List<Vector3>(MAX);
    private int index = 0;
    void StorePosition()
    {
        cashPositions[index] = transform.position;
        index++;
        if (index >= MAX)
        {
            index = 0;
        }
    }

    Vector3 GetMovement()
    {
        if (cashPositions.Count < MAX) return Vector3.zero; // データ不足のため 0.0f を返す

        var previous = index - 30;
        if (previous < 0) previous += MAX; // データはぐるぐる回りながら代入しているので、前に戻す
        return (cashPositions[index] - cashPositions[previous]);
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
            
            answer.Bounce(GetMovement());
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
