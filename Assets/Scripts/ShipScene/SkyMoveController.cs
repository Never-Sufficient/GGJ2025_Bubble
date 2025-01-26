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
            EventManager.Instance.AddListener(EventName.StartOneDay, OnStartOneDay);
            EventManager.Instance.AddListener(EventName.TimerExpire, OnTimerExpire);
        }

        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(EventName.StartOneDay, OnStartOneDay);
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

        private void OnStartOneDay()
        {
            active = true;
        }

        private void OnTimerExpire()
        {
            active = false;
        }
    }
}