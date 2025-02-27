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

        public float PercentOfGame => 1 - timer / (duration == 0 ? 1 : duration);
        private void Awake()
        {
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

  
        }
        public void InitTimer(float duration)
        {
            this.duration = duration;
            timer = duration;
            expired = false;
        }

        private void UpdateTimer()
        {
            if (expired) return;
                timer -= Time.deltaTime;
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
                // Debug.Log(timer);
                await UniTask.WaitForSeconds(0.5f);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}