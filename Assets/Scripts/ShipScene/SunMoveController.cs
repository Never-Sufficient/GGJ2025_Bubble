using System;
using EventCenter;
using UnityEngine;
using UnityEngine.Splines;

namespace ShipScene
{
    public class SunMoveController : MonoBehaviour
    {
        [SerializeField] private SplineContainer sunPath;

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
            transform.position = sunPath.EvaluatePosition(gameTimer.PercentOfGame);
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