using EventCenter;
using UnityEngine;

namespace ShipScene
{
    public class SkyMoveController : MonoBehaviour
    {
        [SerializeField] private Transform startPosition, endPosition;
        private GameTimer gameTimer;
        private bool active = false;

        private void Awake()
        {
            EventManager.Instance.AddListener(EventName.GameStart, OnGameStart);
            EventManager.Instance.AddListener(EventName.TimerExpire, OnTimerExpire);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(EventName.GameStart, OnGameStart);
            EventManager.Instance.RemoveListener(EventName.TimerExpire, OnTimerExpire);
        }

        private void Start()
        {
            gameTimer = FindObjectOfType<GameTimer>();
        }

        private void Update()
        {
            if (!active)
                return;
            transform.position = new Vector2(transform.position.x,
                Mathf.Lerp(startPosition.position.y, endPosition.position.y, gameTimer.PercentOfGame));
        }

        private void OnGameStart()
        {
            active = true;
        }

        private void OnTimerExpire()
        {
            active = false;
        }
    }
}