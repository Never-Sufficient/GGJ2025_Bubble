using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EventCenter;
using ShipScene;
using UnityEngine;
using UnityEngine.UI;

namespace GameController
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float oneDayDuration;
        [SerializeField] private Image globalDark;
        private int dayCount;
        private GameTimer gameTimer;

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

        private void GameStart()
        {
            SoundManager.Instance.PlayBirdSound();
            SoundManager.Instance.EffectPlayStr("19");
            Invoke("delayMusic1", 6.0f);
            var color = globalDark.color;
            color.a = 1;
            globalDark.color = color;
            StartOneDay().Forget();
        }

        private void OnGameStart()
        {
            GameStart();
        }

        private void OnTimerExpire()
        {
            EndOneDay().Forget();
        }

        private async UniTask StartOneDay()
        {
            dayCount++;
            gameTimer.InitTimer(oneDayDuration);
            ChangeBackground();
            this.TriggerEvent(EventName.StartOneDay);

            var color = globalDark.color;
            color.a = 0;
            globalDark.DOColor(color, 5f).SetEase(Ease.OutSine);
        }

        private void ChangeBackground()
        {
            
        }

        private async UniTask EndOneDay()
        {
            var color = globalDark.color;
            color.a = 1;
            globalDark.DOColor(color, 5f).SetEase(Ease.OutSine);
            await UniTask.WaitForSeconds(10);
            StartOneDay().Forget();
        }
        public void delayMusic1()
        {
            SoundManager.Instance.PlayEngine();
        }
    }
}