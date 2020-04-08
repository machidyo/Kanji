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
            });
        
        this.OnTriggerExitAsObservable()
            .Subscribe(x => handRenderer.materials = new[] {off});

        // マウスクリックの設定は片手だけ
        if (isRightHand)
        {
            SetMouseForDebug();
        }
    }

    private void Check(GameObject target)
    {
        Debug.Log($"target is {target.name}.");

        if (target.name.ToLower().StartsWith("answer_donut_"))
        {
            var answer = target.gameObject.GetComponent<AnswerDonut>();
            if (answer != null)
            {
                questioner.Check(answer);
            }
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
