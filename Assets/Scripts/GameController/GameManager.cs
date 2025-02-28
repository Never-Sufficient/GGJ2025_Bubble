using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EventCenter;
using ShipScene;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace GameController
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer srBackground;
        [SerializeField] private EndingEventDirector endingEventDirector;
        
        [SerializeField] private float oneDayDuration;
        [SerializeField] private Image globalDark;
        [SerializeField] private Image globalDay;
        [SerializeField] private Image globalNumber;
        [SerializeField] private Sprite[] dayNumbers;
        [SerializeField] private Sprite[] dayBackgrounds;
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
            dayCount = 1;
            gameTimer = FindObjectOfType<GameTimer>();
        }

        private void GameStart()
        {
            SoundManager.Instance.PlayBirdSound();
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
            SoundManager.Instance.PlayEngine();
            SoundManager.Instance.EffectPlayStr("19");
            if (dayCount == 1)
            {
                var colorD1 = globalDay.color;
                colorD1.a = 1;
                globalDay.DOColor(colorD1, 2f).SetEase(Ease.OutSine);

                var colorN1 = globalNumber.color;
                colorN1.a = 1;
                globalNumber.DOColor(colorN1, 2f).SetEase(Ease.OutSine);
                UniTask.WaitForSeconds(4);

            }
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

            if (dayCount == 5)
            {
                StartEndingEvent();
            }

        }

        private void ChangeBackground()
        {
            srBackground.sprite = dayBackgrounds[dayCount - 1];
        }

        private async UniTask EndOneDay()
        {
            dayCount++;
            changeDayNumber(dayCount);
            Invoke("stopEngineSound", 7f);
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
        private void changeDayNumber(int dayCount)
        {
            switch (dayCount)
            {
                case 2: globalNumber.sprite = dayNumbers[1]; break;
                case 3: globalNumber.sprite = dayNumbers[2]; break;
                case 4: globalNumber.sprite = dayNumbers[3]; break;
                case 5: globalNumber.sprite = dayNumbers[4]; break;
                default: break;

            }


        }

        private void StartEndingEvent()
        {
            endingEventDirector.StartEndingEvent().Forget();
        }
        private void stopEngineSound()
        {
            SoundManager.Instance.StopEngine();
        }
    }
}