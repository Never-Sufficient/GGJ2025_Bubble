using System;
using EventCenter;
using UnityEngine;

namespace ShipScene
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField] private float duration;

        private float timer;
        private bool expired = true;

        private void Start()
        {
            InitTimer(duration);
        }

        private void Update()
        {
            UpdateTimer();
        }

        private void InitTimer(float duration)
        {
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
            //Debug.Log(timer);
        }
    }
}