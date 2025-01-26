using System;
using Cysharp.Threading.Tasks;
using EventCenter;
using UnityEngine;

namespace ShipScene
{
    public class GameTimer : MonoBehaviour
    {
        private float duration;

        private float timer;
        private bool expired = true;
        private bool pause = false;

        public float PercentOfGame => 1 - timer / (duration == 0 ? 1 : duration);
        private void Awake()
        {
            EventManager.Instance.AddListener(EventName.GamePause, OnGamePause);
            EventManager.Instance.AddListener(EventName.GameContinue, OnGameContinue);
        }
        private void Start()
        {
            ShowTimer().Forget();
        }

        private void Update()
        {
            UpdateTimer();
        }
        private void OnDisable()
        {
            EventManager.Instance.RemoveListener(EventName.GamePause, OnGamePause);
            EventManager.Instance.RemoveListener(EventName.GameContinue, OnGameContinue);
        }
        public void InitTimer(float duration)
        {
            this.duration = duration;
            timer = duration;
            expired = false;
            pause = false;
        }

        private void UpdateTimer()
        {
            if (expired) return;
            if (!pause)
            {
                timer -= Time.deltaTime;
            }
            if (timer <= 0)
            {
                this.TriggerEvent(EventName.TimerExpire);
                expired = true;
            }
        }

        private async UniTask ShowTimer()
        {
            while (true)
            {
                Debug.Log(timer);
                await UniTask.WaitForSeconds(0.5f);
            }
            // ReSharper disable once FunctionNeverReturns
        }
        private void OnGamePause()
        {
            pause = true;
        }
        private void OnGameContinue()
        {
            pause = false;
        }
    }
}