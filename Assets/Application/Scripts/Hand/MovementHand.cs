using System.Linq;
using UnityEngine;

public class MovementHand : MonoBehaviour
{
    [SerializeField] private GameObject cube;

    private static readonly int MAX_MOVEMENT_LENGHT = 10;
    private Movement[] movements = new Movement[MAX_MOVEMENT_LENGHT];
    private int currentIndex = 0;
    
    void Start()
    {
        // 初期化
        for (var i = 0; i < movements.Length; ++i)
        {
            movements[i] = new Movement
            {
                Time = 0.000001f,
                Position = transform.position
            };
        }
    }

    void Update()
    {
        StoreMovement();

        // for debug
        cube.transform.localScale = new Vector3(0.0f, GetVirtualSpeed() * 0.05f, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        var target = other.gameObject;
        if (target.tag.Equals("Answer"))
        {
            var answer = target.gameObject.GetComponent<AnswerDonut>();
            if (answer != null)
            {
                answer.Bounce(GetPositionDifference(), GetVirtualSpeed());
            }
        }
    }

    private void StoreMovement()
    {
        movements[currentIndex] = new Movement
        {
            Time = Time.deltaTime,
            Position = transform.position
        };
        currentIndex++;
        if (currentIndex >= MAX_MOVEMENT_LENGHT)
        {
            currentIndex = 0;
        }
    }

    /// <summary>
    /// MAX_MOVEMENT_LENGTH の frame 分の magnitude の総和をとって、時間で割ることで、仮想的な速度としている
    /// </summary>
    private float GetVirtualSpeed()
    {
        var magnitude = Enumerable
            .Range(0, MAX_MOVEMENT_LENGHT - 1)
            .Select(i =>
            {
                var now = currentIndex - i;
                var previous = currentIndex - i - 1;
                if (now < 0) now += MAX_MOVEMENT_LENGHT; // データはぐるぐる回りながら代入しているので、前に戻す
                if (previous < 0) previous += MAX_MOVEMENT_LENGHT; // データはぐるぐる回りながら代入しているので、前に戻す
                return (movements[now].Position - movements[previous].Position).magnitude;
            })
            .Sum(x => x);
        var deltaTime = movements.Sum(m => m.Time);
        return magnitude / deltaTime;
    }

    private Vector3 GetPositionDifference()
    {
        var previous = currentIndex - (MAX_MOVEMENT_LENGHT - 1);
        if (previous < 0) previous += MAX_MOVEMENT_LENGHT; // データはぐるぐる回りながら代入しているので、前に戻す

        return (movements[previous].Position - movements[currentIndex].Position) * 10.0f;
    }
}
