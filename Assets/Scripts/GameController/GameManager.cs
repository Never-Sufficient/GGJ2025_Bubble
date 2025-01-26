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
        [SerializeField] private Image globalDay;
        [SerializeField] private Image globalNumber;
        [SerializeField] private Sprite[] dayNumbers;
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
            changeDayNumber(dayCount);
            gameTimer.InitTimer(oneDayDuration);
            ChangeBackground();
            this.TriggerEvent(EventName.StartOneDay);

            var color = globalDark.color;
            color.a = 0;
            globalDark.DOColor(color, 5f).SetEase(Ease.OutSine);

            var colorD = globalDay.color;
            colorD.a = 0;
            globalDay.DOColor(colorD, 5f).SetEase(Ease.OutSine);

            var colorN = globalNumber.color;
            colorN.a = 0;
            globalNumber.DOColor(colorN, 5f).SetEase(Ease.OutSine);
            Debug.Log("day1");

        }

        private void ChangeBackground()
        {
            
        }

        private async UniTask EndOneDay()
        {
            SoundManager.Instance.EffectPlayStr("20");
            var color = globalDark.color;
            color.a = 1;
            globalDark.DOColor(color, 5f).SetEase(Ease.OutSine);

            var colorD = globalDay.color;
            colorD.a = 1;
            globalDay.DOColor(colorD, 5f).SetEase(Ease.OutSine);

            var colorN = globalNumber.color;
            colorN.a = 1;
            globalNumber.DOColor(colorN, 5f).SetEase(Ease.OutSine);
            await UniTask.WaitForSeconds(10);
            StartOneDay().Forget();
        }
        public void delayMusic1()
        {
            SoundManager.Instance.PlayEngine();
        }
        private void changeDayNumber(int dayCount)
        {
            switch (dayCount)
            {
                case 1:
                    var colorD1 = globalDay.color;
                    colorD1.a = 1;
                    globalDay.DOColor(colorD1, 2.5f).SetEase(Ease.OutSine);

                    var colorN1 = globalNumber.color;
                    colorN1.a = 1;
                    globalNumber.DOColor(colorN1, 2.5f).SetEase(Ease.OutSine);
                    var colorD2 = globalDay.color;
                    colorD2.a = 0;
                    globalDay.DOColor(colorD2, 5f).SetEase(Ease.OutSine);

                    var colorN2 = globalNumber.color;
                    colorN2.a = 0;
                    globalNumber.DOColor(colorN2, 5f).SetEase(Ease.OutSine);
                    break;
                case 2: globalNumber.sprite = dayNumbers[1]; break;
                case 3: globalNumber.sprite = dayNumbers[2]; break;
                case 4: globalNumber.sprite = dayNumbers[3]; break;
                case 5: globalNumber.sprite = dayNumbers[4]; break;
                default: break;

            }


        }
    }
}