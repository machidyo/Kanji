using UniRx;
using UnityEngine;

public class KanjiHand : MonoBehaviour
{
    [SerializeField] private KanjiManager kanjiManager;
    [SerializeField] private Questioner questioner;

    [SerializeField] private bool isRightHand;

    [SerializeField] private SkinnedMeshRenderer handRenderer;
    [SerializeField] private Material on;
    [SerializeField] private Material off;

    void Start()
    {
        // マウスクリックの設定は片手だけ
        if (isRightHand)
        {
            SetMouseForDebug();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        handRenderer.materials = new[] {on};
        Check(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        handRenderer.materials = new[] {off};
    }

    private void Check(GameObject target)
    {
        Debug.Log($"{(isRightHand ? "Right" : "Left")} hand touch {target.name}({target.tag}).");

        if (target.tag.Equals("Answer"))
        {
            var answer = target.gameObject.GetComponent<AnswerDonut>();
            if (answer != null)
            {
                questioner.Check(answer);
            }
        }

        if (target.tag.Equals("Operation"))
        {
            if (kanjiManager.Status.Value == KanjiManager.GameStatus.Initialized)
            {
                kanjiManager.ChangeStatus();
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
                if (target == null) Debug.Log("target is null.");
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