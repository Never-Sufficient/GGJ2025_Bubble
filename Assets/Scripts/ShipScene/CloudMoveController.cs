using EventCenter;
using UnityEngine;

namespace ShipScene
{
    public class CloudMoveController : MonoBehaviour
    {
        [SerializeField] private Transform startPosition, endPosition;
        private GameTimer gameTimer;
        private bool active = false;

        private void Awake()
        {
            EventManager.Instance.AddListener(EventName.StartOneDay, OnGameStart);
            EventManager.Instance.AddListener(EventName.TimerExpire, OnTimerExpire);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(EventName.StartOneDay, OnGameStart);
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
            transform.position = new Vector2(
                Mathf.Lerp(startPosition.position.x, endPosition.position.x, gameTimer.PercentOfGame),
                transform.position.y);
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